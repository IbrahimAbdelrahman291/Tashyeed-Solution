using System;
using System.Collections.Generic;
using System.Text;
using Tashyeed.Infrastructure.Identity;

namespace Tashyeed.Infrastructure.Entities
{
    public class ProjectAssignment
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Project Project { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
    }
}
