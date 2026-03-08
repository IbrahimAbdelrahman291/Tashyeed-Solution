using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.Modules.Expenses.ViewModels;

namespace Tashyeed.Web.Modules.Expenses.Services
{
    public class ExpenseService : IExpenseService
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ExpenseService(AppDBContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IEnumerable<ExpenseListVM>> GetAllAsync()
        {
            var expenses = await _context.Expenses
                .Include(e => e.Project)
                .Include(e => e.SubmittedBy)
                .Include(e => e.ApprovedBy)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ExpenseListVM>>(expenses);
        }

        public async Task<IEnumerable<ExpenseListVM>> GetByProjectAsync(int projectId)
        {
            var expenses = await _context.Expenses
                .Include(e => e.Project)
                .Include(e => e.SubmittedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.ProjectId == projectId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ExpenseListVM>>(expenses);
        }

        public async Task<IEnumerable<ExpenseListVM>> GetMyExpensesAsync(string userId)
        {
            var expenses = await _context.Expenses
                .Include(e => e.Project)
                .Include(e => e.SubmittedBy)
                .Include(e => e.ApprovedBy)
                .Where(e => e.SubmittedByUserId == userId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ExpenseListVM>>(expenses);
        }

        public async Task AddExpenseAsync(AddExpenseVM vm, string submittedByUserId)
        {
            var expense = _mapper.Map<Expense>(vm);
            expense.SubmittedByUserId = submittedByUserId;
            expense.HasInvoice = true;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "expenses");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.InvoiceImage!.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await vm.InvoiceImage.CopyToAsync(stream);

            expense.InvoiceImagePath = $"/uploads/expenses/{fileName}";

            _context.Expenses.Add(expense);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ApproveExpenseAsync(int expenseId, string approvedByUserId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense is null || expense.Status != RequestStatus.Pending) return false;

            expense.Status = RequestStatus.Approved;
            expense.ApprovedByUserId = approvedByUserId;

            // نخصم من الـ RemainingAmount في العهدة
            var custody = await _context.Custodies
                .Where(c => c.GivenToUserId == expense.SubmittedByUserId
                    && c.ProjectId == expense.ProjectId
                    && c.Status == CustodyStatus.Confirmed)
                .OrderByDescending(c => c.CreatedAt)
                .FirstOrDefaultAsync();

            if (custody != null)
                custody.RemainingAmount -= expense.Amount;

            // نزود الـ SpentAmount في المشروع
            var project = await _context.Projects.FindAsync(expense.ProjectId);
            if (project != null)
                project.SpentAmount += expense.Amount;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectExpenseAsync(int expenseId, string approvedByUserId)
        {
            var expense = await _context.Expenses.FindAsync(expenseId);
            if (expense is null || expense.Status != RequestStatus.Pending) return false;

            expense.Status = RequestStatus.Rejected;
            expense.ApprovedByUserId = approvedByUserId;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
