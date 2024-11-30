using System;

namespace FitnesClanstvo.Models
{
    public enum Tip
    {
        Mesečna,
        Tromesečna,
        Letna
    }
    public class Clanstvo
    {
        private static readonly Dictionary<Tip, decimal> _cene = new()
        {
            { Models.Tip.Mesečna, 40 },
            { Models.Tip.Tromesečna, 100 },
            { Models.Tip.Letna, 360 }
        };

        public int Id { get; set; }
        public Tip? Tip { get; set; }
        public decimal Cena
        {
            get
            {
                if (Tip.HasValue)
                {
                    return _cene[Tip.Value];
                }
                else
                {
                    return 0; 
                }
            }
        }
        public DateTime Zacetek { get; set; }
        public DateTime Konec { get; set; }
        public int ClanId { get; set; }
        public Clan? Clan { get; set; }
    }
}