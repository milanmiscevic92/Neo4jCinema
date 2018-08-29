using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Entities;
using Domain.Abstract;
using Neo4jCinema.Models;

namespace Neo4jCinema.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Index()
        {
            ViewBag.Navbar = 1;

            return View();
        }

        public ViewResult Registration()
        {
            ViewBag.Navbar = 2;

            return View();
        }

        public ViewResult About()
        {
            ViewBag.Navbar = 1;

            return View();
        }
    }
}