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
    public class HomeController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();

        public ActionResult Index()
        {
            List<Product> productslist = new List<Product>();
            var products = db.Products.Include(p => p.Brand).Include(p => p.Category).ToList();
            var count = 0;
            foreach (var item in products)
            {
                if (count < 4)
                {
                    productslist.Add(item);
                    count++;
                }
                else
                    break;
            }

            return View(productslist);
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int id)
        {
            return RedirectToAction("Edit", "Products", new { id = id });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Details(int id)
        {
            return RedirectToAction("Details", "Products", new { id = id });
        }

        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            return RedirectToAction("Delete", "Products", new { id = id });
        }

        public JsonResult NewsLetterSubscribe(string val)
        {
            string msg = "Your Email Id is successfuly registered for News Letter..!";
            try
            {
                var RegisterCheck = db.NewsLetterEmails.FirstOrDefault(s => s.Email == val);
                if (RegisterCheck != null)
                {
                    msg = "Your Email id is Already Register for News Letter..!";
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    var newsletterRegister = new NewsLetterEmail();
                    newsletterRegister.Email = val;
                    db.NewsLetterEmails.Add(newsletterRegister);
                    db.SaveChanges();
                    return Json(msg, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                msg = "AN ERROR OCCURED..! PLEASE CONTACT WITH DEVELOPER..!!";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
        }

        public JsonResult NewsLetterUnSubscribe(string val)
        {
            string msg = "";
            try
            {
                var RegisterCheck = db.NewsLetterEmails.FirstOrDefault(s => s.Email == val);
                db.NewsLetterEmails.Remove(RegisterCheck);
                db.SaveChanges();
                msg = "Your email id is Unsubscribed for News Letter..!";
                return Json(msg, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                msg = "Your email id is unsubscribed for News Letter..!";
                return Json(msg, JsonRequestBehavior.AllowGet);

            }
        }

        public ActionResult Search(string searchStrings)
        {
            var searchString = Request.Form["searchBar"];
            ViewBag.Message = "Search Results for: '" + searchString + "'";
            var products = db.Products.Include(p => p.Brand).Include(p => p.Category).Where(s => s.ProductName == searchString).ToList();

            if (products.Count == 0)
            {
                ViewBag.Message = "Search Results for: '" + searchString + "' couldn't found.. Please try new search";

            }
            return View(products);
        }
    }
}