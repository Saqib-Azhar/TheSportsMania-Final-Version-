using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportsMania.Models;
using Microsoft.AspNet.Identity;

namespace SportsMania.Controllers
{
    public class ProductReviewsController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        // GET: ProductReviews
        public ActionResult Index()
        {
            var productReviews = db.ProductReviews.Include(p => p.AspNetUser).Include(p => p.Product);
            return View(productReviews.ToList());
        }

        // GET: ProductReviews/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReviews.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            return View(productReview);
        }

        // GET: ProductReviews/Create
        public ActionResult Create()
        {
            ViewBag.VoterID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName");
            return View();
        }

        // POST: ProductReviews/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductID,Review,Rate,VoterID,Time")] ProductReview productReview)
        {
            if (ModelState.IsValid)
            {
                db.ProductReviews.Add(productReview);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.VoterID = new SelectList(db.AspNetUsers, "Id", "Email", productReview.VoterID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", productReview.ProductID);
            return View(productReview);
        }

        // GET: ProductReviews/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReviews.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            ViewBag.VoterID = new SelectList(db.AspNetUsers, "Id", "Email", productReview.VoterID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", productReview.ProductID);
            return View(productReview);
        }

        // POST: ProductReviews/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductID,Review,Rate,VoterID,Time")] ProductReview productReview)
        {
            if (ModelState.IsValid)
            {
                db.Entry(productReview).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.VoterID = new SelectList(db.AspNetUsers, "Id", "Email", productReview.VoterID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", productReview.ProductID);
            return View(productReview);
        }

        // GET: ProductReviews/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductReview productReview = db.ProductReviews.Find(id);
            if (productReview == null)
            {
                return HttpNotFound();
            }
            return View(productReview);
        }

        // POST: ProductReviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProductReview productReview = db.ProductReviews.Find(id);
            db.ProductReviews.Remove(productReview);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        /*********************************************************************************************************************************/

        

        public JsonResult AddEditReview(int prodID, int rating, string review)
        {
            var msg = "An Error Occured Please Try Again";
            var userID = this.User.Identity.GetUserId();
            var checkVoter = db.ProductReviews.FirstOrDefault(s => s.VoterID == userID);

            if (userID == null)
            {
                msg = "Please Login First to review this Item";
            }

            else if (checkVoter == null)
            {
                var prodObj = db.Products.FirstOrDefault(s => s.Id == prodID);
                var totalRating = prodObj.AvgRating * prodObj.TotalVotes;
                totalRating = totalRating + rating;
                prodObj.TotalVotes = prodObj.TotalVotes + 1;
                prodObj.AvgRating = totalRating / prodObj.TotalVotes;

                ProductReview ReviewObj = new ProductReview();
                ReviewObj.ProductID = prodID;
                ReviewObj.Rate = rating;
                ReviewObj.Review = review;
                ReviewObj.Time = DateTime.Now;
                ReviewObj.VoterID = userID;

                db.ProductReviews.Add(ReviewObj);
                db.SaveChanges();
                msg = "Review Added Successfuly";
            }

            else
            {
                var prodObj = db.Products.FirstOrDefault(s => s.Id == prodID);
                var totalRating = prodObj.AvgRating * prodObj.TotalVotes;
                totalRating = totalRating - checkVoter.Rate;
                totalRating = totalRating + rating;
                prodObj.AvgRating = totalRating / prodObj.TotalVotes;


                checkVoter.ProductID = prodID;
                checkVoter.Rate = rating;
                checkVoter.Review = review;
                checkVoter.Time = DateTime.Now;
                checkVoter.VoterID = userID;

                db.SaveChanges();
                msg = "Review Updated Successfully";
            }

            return Json(msg,JsonRequestBehavior.AllowGet);
        }


        public JsonResult ProductReviews(int ProdID)
        {
            //var ReviewList = db.ProductReviews.Where(s => s.ProductID == ProdID).OrderBy(s => s.Time).Include(s=>s.AspNetUser).ToList();
            
            var ReviewList = (from review in db.ProductReviews
                            where review.ProductID == ProdID
                            orderby review.Time
                            select new { review.Id, review.Rate, review.Review, review.Time, review.AspNetUser.UserName }).ToList();
            return Json(ReviewList, JsonRequestBehavior.AllowGet);
        }



        /*********************************************************************************************************************************/



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
