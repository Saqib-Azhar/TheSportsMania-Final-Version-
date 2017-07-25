using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SportsMania.Models;

namespace SportsMania.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminPanelController : Controller
    {
        private SportsManiaEntities db = new SportsManiaEntities();
        // GET: AdminPanel
        public ActionResult Panel()
        {
            var Userid = User.Identity;
            var UserObj = db.AspNetUsers.FirstOrDefault(s => s.UserName == Userid.Name);
            return View(UserObj);
        }
    }
}