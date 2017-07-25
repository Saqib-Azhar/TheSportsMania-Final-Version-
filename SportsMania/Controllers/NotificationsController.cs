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
    public class NotificationsController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        [Authorize]
        // GET: Notifications
        public ActionResult Index()
        {
            if (User.IsInRole("User"))
            {
                var Usernotifications = db.Notifications.Include(n => n.AspNetUser).Where(s => s.Show == true);
                return View(Usernotifications.ToList());
            }
            var notifications = db.Notifications.Include(n => n.AspNetUser);
            return View(notifications.ToList());
        }

        // GET: Notifications/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        // GET: Notifications/Create
        public ActionResult Create()
        {
            ViewBag.AdminID = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        [Authorize(Roles = "Admin")]
        // POST: Notifications/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Message,CreationDate,ValidDate,Show")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                notification.AdminID = User.Identity.GetUserId();
                db.Notifications.Add(notification);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.AdminID = new SelectList(db.AspNetUsers, "Id", "Email", notification.AdminID);
            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        // GET: Notifications/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            ViewBag.AdminID = new SelectList(db.AspNetUsers, "Id", "Email", notification.AdminID);
            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        // POST: Notifications/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Message,CreationDate,ValidDate,Show")] Notification notification)
        {
            if (ModelState.IsValid)
            {
                db.Entry(notification).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.AdminID = new SelectList(db.AspNetUsers, "Id", "Email", notification.AdminID);
            return View(notification);
        }

        [Authorize(Roles = "Admin")]
        // GET: Notifications/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Notification notification = db.Notifications.Find(id);
            if (notification == null)
            {
                return HttpNotFound();
            }
            return View(notification);
        }

        public JsonResult GetNotifications()
        {
            //List<Notification> notificationsList = new List<Notification>();
            var notifications = db.Notifications.Where(s => s.Show == true).ToList();

            foreach (var item in notifications)
            {
                if (item.ValidDate < DateTime.Now)
                {
                    item.Show = false;
                    db.SaveChanges();
                }
            }
            var notificationsList = (from notification in db.Notifications
                                     where notification.Show == true
                                     select new { notification.Id, notification.Message, notification.ValidDate, notification.CreationDate }).ToList();
            return Json(notificationsList, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Admin")]
        // POST: Notifications/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Notification notification = db.Notifications.Find(id);
            db.Notifications.Remove(notification);
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
