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
    [Authorize(Roles = "Admin")]
    public class OrderStatusController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        // GET: OrderStatus
        public ActionResult Index()
        {
            return View(db.OrderStatus.ToList());
        }

        // GET: OrderStatus/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = db.OrderStatus.Find(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // GET: OrderStatus/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: OrderStatus/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Status")] OrderStatu orderStatu)
        {
            if (ModelState.IsValid)
            {
                db.OrderStatus.Add(orderStatu);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orderStatu);
        }

        // GET: OrderStatus/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = db.OrderStatus.Find(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // POST: OrderStatus/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Status")] OrderStatu orderStatu)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderStatu).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderStatu);
        }

        // GET: OrderStatus/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            OrderStatu orderStatu = db.OrderStatus.Find(id);
            if (orderStatu == null)
            {
                return HttpNotFound();
            }
            return View(orderStatu);
        }

        // POST: OrderStatus/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            OrderStatu orderStatu = db.OrderStatus.Find(id);
            db.OrderStatus.Remove(orderStatu);
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
