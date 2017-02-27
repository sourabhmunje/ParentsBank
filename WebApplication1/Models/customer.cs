using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class customer
    {
        [DisplayName("ID")]
        public int ID { get; set; }

        [DisplayName("User Name")]
        public string Username { get; set; }

        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Phone Number")]
        public long PhoneNumber { get; set; }

        
        [DisplayName("Credit Card Number")]
        public long CreditCardNumber { get; set; }
        public long Balance { get; set; }
        public int CVV { get; set; }

        [DisplayName("Expiration Date")]
        public DateTime ExpirationDate { get; set; }

        public string Street { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public double LocationLat { get; set; }
        public double LocationLon { get; set; }

        public virtual List<Eventbooking> EventBooking { get; set; }
    }
}