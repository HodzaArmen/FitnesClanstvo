using System;

namespace FitnesClanstvo.Models
{
    public class Vadba
    {
        public int Id { get; set; }
        public string? Ime { get; set; }
        public DateTime DatumInUra { get; set; }
        public int Kapaciteta { get; set; }
    }
}
