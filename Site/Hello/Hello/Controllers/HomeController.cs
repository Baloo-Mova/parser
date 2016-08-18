using Hello.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Hello.Controllers
{
    public class HomeController : Controller
    {
        ApplicationContext db = new ApplicationContext();
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Index(Users mod)
        {
            if (ModelState.IsValid)
            {
                var test = db.users.Where(e => e.login == mod.login).FirstOrDefault();
                if (test == null)
                {
                    db.users.Add(mod);
                    db.SaveChanges();
                    
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("login", "Данный логин уже существует");
                }
            }
            return View(mod);
        }
        public ActionResult Register()
        {
            return View();
        }

     
    }
}