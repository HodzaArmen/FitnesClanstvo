using System.ComponentModel.DataAnnotations;

namespace FitnesClanstvo.Models
{
    public class Vadba
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string? Ime { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatumInUra { get; set; }
        public int Kapaciteta { get; set; }
    }
}
