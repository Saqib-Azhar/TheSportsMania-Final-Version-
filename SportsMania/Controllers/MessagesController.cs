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
    public class MessagesController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        // GET: Messages
        public ActionResult Index()
        {
            if (User.IsInRole("Admin"))
            {
                var messages = db.Messages.Include(m => m.AspNetUser).Where(m => m.RecieverID == null);
                return View(messages.ToList());
            }
            else
            {
                var userid = User.Identity.GetUserId();
                var messages = db.Messages.Include(m => m.AspNetUser).Where(s => s.RecieverID == userid);
                return View(messages.ToList());
            }
        }

        public JsonResult GetMessages()
        {
            try
            {
                if (User.IsInRole("Admin"))
                {
                    var UserNameLog = User.Identity.Name;
                    AspNetUser currentUser = db.AspNetUsers.First(x => x.UserName == UserNameLog);
                    var MessagesList = (from message in db.Messages
                                        where message.RecieverID == null && message.Seen == false
                                        select new { message.Id, message.Message1, message.CreationTime, message.Name }).ToList();

                    return Json(MessagesList, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var UserNameLog = User.Identity.Name;
                    AspNetUser currentUser = db.AspNetUsers.First(x => x.UserName == UserNameLog);
                    var MessagesList = (from message in db.Messages
                                        where message.RecieverID == currentUser.Id && message.Seen == false
                                        select new { message.Id, message.Message1, message.CreationTime, message.Name }).ToList();

                    return Json(MessagesList, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception) { return null; }

        }

        // GET: Messages/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            message.Seen = true;
            message.SeenTime = DateTime.Now;
            db.SaveChanges();
            return View(message);
        }

        // GET: Messages/Create
        public ActionResult Create()
        {
            ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "Email");
            return View();
        }

        // POST: Messages/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Email,Contact,Message1,RecieverID")] Message message)
        {
            if (ModelState.IsValid)
            {
                if (User.Identity.IsAuthenticated)
                {
                    var userid = User.Identity.GetUserId();
                    var userObj = db.AspNetUsers.FirstOrDefault(s => s.Id == userid);
                    message.Email = userObj.Email;
                }
                message.Seen = false;
                message.CreationTime = DateTime.Now;
                db.Messages.Add(message);
                db.SaveChanges();
                if (User.Identity.IsAuthenticated)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    return RedirectToAction("Contact", "Home");
                }
            }

            ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "Email", message.RecieverID);
            return View(message);
        }

        // GET: Messages/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "Email", message.RecieverID);
            return View(message);
        }

        // POST: Messages/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Email,Contact,Message1,CreationTime,RecieverID")] Message message)
        {
            if (ModelState.IsValid)
            {
                db.Entry(message).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.RecieverID = new SelectList(db.AspNetUsers, "Id", "Email", message.RecieverID);
            return View(message);
        }

        // GET: Messages/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message message = db.Messages.Find(id);
            if (message == null)
            {
                return HttpNotFound();
            }
            return View(message);
        }


        public ActionResult SentIndex()
        {
            var userid = User.Identity.GetUserId();
            var userobj = db.AspNetUsers.FirstOrDefault(s => s.Id == userid);
            var messages = db.Messages.Include(m => m.AspNetUser).Where(m => m.Email == userobj.Email);
            return View(messages.ToList());
        }

        // POST: Messages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Message message = db.Messages.Find(id);
            db.Messages.Remove(message);
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
