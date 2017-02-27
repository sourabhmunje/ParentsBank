using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class Wishlist
    {
        public int ID { get; set; }

        [Required]
        [DisplayName("Date of Adding Wishlist"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime DateAdded { get; set; }

        [Required]
        public float Cost { get; set; }

        [Required]
        public string Description { get; set; }

        [Url]
        [Required]
        public string Link { get; set; }

        [Required]
        public bool Purchase { get; set; }

        public string owner { get; set; }

        public int purchasable { get; set; }

        //[DisplayName("Balance After Purchase(Cash Required)")]
        //public double CanAfford { get; set; }

        [Display(Name = "Account ID")]
        public virtual int AccountId { get; set; }

        public virtual Account Account { get; set; }
    }
}