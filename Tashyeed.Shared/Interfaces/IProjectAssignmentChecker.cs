using System;
using System.Collections.Generic;
using System.Text;

namespace Tashyeed.Shared.Interfaces
{
    public interface IProjectAssignmentChecker
    {
        Task<bool> HasAssignmentsAsync(string userId);
    }   
}
