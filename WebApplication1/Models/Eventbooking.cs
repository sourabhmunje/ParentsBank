using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace WebApplication1.Models
{
    public class Eventbooking
    {
        public int ID { get; set; }
        [DisplayName("Number of guests")]
        public int countOfBooking { get; set; }
        [DisplayName("Name of the Event")]
        public virtual int EventID { get; set; }
        public virtual Event events { get; set; }
        [DisplayName("Name")]
        public virtual int CustomerID { get; set; }
        public virtual customer cust { get; set; }
    }
}