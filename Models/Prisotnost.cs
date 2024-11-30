using System;

namespace FitnesClanstvo.Models
{
    public class Prisotnost
    {
        public int Id { get; set; }
        public DateTime DatumPrisotnosti { get; set; }
        public int ClanId { get; set; }
        public Clan? Clan { get; set; }
        public int VadbaId { get; set; }
        public Vadba? Vadba { get; set; }
    }
}
