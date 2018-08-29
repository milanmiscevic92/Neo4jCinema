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
    public class CinemaController : SecurityController
    {
        private ICinemaRepository cinemaRepo;
        private ICityRepository cityRepo;
        private IGraphClient graphClient;

        public CinemaController(ICinemaRepository cinRepo, ICityRepository citRepo,IUserRepository usRepo, IGraphClient client):base(usRepo, client)
        {
            cinemaRepo = cinRepo;
            cityRepo = citRepo;
            graphClient = client;
        }

        // GET: Cinema
        public ActionResult Index()
        {
            CinemaListViewModel vm = new CinemaListViewModel();
            vm.Cinemas = cinemaRepo.GetCinemas();

            foreach(var c in vm.Cinemas)
            {
                c.CinemaLocation = c.ReturnCinemaLocationName(c.CinemaId, graphClient);
            }

            return View(vm);
        }

        public ViewResult EditCinema(string cinemaId)
        {
            EditCinemaViewModel vm = new EditCinemaViewModel();
            vm.Cinema = cinemaRepo.GetCinemaById(cinemaId);
            vm.CurrentCinemaLocationId = vm.Cinema.ReturnCinemaLocationId(cinemaId, graphClient);
            vm.CitiesSelectList = (from c in cityRepo.GetCities() select new SelectListItem { Text = c.Name, Value = c.CityId });

            return View(vm);
        }

        [HttpPost]
        public ActionResult EditCinema(EditCinemaViewModel vm)
        {
            if(vm.Cinema.CinemaId == null)
            {
                // Create cinema
                cinemaRepo.InsertCinema(vm.Cinema);
                vm.Cinema.AddCinemaLocation(vm.Cinema.CinemaId, vm.SelectedCinemaLocationId, graphClient);
            }
            else
            {
                // Update cinema
                cinemaRepo.UpdateCinema(vm.Cinema);
                vm.Cinema.RemoveCinemaLocation(vm.Cinema.CinemaId, graphClient);
                vm.Cinema.AddCinemaLocation(vm.Cinema.CinemaId, vm.SelectedCinemaLocationId, graphClient);
            }

            return RedirectToAction("Index");
        }

        public ViewResult CreateCinema()
        {
            EditCinemaViewModel vm = new EditCinemaViewModel();
            vm.CitiesSelectList = (from c in cityRepo.GetCities() select new SelectListItem { Text = c.Name, Value = c.CityId });

            return View("EditCinema", vm);
        }

        [HttpPost]
        public ActionResult DeleteCinema(string cinemaId)
        {
            if(cinemaId != null)
            {
                cinemaRepo.DeleteCinema(cinemaId);
            }

            return RedirectToAction("Index");
        }
    }
}