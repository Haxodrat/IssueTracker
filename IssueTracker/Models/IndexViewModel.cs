using System;
namespace IssueTracker.Models
{
    public class IndexViewModel
    {
        public IndexViewModel()
        {
            this.UserTickets = new Dictionary<string, int>();
        }

        public int NoPriority { get; set; }

        public int LowPriority { get; set; }

        public int MediumPriority { get; set; }

        public int HighPriority { get; set; }

        public int UrgentPriority { get; set; }

        public int NoStatus { get; set; }

        public int OpenStatus { get; set; }

        public int InProgressStatus { get; set; }

        public int ResolvedStatus { get; set; }

        public int InfoStatus { get; set; }

        public int Bugs { get; set; }

        public int Features { get; set; }

        public int Other { get; set; }

        public int Styling { get; set; }

        public Dictionary<String, int> UserTickets { get; set; }
    }
}

