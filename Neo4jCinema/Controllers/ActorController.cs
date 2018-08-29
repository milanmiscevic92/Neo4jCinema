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
    public class ActorController : SecurityController
    {
        private IActorRepository actorRepo;
        private IUserRepository userRepo;
        private IGraphClient graphClient;


        public ActorController(IActorRepository acRepo, IUserRepository usRepo, IGraphClient client):base(usRepo, client)
        {
            actorRepo = acRepo;
            userRepo = usRepo;
            graphClient = client;
        }
        
        // GET: Actor
        public ActionResult Index()
        {
            ActorListViewModel vm = new ActorListViewModel();
            vm.Actors = actorRepo.GetActors();

            return View(vm);
        }

        public ViewResult ActorDetails(string userId, string actorId)
        {
            ActorDetailsViewModel vm = new ActorDetailsViewModel();
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.Actor = actorRepo.GetActorById(actorId);
            vm.ActorsMovies = vm.Actor.ReturnAllMoviesFromActor(actorId, graphClient);

            return View("ActorDetails", vm);
        }


        public ViewResult EditActor(string actorId)
        {
            Actor actor = actorRepo.GetActorById(actorId);
            return View(actor);
        }

        [HttpPost]
        public ActionResult EditActor(Actor actor)
        {
            if(actor.ActorId == null)
            {
                actorRepo.InsertActor(actor);
            }

            else
            {
                actorRepo.UpdateActor(actor);
            }

            return RedirectToAction("Index");
        }

        public ViewResult CreateActor()
        {
            return View("EditActor", new Actor());
        }

        [HttpPost]
        public ActionResult DeleteActor(string actorId)
        {
            if (actorId != null)
            {
                actorRepo.DeleteActor(actorId);   
            }

            return RedirectToAction("Index");
        }
    }
}