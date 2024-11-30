using System;

namespace FitnesClanstvo.Models
{
    public enum Ime
    {
        Joga,
        Kardio,
        Pilates,
        Fitnes
    }
    public class Vadba
    {
        public int Id { get; set; }
        public required Ime Ime { get; set; }
        public DateTime DatumInUra { get; set; }
        public int Kapaciteta { get; set; }
    }
}
