using System;

namespace API.Entities {
    public class UserVisits {
        public AppUser SourceUser { get; set; }
        public int SourceUserId { get; set; }
        public AppUser VisitedUser { get; set; }
        public int VisitedUserId { get; set; }
        public DateTime Created { get; set; } = DateTime.UtcNow;

    }
}