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
    public class CartsController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        [Authorize]
        // GET: Carts
        public ActionResult Index()
        {
            var user = User.Identity.Name;
            var UserId = db.AspNetUsers.FirstOrDefault(s => s.UserName == user);

            var carts = db.Carts.Include(c => c.AspNetUser).Include(c => c.Product);
            return View(carts.Where(s => s.UserID == UserId.Id).ToList());
        }

        [Authorize]
        // GET: Carts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        [Authorize]
        // GET: Carts/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName");
            return View();
        }

        [Authorize]
        // POST: Carts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserID,ProductID,ProductCount")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", cart.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", cart.ProductID);
            return View(cart);
        }

        [Authorize]
        // GET: Carts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", cart.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", cart.ProductID);
            return View(cart);
        }

        [Authorize]
        // POST: Carts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserID,ProductID,ProductCount")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", cart.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", cart.ProductID);
            return View(cart);
        }

        [Authorize]
        // GET: Carts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        [Authorize]
        // POST: Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            db.Carts.Remove(cart);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //

        public JsonResult AddToCart(int Id, int ProductCount)
        {
            var msg = "";
            var product = db.Products.FirstOrDefault(s => s.Id == Id);
            var TransactionObj = db.Database.BeginTransaction();
            try
            {
                var user = User.Identity;
                var userId = db.AspNetUsers.FirstOrDefault(s => s.UserName == user.Name);
                Cart ProductCheck = null;
                ProductCheck = db.Carts.FirstOrDefault(s => s.ProductID == Id && s.UserID == userId.Id);
                if (ProductCheck != null)
                {
                    ProductCheck.ProductCount = ProductCheck.ProductCount + ProductCount;
                    ProductCheck.TotalProductPrice = product.Price * ProductCheck.ProductCount;
                    db.SaveChanges();
                }
                else
                {

                    var CartObj = new Cart();
                    CartObj.ProductID = Id;
                    CartObj.ProductCount = ProductCount;
                    CartObj.UserID = userId.Id;
                    CartObj.TotalProductPrice = product.Price * ProductCount;
                    db.Carts.Add(CartObj);
                    db.SaveChanges();
                }


                msg = "Product successfuly Added to Cart";
                TransactionObj.Commit();
            }
            catch (Exception)
            {
                TransactionObj.Dispose();
                if (User.Identity.IsAuthenticated)
                {
                    msg = "Product couldn't Added to Cart, Please Reload the Page & Try Again.., Sorry for Inconvinience..";
                }
                else
                {
                    msg = "Please Login or Sign Up for shopping..!";
                }
            }

            return Json(msg, JsonRequestBehavior.AllowGet);
        }


        public JsonResult CartPrice(int val)
        {
            decimal? totalPrice = 0;
            var userID = User.Identity;
            var userObj = db.AspNetUsers.FirstOrDefault(s => s.UserName == userID.Name);
            if (userObj != null)
            {

                var BoughtList = db.Carts.Where(s => s.UserID == userObj.Id).ToList();

                foreach (var item in BoughtList)
                {
                    totalPrice = totalPrice + item.TotalProductPrice;
                }

            }


            double SumPrice = Convert.ToDouble(totalPrice);
            return Json(SumPrice, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ItemsCountInCart(int val)
        {
            decimal? totalItems = 0;
            var userObj = User.Identity.GetUserId();
            if (userObj != null)
            {

                var BoughtList = db.Carts.Where(s => s.UserID == userObj).ToList();

                foreach (var item in BoughtList)
                {
                    totalItems = totalItems + 1;
                }

            }

            return Json(totalItems, JsonRequestBehavior.AllowGet);
        }
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
        // //////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
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
