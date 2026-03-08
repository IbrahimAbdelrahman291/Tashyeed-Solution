using Tashyeed.Web.Modules.Expenses.ViewModels;

namespace Tashyeed.Web.Modules.Expenses.Services
{
    public interface IExpenseService
    {
        Task<IEnumerable<ExpenseListVM>> GetAllAsync();
        Task<IEnumerable<ExpenseListVM>> GetByProjectAsync(int projectId);
        Task<IEnumerable<ExpenseListVM>> GetMyExpensesAsync(string userId);
        Task AddExpenseAsync(AddExpenseVM vm, string submittedByUserId);
        Task<bool> ApproveExpenseAsync(int expenseId, string approvedByUserId);
        Task<bool> RejectExpenseAsync(int expenseId, string approvedByUserId);
    }
}
