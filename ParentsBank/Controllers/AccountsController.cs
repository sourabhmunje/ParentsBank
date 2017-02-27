using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ParentsBank.Models;

namespace ParentsBank.Controllers
{
    [Authorize]
    public class AccountsController : Controller
    {

        private ApplicationDbContext db = new ApplicationDbContext();
        public static double balanceAmt = 0;
        // GET: Accounts
        public ActionResult Index()
        {
            var account = db.Accounts.Where(c => c.Username == User.Identity.Name || c.Recipient == User.Identity.Name);
            return View(account.ToList());
        }

        // GET: Accounts/Details/5
        public ActionResult Details(int? id)
        {
           // balanceWithInterest();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Account account = db.Accounts.FirstOrDefault((c => c.Id == id ));
            
            if (account != null)
            {
                account.yearDateInterestCalculation();
            }
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        // GET: Accounts/Create
        public ActionResult Create()
        {
            return View();
        }
        
    // POST: Accounts/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Parent")]
        public ActionResult Create([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,Interest")] Account account)
        {

            if (ModelState.IsValid)
            { 
                if (account.Recipient == account.Owner)
            {
                ModelState.AddModelError("Recipient", "EmailID is the same");
            }

            int emailInstances = db.Accounts
                                            .Where(p => p.Recipient == account.Recipient)
                                            .Count();
            emailInstances += db.Accounts
                                         .Where(p => p.Recipient == account.Owner)
                                         .Count();

            emailInstances += db.Accounts
                                         .Where(p => p.Owner == account.Recipient)
                                         .Count();


            if (emailInstances > 0)
            {
                ModelState.AddModelError("Recipient", "Email id already exist");
            }

            if (ModelState.IsValid)
            {
                if (User.IsInRole("Parent"))
                {
                    account.Username = User.Identity.Name;
                    db.Accounts.Add(account);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
            }
            }
            return View(account);
        }

        // GET: Accounts/Edit/5
        [Authorize(Roles = "Parent")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.FirstOrDefault((c => c.Id == id && c.Username == User.Identity.Name));
            if (account == null)
            {
                return HttpNotFound();
            }
            balanceAmt = account.Balance;
            return View(account);
        }

        // POST: Accounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Parent")]
        public ActionResult Edit([Bind(Include = "Id,Owner,Recipient,Name,OpenDate,Interest")] Account account)
        {
            if (ModelState.IsValid && User.IsInRole("Parent"))
            {
                account.Username = User.Identity.Name;
                account.Balance = balanceAmt;
                db.Entry(account).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(account);
        }

        // GET: Accounts/Delete/5
        [Authorize(Roles = "Parent")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Account account = db.Accounts.FirstOrDefault((c => c.Id == id && c.Username == User.Identity.Name));
            if (account == null)
            {
                return HttpNotFound();
            }
            return View(account);
        }

        public PartialViewResult PartialViewTransaction(int? id)
        {
            //var accounts = db.Accounts.Single(c => c.Id == id);
            var transaction = db.Transactions.Where(c => c.AccountID == id);

            return PartialView(transaction);
        }

        public PartialViewResult PartialViewWishlist(int? id)
        {
            //var accounts = db.Accounts.Single(c => c.Id == id);
            var wishlist = db.Wishlists.Where(c => c.AccountId == id);
           
            return PartialView(wishlist);
        }


        // POST: Accounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Parent")]
        public ActionResult DeleteConfirmed(int id)
        {
            Account account = db.Accounts.FirstOrDefault((c => c.Id == id && c.Username == User.Identity.Name));
            if (ModelState.IsValid)
            {
                if (account.Balance != 0)
                {
                    ModelState.AddModelError("Delete", "Balance is Non-Zero");
                }

                if (ModelState.IsValid)
                {
                    db.Accounts.Remove(account);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return View(account);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

    }
}
