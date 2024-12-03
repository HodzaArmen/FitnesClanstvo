namespace FitnesClanstvo.Models
{
    public class Clan
    {
        public int Id { get; set; }
        public required string Ime { get; set; }
        public required string Priimek { get; set; }
        public DateTime DatumRojstva { get; set; }
        public string? Email { get; set; }
        public Clanstvo? Clanstvo { get; set; }
    }
}