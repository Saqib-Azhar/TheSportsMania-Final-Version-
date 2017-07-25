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
    public class SavedItemsController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        // GET: SavedItems
        public ActionResult Index()
        {
            var username = User.Identity.Name;
            var user = db.AspNetUsers.FirstOrDefault(s => s.UserName == username);

            var savedItems = db.SavedItems.Include(s => s.AspNetUser).Include(s => s.Product);
            return View(savedItems.Where(s => s.UserID == user.Id).ToList());
        }

        // GET: SavedItems/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedItem savedItem = db.SavedItems.Find(id);
            if (savedItem == null)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Details", "Products", new { id = savedItem.Product.Id });

        }

        // GET: SavedItems/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName");
            return View();
        }

        // POST: SavedItems/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserID,ProductID")] SavedItem savedItem)
        {
            if (ModelState.IsValid)
            {
                db.SavedItems.Add(savedItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", savedItem.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", savedItem.ProductID);
            return View(savedItem);
        }

        // GET: SavedItems/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedItem savedItem = db.SavedItems.Find(id);
            if (savedItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", savedItem.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", savedItem.ProductID);
            return View(savedItem);
        }

        // POST: SavedItems/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserID,ProductID")] SavedItem savedItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(savedItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", savedItem.UserID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", savedItem.ProductID);
            return View(savedItem);
        }

        // GET: SavedItems/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SavedItem savedItem = db.SavedItems.Find(id);
            if (savedItem == null)
            {
                return HttpNotFound();
            }
            return View(savedItem);
        }

        // POST: SavedItems/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SavedItem savedItem = db.SavedItems.Find(id);
            db.SavedItems.Remove(savedItem);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult SaveNewItem(int Id)
        {
            try
            {
                var username = User.Identity.Name;
                var user = db.AspNetUsers.FirstOrDefault(s => s.UserName == username);

                var NewSaveProd = new SavedItem();
                NewSaveProd.ProductID = Id;
                NewSaveProd.UserID = user.Id;

                db.SavedItems.Add(NewSaveProd);
                db.SaveChanges();
                var Message = "Product saved Successfuly..!";
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
            catch (Exception)
            {

                var Message = "Product couldn't saved..!";
                if (User.Identity.IsAuthenticated)
                {
                    Message = "Product couldn't Saved, Please Reload the Page & Try Again.., Sorry for Inconvinience..";
                }
                else
                {
                    Message = "Product couldn't saved..! Please Login or Sign Up..!";
                }
                return Json(Message, JsonRequestBehavior.AllowGet);
            }
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
