using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace ParentsBank.Models
{
    public class Account
    {
        public Account()
        {
            var TodayDate = DateTime.Now;
            OpenDate = TodayDate;
            Transaction = new List<Transaction>();
            Wishlist = new List<Wishlist>();
        }
        
        public int Id { get; set; }

        [Required, EmailAddress, DisplayName("Owner Account")]
        public string Owner { get; set; }

        [Required,EmailAddress, DisplayName("Recipient Account")] 
        public string Recipient { get; set; }

        [Required]
        public string Name { get; set; }

        [Required, DisplayName("Account opening date"), DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime OpenDate { get; set; }

        [Range(0,100)]
        [Required, DisplayName("Rate of Interest")]
        public double Interest { get; set; }

        [Required, DisplayName("Account Balance")]
        public double Balance { get; set; }

        public double? InterestGenerated { get; set; }
        public double CompoundedAmount { get; set; }
        public string Username { get; set; }
        public DateTime? InterestAddedDate { get; set; }
        public virtual List<Transaction> Transaction { get; set; }
        
        public virtual List<Wishlist> Wishlist { get; set; }

        public void yearDateInterestCalculation()
        {
            int CurrentDay = DateTime.Now.DayOfYear;
            double amount = Balance;
            int TimePeriod = 365;
            ApplicationDbContext db = new ApplicationDbContext();
            Account account = db.Accounts.Find(this.Id);
            Transaction t = new Models.Transaction();
            DateTime x = t.TransactionDate;
           
            int AddedDate = this.OpenDate.DayOfYear;
            int duration = CurrentDay - AddedDate;

            List<int> dates = new List<int>();
            List<double> amounts = new List<double>();

            if (this.Transaction.Count > 0)
            {
                foreach (Transaction transaction in Transaction)
                {
                    dates.Add(transaction.TransactionDate.DayOfYear);
                    amounts.Add(transaction.Amount);
                }
            }

            for (int day = 0; day < duration; day++)
            {

                double TodaysAmount = 0.0;

                if (dates.Count != 0 && dates.Contains(day))
                {
                    TodaysAmount = amounts.ElementAt(day);
                }
                amount = amount + TodaysAmount;
                amount = amount * Math.Pow((1 + this.Interest / (TimePeriod * 100)), 1);
            }
            account.InterestAddedDate = DateTime.Now;
            account.InterestGenerated = amount - Balance;
            account.CompoundedAmount = amount;
            db.Entry(account).State = EntityState.Modified;
            db.SaveChanges();
        }

        public double PrincipalPercentage()
        {
            double BalancePercentage = (Balance / CompoundedAmount) * 100;
            return BalancePercentage;
        }

        public double InterestPercentage()
        {
            double InterestGenerated = CompoundedAmount - Balance;
            double InterestPercentage = (InterestGenerated / CompoundedAmount) * 100;
            return InterestPercentage;
        }
    }
}