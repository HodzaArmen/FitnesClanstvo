namespace FitnesClanstvo.Models
{
    public class Clanstvo
    {
        public int Id { get; set; }
        public string? Tip { get; set; }
        public decimal Cena { get; set; }
        public DateTime Zacetek { get; set; }
        public DateTime Konec { get; set; }
        public int ClanId { get; set; }
        public Clan? Clan { get; set; }
    }
}