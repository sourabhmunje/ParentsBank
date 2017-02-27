using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using unirest_net.http;
using WebApplication1.Models;
using System.Net.Http;
using System.Net.Http.Headers;
using WebApplication1.EmailerView;
using System.Threading.Tasks;
using System.Web.Mail;
using System.Text;
using WebApplication1.EmailerView;

namespace WebApplication1.Controllers
{
    public class EventbookingsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Eventbookings
        public ActionResult Index()
        {
            var eventbookings = db.Eventbookings.Include(e => e.cust).Include(e => e.events);
            return View(eventbookings.ToList());
        }

        // GET: Eventbookings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventbooking eventbooking = db.Eventbookings.Find(id);
            if (eventbooking == null)
            {
                return HttpNotFound();
            }
            return View(eventbooking);
        }

        // GET: Eventbookings/Create
        public ActionResult Create(int? id)
        {
            // Eventbooking eventbooking = db.Eventbookings.Find(id);
            var eventz = db.Events.Where(ez => ez.ID == id);
            var cust = db.customers.Where(cu => cu.Username == User.Identity.Name);
            ViewBag.CustomerID = new SelectList(cust, "ID", "FirstName");
            ViewBag.EventID = new SelectList(eventz, "ID", "EventName");
            return View();
        }
        public async Task sendEmail(string emailId, string details)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://emailweb.azurewebsites.net/");
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                Emailer emailer = new Emailer();
                emailer.customerEmail = emailId;
                emailer.bookingDetails = details;
                // HttpResponseMessage response = await client.PostAsJsonAsync("api/Mail", emailer).ConfigureAwait(false);
                //if (response.IsSuccessStatusCode)
                {

                }

            }

        }

        [HttpPost]
        public ActionResult Create([Bind(Include = "ID,EventID,CustomerID,countOfBooking")] Eventbooking eventbooking)
        {

            var eventone = db.Events.Find(eventbooking.EventID);
            var customerone = db.customers.Find(eventbooking.CustomerID);
            {
                if (ModelState.IsValid && ((eventone.SeatsLeft - eventbooking.countOfBooking) >= 0))
                {
                    long costForCustomer=eventbooking.countOfBooking * eventone.RatePerPerson;
                    long costToBePaid = (long)(costForCustomer);
                    customerone.Balance = customerone.Balance - costToBePaid; 
                    eventone.SeatsBooked += eventbooking.countOfBooking;
                    eventone.SeatsLeft -= eventbooking.countOfBooking;
                    db.Eventbookings.Add(eventbooking);
                    db.SaveChanges();
                    sendMail(customerone.Username, "Hi,\n" + "\nAn order has been placed for you." + "\nThe order contains " + eventbooking.countOfBooking + " slots.\n\n" + "Thanks,\n" + "Taste of Home", "Your booking for event" + eventbooking.events.EventName);
                    return RedirectToAction("Index");
                }
                else
                {
                    
                    ModelState.AddModelError("countOfBooking", "Count more than seats left");
                }
            }
            

            ViewBag.CustomerID = new SelectList(db.customers, "ID", "FirstName", eventbooking.CustomerID);
            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventbooking.EventID);
            //

            // sendEmail("sourabhmunje@gmail.com","details -----");

            //
            return View(eventbooking);
        }

        // GET: Eventbookings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var eventbooking = db.Eventbookings.Single(ez => ez.ID == id);
            // foreach (var e in eventz)
            var eventz = db.Events.Where(ez => ez.ID == eventbooking.EventID);
            var cust = db.customers.Where(cu => cu.Username == User.Identity.Name);
            ViewBag.CustomerID = new SelectList(cust, "ID", "FirstName");
            ViewBag.EventID = new SelectList(eventz, "ID", "EventName");

            return View(eventbooking);
        }

        // POST: Eventbookings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,EventID,CustomerID,countOfBooking")] Eventbooking eventbooking)
        {
            if (ModelState.IsValid)
            {
                db.Entry(eventbooking).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerID = new SelectList(db.customers, "ID", "FirstName", eventbooking.CustomerID);
            ViewBag.EventID = new SelectList(db.Events, "ID", "EventName", eventbooking.EventID);
            return View(eventbooking);
        }

        // GET: Eventbookings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Eventbooking eventbooking = db.Eventbookings.Find(id);
            if (eventbooking == null)
            {
                return HttpNotFound();
            }
            return View(eventbooking);
        }

        // POST: Eventbookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Eventbooking eventbooking = db.Eventbookings.Find(id);
            var customerone = db.customers.Find(eventbooking.CustomerID);
            var eventone = db.Events.Find(eventbooking.EventID);
            eventone.SeatsLeft = eventone.SeatsLeft + eventbooking.countOfBooking;
            var balToIncrease =(long) (eventbooking.countOfBooking * eventone.RatePerPerson * (0.9));
            customerone.Balance =customerone.Balance + balToIncrease;
            db.Eventbookings.Remove(eventbooking);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public void sendMail(string toEmailId, string message, string subject)
        {
            {
                //string toAddress, string fromAddress,
                //  string subject, string messageBody
                string returnMessage = "SendEmail - blah";
                try
                {
                    string server = "smtp.gmail.com";
                    int port = 465;
                    string username = "tasteofhome.noreply@gmail.com";
                    string password = "tasteofhome123";
                    MailMessage mailMsg = new MailMessage();

                    mailMsg.To = toEmailId;
                    mailMsg.Headers.Add("From", string.Format("{0} <{1}>", "System", "tasteofhome.noreply@gmail.com"));
                    mailMsg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserver"] = server;
                    mailMsg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpserverport"] = port;
                    mailMsg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusing"] = 2;
                    mailMsg.Fields["http://schemas.microsoft.com/cdo/configuration/smtpauthenticate"] = 1;
                    mailMsg.Fields["http://schemas.microsoft.com/cdo/configuration/sendusername"] = username;
                    mailMsg.Fields["http://schemas.microsoft.com/cdo/configuration/sendpassword"] = password;
                    mailMsg.Fields.Add("http://schemas.microsoft.com/cdo/configuration/smtpusessl", "true");

                    mailMsg.BodyEncoding = Encoding.UTF8;
                    mailMsg.Subject = subject;
                    mailMsg.Body = message;

                    SmtpMail.SmtpServer = server;
                    SmtpMail.Send(mailMsg);

                }
                catch
                {
                    returnMessage = "An error occured. Could not send email.";
                }

            }
        }


    }
}
