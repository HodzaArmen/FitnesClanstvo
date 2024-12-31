using System.ComponentModel.DataAnnotations;

namespace FitnesClanstvo.Models
{
    public class Clanstvo
    {
        public int Id { get; set; }
        [StringLength(50)]
        public string? Tip { get; set; }
        public decimal Cena { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Zacetek { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime Konec { get; set; }
        public int ClanId { get; set; }
        public Clan? Clan { get; set; }
    }
}