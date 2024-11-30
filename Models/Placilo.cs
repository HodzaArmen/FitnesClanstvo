using System;
using FitnesClanstvo.Data;
using System.ComponentModel.DataAnnotations.Schema;


namespace FitnesClanstvo.Models
{
    public class Placilo
    {
        public int Id { get; set; }
        public DateTime DatumPlacila { get; set; }
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Znesek { get; set; }
        public int ClanstvoId { get; set; }
        public Clanstvo? Clanstvo { get; set; }
    }
}
