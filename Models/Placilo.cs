using System.ComponentModel.DataAnnotations;

namespace FitnesClanstvo.Models
{
    public class Placilo
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatumPlacila { get; set; }
        public decimal Znesek { get; set; }
        public int ClanstvoId { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateCreated { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime? DateEdited { get; set; }
        public ApplicationUser? Owner { get; set; }
        public Clanstvo? Clanstvo { get; set; }
    }
}
