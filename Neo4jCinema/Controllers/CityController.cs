using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Domain.Entities;
using Domain.Abstract;
using Neo4jCinema.Models;
using Neo4jClient;

namespace Neo4jCinema.Controllers
{
    public class CityController : SecurityController
    {
        private ICityRepository cityRepo;
        private IGraphClient graphClient;

        public CityController(ICityRepository cRepo, IUserRepository usRepo, IGraphClient client):base(usRepo, client)
        {
            cityRepo = cRepo;
            graphClient = client;
        }

        // GET: City
        [HttpGet]
        public ActionResult Index()
        {
            CityListViewModel model = new CityListViewModel
            {
                Cities = cityRepo.GetCities()
            };

            return View(model);
        }

        [HttpGet]
        public ViewResult EditCity(string cityid)
        {
            City city = cityRepo.GetCityById(cityid);
            return View(city);
        }

        [HttpPost]
        public ActionResult EditCity(City city)
        {
            if (ModelState.IsValid)
            {
                if (city.CityId == null)
            {
                // Create
                cityRepo.InsertCity(city);
            }
                else
            {
                // Update
                cityRepo.UpdateCity(city);
            }
                return RedirectToAction("Index");
            }

            else
            {
                ViewBag.Message = "ENTER DETAILS";

                return View(city);

            }
        }

        [HttpGet]
        public ViewResult CreateCity()
        {
            return View("EditCity", new City());
        }

        [HttpPost]
        public ActionResult DeleteCity(string cityId)
        {
            if (cityId != null)
            {
                cityRepo.DeleteCity(cityId);
            }

            return RedirectToAction("Index");
        }
    }
}