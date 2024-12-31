using System.ComponentModel.DataAnnotations;

namespace FitnesClanstvo.Models
{
    public class Rezervacija
    {
        public int Id { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
        public DateTime DatumRezervacije { get; set; }
        public int ClanId { get; set; }
        public Clan? Clan { get; set; }
        public int VadbaId { get; set; }
        public Vadba? Vadba { get; set; }
    }
}
