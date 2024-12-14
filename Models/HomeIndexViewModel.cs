using System.Collections.Generic;
using FitnesClanstvo.Models;

namespace FitnesClanstvo.ViewModels
{
    public class HomeIndexViewModel
    {
        public List<Vadba> Vadbe { get; set; } = new List<Vadba>();
        public int IzbraniClanId { get; set; } // Za prijavo člana
    }
}
