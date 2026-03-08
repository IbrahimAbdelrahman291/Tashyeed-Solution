using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.Modules.Procurement.ViewModels;

namespace Tashyeed.Web.Modules.Procurement.Services
{
    public class ProcurementService : IProcurementService
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public ProcurementService(AppDBContext context, IMapper mapper, IWebHostEnvironment env)
        {
            _context = context;
            _mapper = mapper;
            _env = env;
        }

        public async Task<IEnumerable<ProcurementListVM>> GetAllAsync()
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.Project)
                .Include(pr => pr.RequestedBy)
                .Include(pr => pr.PurchaseOrder)
                    .ThenInclude(po => po != null ? po.PurchasedBy : null)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProcurementListVM>>(requests);
        }

        public async Task<IEnumerable<ProcurementListVM>> GetByProjectAsync(int projectId)
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.Project)
                .Include(pr => pr.RequestedBy)
                .Include(pr => pr.PurchaseOrder)
                    .ThenInclude(po => po != null ? po.PurchasedBy : null)
                .Where(pr => pr.ProjectId == projectId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProcurementListVM>>(requests);
        }

        public async Task<IEnumerable<ProcurementListVM>> GetPendingAsync()
        {
            var requests = await _context.PurchaseRequests
                .Include(pr => pr.Project)
                .Include(pr => pr.RequestedBy)
                .Where(pr => pr.Status == RequestStatus.Pending)
                .ToListAsync();

            return _mapper.Map<IEnumerable<ProcurementListVM>>(requests);
        }

        public async Task CreateRequestAsync(PurchaseRequestVM vm, string requestedByUserId)
        {
            var request = _mapper.Map<PurchaseRequest>(vm);
            request.RequestedByUserId = requestedByUserId;

            _context.PurchaseRequests.Add(request);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CreateOrderAsync(PurchaseOrderVM vm, string purchasedByUserId)
        {
            var request = await _context.PurchaseRequests.FindAsync(vm.PurchaseRequestId);
            if (request is null || request.Status != RequestStatus.Pending) return false;

            // لو الدفع من العهدة نتأكد إن عنده عهدة كافية
            if (vm.PaymentMethod == PaymentMethod.FromCustody)
            {
                var custody = await _context.Custodies
                    .Where(c => c.GivenToUserId == purchasedByUserId
                        && c.ProjectId == vm.ProjectId
                        && c.Status == CustodyStatus.Confirmed
                        && c.RemainingAmount >= vm.Amount)
                    .OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefaultAsync();

                if (custody is null) return false;

                custody.RemainingAmount -= vm.Amount;
            }

            // حفظ الفاتورة
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "procurement");
            Directory.CreateDirectory(uploadsFolder);
            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(vm.InvoiceImage.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await vm.InvoiceImage.CopyToAsync(stream);

            var order = new PurchaseOrder
            {
                PurchaseRequestId = vm.PurchaseRequestId,
                ProjectId = vm.ProjectId,
                PurchasedByUserId = purchasedByUserId,
                Amount = vm.Amount,
                PaymentMethod = vm.PaymentMethod,
                InvoiceImagePath = $"/uploads/procurement/{fileName}"
            };

            request.Status = RequestStatus.Approved;

            // زيادة SpentAmount في المشروع في الحالتين
            var project = await _context.Projects.FindAsync(vm.ProjectId);
            if (project != null)
                project.SpentAmount += vm.Amount;

            _context.PurchaseOrders.Add(order);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> DeleteRequestAsync(int requestId, string userId)
        {
            var request = await _context.PurchaseRequests
                    .FirstOrDefaultAsync(r => r.Id == requestId
                    && r.RequestedByUserId == userId
                    && r.Status == RequestStatus.Pending);

            if (request is null) return false;

            _context.PurchaseRequests.Remove(request);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<bool> RevertOrderAsync(int requestId, string userId)
        {
            var request = await _context.PurchaseRequests
                .Include(r => r.PurchaseOrder)
                .FirstOrDefaultAsync(r => r.Id == requestId
                    && r.Status == RequestStatus.Approved);

            if (request is null || request.PurchaseOrder is null) return false;

            // لو الدفع كان من العهدة نرد المبلغ
            if (request.PurchaseOrder.PaymentMethod == PaymentMethod.FromCustody)
            {
                var custody = await _context.Custodies
                    .Where(c => c.GivenToUserId == userId
                        && c.ProjectId == request.PurchaseOrder.ProjectId
                        && c.Status == CustodyStatus.Confirmed)
                    .OrderByDescending(c => c.CreatedAt)
                    .FirstOrDefaultAsync();

                if (custody != null)
                    custody.RemainingAmount += request.PurchaseOrder.Amount;
            }

            // نطرح من SpentAmount
            var project = await _context.Projects.FindAsync(request.PurchaseOrder.ProjectId);
            if (project != null)
                project.SpentAmount -= request.PurchaseOrder.Amount;

            // نمسح الأوردر ونرجع الطلب Pending
            _context.PurchaseOrders.Remove(request.PurchaseOrder);
            request.Status = RequestStatus.Pending;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
