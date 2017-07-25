using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SportsMania.Models;

namespace SportsMania.Controllers
{
    public class ProductsController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();
        [Authorize(Roles ="Admin")]
        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Brand).Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        // GET: Products/Create
        public ActionResult Create()
        {
            ViewBag.BrandID = new SelectList(db.Brands, "Id", "BrandName");
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "CategoryName");
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product product, HttpPostedFileBase Images)
        {
            if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    product.Image = new byte[Images.ContentLength];
                    Images.InputStream.Read(product.Image, 0, Images.ContentLength);
                }
                product.AvgRating = 0;
                product.TotalVotes = 0;
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.BrandID = new SelectList(db.Brands, "Id", "BrandName", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "CategoryName", product.CategoryID);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        // GET: Products/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.BrandID = new SelectList(db.Brands, "Id", "BrandName", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "CategoryName", product.CategoryID);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product product, HttpPostedFileBase Images)
        {
            if (ModelState.IsValid)
            {
                if (Images != null)
                {
                    product.Image = new byte[Images.ContentLength];
                    Images.InputStream.Read(product.Image, 0, Images.ContentLength);
                }
                var productObj = db.Products.FirstOrDefault(s => s.Id == product.Id);
                productObj.Image = product.Image;
                productObj.ProductName = product.ProductName;
                productObj.Price = product.Price;
                productObj.BrandID = product.BrandID;
                productObj.CategoryID = product.CategoryID;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BrandID = new SelectList(db.Brands, "Id", "BrandName", product.BrandID);
            ViewBag.CategoryID = new SelectList(db.Categories, "Id", "CategoryName", product.CategoryID);
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        // GET: Products/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        [Authorize(Roles = "Admin")]
        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            db.Products.Remove(product);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
        public ActionResult ProductsByBrand(int Id)
        {
            var products = db.Products.Include(p => p.Brand).Include(p => p.Category);
            return View(products.Where(x => x.BrandID == Id).ToList());
        }

        public ActionResult ProductsByCategories(int Id)
        {
            var products = db.Products.Include(p => p.Brand).Include(p => p.Category);
            return View(products.Where(x => x.CategoryID == Id).ToList());
        }
        public ActionResult SelectImage()
        {
            return View(db.Products.ToList());
        }
        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
        // ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////// //
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
