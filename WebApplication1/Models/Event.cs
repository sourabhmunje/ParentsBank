using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Event
    {
        public int ID { get; set; }
        [DisplayName("Name of the Event")]
        public string EventName { get; set; }
        public string Owner { get; set; }

        public string Street { get; set; }
        public string Apartment { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zip { get; set; }
        public double LocationLat { get; set; }
        public double LocationLon { get; set; }
        [DisplayName("Cost per Person")]
        public int RatePerPerson { get; set; }
        [DisplayName("Cusine")]
        public string cusine { get; set; }
        [DisplayName("Time of the Event")]
        public DateTime EventTime { get; set; }
        [DisplayName("Hosting Capacity")]
        public int Capacity { get; set; }
        [DisplayName("Number of Seats Booked")]
        public int SeatsBooked { get; set; }
        [DisplayName("Number of Seats Left")]
        public int SeatsLeft { get; set; }

        public virtual List<Eventbooking> EventBooking { get; set; }
        
    }
}