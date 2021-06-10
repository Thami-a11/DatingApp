using System;

namespace API.Helpers
{
    public class VisitsParams : PaginationParams
    {
        public int UserId { get; set; }

        public string Predicate { get; set; }

        //public string AllVisits { get; set; }
        //public DateTime pastMonthVisits { get; set; } = DateTime.Now.AddMonths(-1);
    }
}