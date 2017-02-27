using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class Transaction
    {
        
        public virtual int ID { get; set; }

        public string Owner { get; set; }

        [Required]
        [Display(Name = "Transaction Date"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TransactionDate { get; set; }
        
        public long Amount { get; set; }
        [Required]
        public string Note { get; set; }
        [Display(Name = "Account ID")]
        public virtual int AccountID { get; set; }
        public virtual Account Account { get; set; }
        
        
    }
}