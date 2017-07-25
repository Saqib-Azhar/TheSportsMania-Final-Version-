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
    public class OrdersController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult ConfirmOrder()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.StatusID = new SelectList(db.OrderStatus, "Id", "Status");
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName");
            return View();
        }

        public JsonResult ConfirmedOrder(string email, string phone, string address)
        {
            var TransactionObj = db.Database.BeginTransaction();
            try
            {
                if (email != null && phone != null && address != null)
                {
                    var user = User.Identity;
                    var userID = db.AspNetUsers.FirstOrDefault(s => s.UserName == user.Name);

                    var CartItems = db.Carts.Where(s => s.UserID == userID.Id).ToList();

                    Message autoMessage = new Message();
                    String msg = "Your Order Confirmed, Items ordered: ";

                    foreach (var item in CartItems)
                    {

                        //var prod = db.Products.FirstOrDefault(s => s.Id == item.ProductID);
                        msg = msg + "<br/> &rArr; Item Name: " + item.Product.ProductName + " Items Count: " + item.ProductCount;
                        Order NewOrder = new Order();
                        NewOrder.Address = address;
                        NewOrder.EmailID = email;
                        NewOrder.PhoneNumber = phone;
                        NewOrder.StatusID = 1;
                        NewOrder.Time = DateTime.Now;
                        NewOrder.ProductID = item.ProductID;
                        NewOrder.ProductCount = item.ProductCount;
                        NewOrder.UserID = item.UserID;
                        NewOrder.TotalProductPrice = item.TotalProductPrice;
                        db.Orders.Add(NewOrder);
                        db.Carts.Remove(item);
                        db.SaveChanges();
                    }

                    autoMessage.Name = "Auto Generated Message";
                    autoMessage.Email = "system@sportsmania.com";
                    autoMessage.Contact = "info@sportsmania.com";
                    autoMessage.Message1 = msg + "<br/> YOU WILL GET THIS ORDER WITHIN 3-7 WORKING DAYS..";
                    autoMessage.RecieverID = userID.Id;
                    autoMessage.CreationTime = DateTime.Now;
                    autoMessage.Seen = false;

                    db.Messages.Add(autoMessage);
                    db.SaveChanges();

                    TransactionObj.Commit();
                    return Json("Orders Placed Successfully..!", JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                TransactionObj.Dispose();
            }

            return Json("Something went wrong..!", JsonRequestBehavior.AllowGet);
        }
        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        // ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public ActionResult Index()
        {
            if (User.IsInRole("User"))
            {
                var userId = User.Identity;
                var userObj = db.AspNetUsers.FirstOrDefault(s => s.UserName == userId.Name);
                var orders = db.Orders.Include(o => o.AspNetUser).Include(o => o.OrderStatu).Include(o => o.Product);
                return View(orders.Where(s => s.UserID == userObj.Id).ToList());
            }
            else
            {
                var orders = db.Orders.Include(o => o.AspNetUser).Include(o => o.OrderStatu).Include(o => o.Product);
                return View(orders.ToList());
            }
        }

        // GET: Orders/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email");
            ViewBag.StatusID = new SelectList(db.OrderStatus, "Id", "Status");
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName");
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,UserID,ProductID,ProductCount,Address,PhoneNumber,EmailID,Time,StatusID,TotalProductPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", order.UserID);
            ViewBag.StatusID = new SelectList(db.OrderStatus, "Id", "Status", order.StatusID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", order.ProductID);
            return View(order);
        }

        // GET: Orders/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", order.UserID);
            ViewBag.StatusID = new SelectList(db.OrderStatus, "Id", "Status", order.StatusID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", order.ProductID);
            return View(order);
        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,UserID,ProductID,ProductCount,Address,PhoneNumber,EmailID,Time,StatusID,TotalProductPrice")] Order order)
        {
            if (ModelState.IsValid)
            {
                var orderobj = db.Orders.Find(order.Id);
                order.UserID = orderobj.UserID;
                order.ProductID = orderobj.ProductID;
                order.Product = orderobj.Product;
                order.TotalProductPrice = order.ProductCount * order.Product.Price;
                order.Time = DateTime.Now;
                order.OrderStatu = db.OrderStatus.Find(order.StatusID);
                order.AspNetUser = db.AspNetUsers.Find(order.UserID);
                db.Orders.Remove(orderobj);
                db.SaveChanges();
                orderobj = order;
                db.Orders.Add(orderobj);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", order.UserID);
            ViewBag.StatusID = new SelectList(db.OrderStatus, "Id", "Status", order.StatusID);
            ViewBag.ProductID = new SelectList(db.Products, "Id", "ProductName", order.ProductID);
            return View(order);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order order = db.Orders.Find(id);
            if (order == null)
            {
                return HttpNotFound();
            }
            return View(order);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order order = db.Orders.Find(id);
            db.Orders.Remove(order);
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
