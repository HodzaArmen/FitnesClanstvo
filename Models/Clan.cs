using System.ComponentModel.DataAnnotations;

namespace FitnesClanstvo.Models
{
    public class Clan
    {
        public int Id { get; set; }
        [StringLength(50)]
        public required string Ime { get; set; }
        [StringLength(50)]
        public required string Priimek { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatumRojstva { get; set; }
        public string? Email { get; set; }
        public Clanstvo? Clanstvo { get; set; }
    }
}