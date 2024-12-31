using System.Collections.Generic;
using FitnesClanstvo.Models;

namespace FitnesClanstvo.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Vadba> Vadbe { get; set; } = new List<Vadba>();
        public List<MonthlyStatistic> MonthlyStatistics { get; set; }
        public int IzbraniClanId { get; set; } // Za prijavo člana
    }
    public class MonthlyStatistic
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int MemberCount { get; set; }
    }
}
