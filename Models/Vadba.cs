using System.ComponentModel.DataAnnotations;

namespace FitnesClanstvo.Models
{
    public class Vadba
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string? Ime { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime DatumInUra { get; set; }
        public int Kapaciteta { get; set; }
        public ICollection<Rezervacija>? Rezervacije { get; set; }
    }
}
