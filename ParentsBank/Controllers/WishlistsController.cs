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
    public class WishlistsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Wishlists
        public ActionResult Index()
        {
            var wishlists = db.Wishlists.Include(w => w.Account);
            return View(wishlists.ToList());
        }

        // GET: Wishlists/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }


            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            //CanPurchase(id, wishlist.Cost);
            return View(wishlist);
        }

        public ActionResult Search(string SearchCost, string SearchDescription)
        {
            bool searchPerformed = false;
            //int flag = 0;
            var wishlists = db.Wishlists.Where(w=>w.Description==SearchDescription);
            if(User.IsInRole("Child"))
            { 
                Account acc = db.Accounts.Single(a => a.Recipient == User.Identity.Name);
                  wishlists = db.Wishlists.Where(w => w.AccountId == acc.Id);
            }

            if(User.IsInRole("Parent"))
            {
                wishlists = db.Wishlists.Where(w => w.owner == User.Identity.Name);
            }
            List<Wishlist> FinalList = new List<Wishlist>();

            if (!string.IsNullOrWhiteSpace(SearchCost))
            {

                double cost = Convert.ToDouble(SearchCost);
                searchPerformed = true;
                foreach(Wishlist w in wishlists)
                {
                    if(w.Cost == cost)
                    {
                        FinalList.Add(w);
                        
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(SearchDescription))
            {
                searchPerformed = true;
                foreach(Wishlist w in wishlists)
                {
                    
                    if (w.Description == SearchDescription)
                    {
                        FinalList.Add(w);
                    }
                    
                   
                }
            }

            if (searchPerformed)
            {
               
                    return View(FinalList);
                
            }
            else
            {
                return View(new List<Wishlist>());
            }
          

        }
        
        // GET: Wishlists/Create

        public ActionResult Create()
        {
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner");
            return View();
        }

        // POST: Wishlists/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "WishListID,DateAdded,Cost,Description,Link,Purchase,AccountId")] Wishlist wishlist)
        {
            if (ModelState.IsValid && (User.IsInRole("Parent") || User.IsInRole("Child")))
            {
                Account acc = db.Accounts.Single(a => a.Id == wishlist.AccountId);
                if(acc.CompoundedAmount>wishlist.Cost)
                {
                    wishlist.purchasable = 1;
                }
                else
                {
                    wishlist.purchasable = 0;
                }
                wishlist.owner = acc.Owner;
                db.Wishlists.Add(wishlist);
                db.SaveChanges();
                if (wishlist.Purchase==true)
                {
                    updatebalance(wishlist.AccountId, wishlist.Cost);
                }
                
                return RedirectToAction("Index");
            }

            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishlist.AccountId);
            return View(wishlist);
        }

        public void updatebalance(int id, double cost)
        {
            Account acc = db.Accounts.Single(a => a.Id == id);
            acc.Balance = acc.Balance - cost;
            acc.CompoundedAmount = acc.CompoundedAmount - cost;
            db.SaveChanges();
        }



        //[Bind(Include = "ID")]
        // GET: Wishlists/Edit/5
        public static int flag = 0;
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            wishlist.purchasable = flag;
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishlist.AccountId);
            return View(wishlist);
        }

        // POST: Wishlists/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,DateAdded,Cost,Description,Link,Purchase,AccountId")] Wishlist wishlist)
        {
            if (ModelState.IsValid)
            {
                Account acc = db.Accounts.Single(a => a.Id == wishlist.AccountId);
                db.Entry(wishlist).State = EntityState.Modified;
                wishlist.owner = acc.Owner;
                flag = wishlist.purchasable;
                db.SaveChanges();
                if (wishlist.Purchase == true)
                {
                    updatebalance(wishlist.AccountId, wishlist.Cost);
                }
                return RedirectToAction("Index");
            }
            ViewBag.AccountId = new SelectList(db.Accounts, "Id", "Owner", wishlist.AccountId);
            return View(wishlist);
        }

        // GET: Wishlists/Delete/5
        [Authorize(Roles = "Parent")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Wishlist wishlist = db.Wishlists.Find(id);
            if (wishlist == null)
            {
                return HttpNotFound();
            }
            return View(wishlist);
        }

        // POST: Wishlists/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Parent")]
        public ActionResult DeleteConfirmed(int id)
        {
            Wishlist wishlist = db.Wishlists.Find(id);
            db.Wishlists.Remove(wishlist);
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
