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
    public class TransactionsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Transactions
        public ActionResult Index()
        {

            var transactions = db.Transactions.Include(t => t.Account);
            return View(transactions.ToList());
        }

        // GET: Transactions/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // GET: Transactions/Create
        [Authorize(Roles ="Parent")]
        public ActionResult Create()
        {
            ViewBag.AccountID = new SelectList(db.Accounts, "Id", "Recipient");
            return View();
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Parent")]
        public ActionResult Create([Bind(Include = "ID,TransactionDate,Amount,Note,AccountID, Account")] Transaction transaction)
        {
            Account existingAccount = db.Accounts.FirstOrDefault(c => c.Username == User.Identity.Name);

            if (ModelState.IsValid)
            {
                if(transaction.Amount == 0)
                {
                    ModelState.AddModelError("Amount", "Amount can't be zero.");
                }

                if (transaction.Amount < 0)
                {
                    if (existingAccount.Balance < transaction.Amount)
                    {
                        ModelState.AddModelError("Amount", "Amount value exceeds balance.");
                    }

                }

                if (transaction.TransactionDate > DateTime.Now)
                {
                    ModelState.AddModelError("TransactionDate", "Transaction Date in future.");
                }

                if (transaction.TransactionDate.Year < DateTime.Now.Year)
                {
                    ModelState.AddModelError("TransactionDate", "Transaction Date in last year.");
                }
                
                //if(transaction.Amount> db.Accounts)
                if (ModelState.IsValid)
                {
                    //transaction.TransactionDate = DateTime.Now;
                    transaction.Owner = User.Identity.Name;
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    addTransactionAmount(transaction.AccountID, transaction.Amount);
                    updatewish(transaction.AccountID);
                    return RedirectToAction("Index");
                }
            }

            ViewBag.AccountID = new SelectList(db.Accounts, "Id", "Owner", transaction.AccountID);
            return View(transaction);
        }

        public void updatewish(int id)
        {
            Account acc = db.Accounts.Single(a => a.Id == id);
            var wishlist = db.Wishlists.Where(w => w.AccountId == id);
            foreach(Wishlist wi in wishlist)
            {
                if(acc.Balance>wi.Cost)
                {
                    wi.purchasable = 1;
                }
                else
                {
                    wi.purchasable = 0;
                }
                
            }
            db.SaveChanges();
        }

        public void addTransactionAmount(int id, double amount)
        {
            Account acc = db.Accounts.Single(a => a.Id == id);
            acc.Balance = acc.Balance + amount;
            db.SaveChanges();
        }

        
        // GET: Transactions/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);
            if (transaction == null)
            {
                return HttpNotFound();
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "Id", "Owner", transaction.AccountID);
           
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,TransactionDate,Amount,Note,AccountID")] Transaction transaction)
        {
            if (ModelState.IsValid)
            {
                db.Entry(transaction).State = EntityState.Modified;
                transaction.Owner = User.Identity.Name;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AccountID = new SelectList(db.Accounts, "Id", "Owner", transaction.AccountID);
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Transaction transaction = db.Transactions.Find(id);

            if (transaction == null)
            {
                return HttpNotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Transaction transaction = db.Transactions.Find(id);
            db.Transactions.Remove(transaction);
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
    }
}
