using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Tashyeed.Infrastructure.Entities;
using Tashyeed.Infrastructure.Persistence;
using Tashyeed.Shared.Enums;
using Tashyeed.Web.Modules.CustodyModule.ViewModels;

namespace Tashyeed.Web.Modules.CustodyModule.Services
{
    public class CustodyService : ICustodyService
    {
        private readonly AppDBContext _context;
        private readonly IMapper _mapper;

        public CustodyService(AppDBContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<CustodyListVM>> GetAllAsync()
        {
            var custodies = await _context.Custodies
                .Include(c => c.Project)
                .Include(c => c.GivenBy)
                .Include(c => c.GivenTo)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustodyListVM>>(custodies);
        }

        public async Task<IEnumerable<CustodyListVM>> GetByProjectAsync(int projectId)
        {
            var custodies = await _context.Custodies
                .Include(c => c.Project)
                .Include(c => c.GivenBy)
                .Include(c => c.GivenTo)
                .Where(c => c.ProjectId == projectId)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustodyListVM>>(custodies);
        }

        public async Task<IEnumerable<CustodyListVM>> GetMyPendingCustodiesAsync(string userId)
        {
            var custodies = await _context.Custodies
                .Include(c => c.Project)
                .Include(c => c.GivenBy)
                .Include(c => c.GivenTo)
                .Where(c => c.GivenToUserId == userId && c.Status == CustodyStatus.Pending)
                .ToListAsync();

            return _mapper.Map<IEnumerable<CustodyListVM>>(custodies);
        }

        public async Task AssignCustodyAsync(AssignCustodyVM vm, string givenByUserId)
        {
            var custody = _mapper.Map<Custody>(vm);
            custody.GivenByUserId = givenByUserId;
            custody.RemainingAmount = vm.Amount;
            custody.Status = CustodyStatus.Pending;

            _context.Custodies.Add(custody);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ConfirmCustodyAsync(int custodyId, string userId)
        {
            var custody = await _context.Custodies
                .FirstOrDefaultAsync(c => c.Id == custodyId && c.GivenToUserId == userId);

            if (custody is null || custody.Status != CustodyStatus.Pending)
                return false;

            custody.Status = CustodyStatus.Confirmed;
            custody.ConfirmedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RejectCustodyAsync(int custodyId, string userId)
        {
            var custody = await _context.Custodies
                .FirstOrDefaultAsync(c => c.Id == custodyId && c.GivenToUserId == userId);

            if (custody is null || custody.Status != CustodyStatus.Pending)
                return false;

            custody.Status = CustodyStatus.Rejected;
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
