using System;
using FitnesClanstvo.Data;
using System.ComponentModel.DataAnnotations.Schema;


namespace FitnesClanstvo.Models
{
    public class Placilo
    {
        public int Id { get; set; }
        public DateTime DatumPlacila { get; set; }
        public decimal Znesek { get; set; }
        public int ClanstvoId { get; set; }
        public DateTime? DateCreated { get; set; }
        public DateTime? DateEdited { get; set; }
        public ApplicationUser? Owner { get; set; }
        public Clanstvo? Clanstvo { get; set; }
    }
}
