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
    public class EventController : SecurityController
    {
        private IEventRepository eventRepo;
        private ICinemaRepository cinemaRepo;
        private IUserRepository userRepo;
        private IGraphClient graphClient;

        public EventController(IEventRepository evRepo, ICinemaRepository ciRepo, IUserRepository usRepo, IGraphClient client) : base(usRepo, client)
        {
            eventRepo = evRepo;
            cinemaRepo = ciRepo;
            userRepo = usRepo;
            graphClient = client;
        }

        // GET: Event
        
        public ActionResult Index()
        {
            EventListViewModel vm = new EventListViewModel();
            vm.Events = eventRepo.GetEvents();

            foreach(var e in vm.Events)
            {
                e.EventLocation = e.ReturnEventLocationName(e.EventId, graphClient);
            }

            return View(vm);
        }


        public ViewResult EventDetails(string userId, string eventId)
        {
            EventDetailsViewModel vm = new EventDetailsViewModel();
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.Event = eventRepo.GetEventById(eventId);
            vm.Event.EventLocation = vm.Event.ReturnEventLocationName(eventId, graphClient);

            vm.PeopleIFollowThatAttend = vm.Event.ReturnAllFriendsAttending(userId, eventId, graphClient);

            if(vm.Event.EventAttendedByUser(userId, eventId, graphClient) == false)
            {
                ViewBag.AttendingEvent = 0;
            }
            else
            {
                ViewBag.AttendingEvent = 1;
            }

            return View("EventDetails", vm);
        }

        public ActionResult AttendEvent(string userId, string eventId)
        {
            Event ev = eventRepo.GetEventById(eventId);
            ev.AttendEvent(userId, eventId, graphClient);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ActionResult CancelEvent(string userId, string eventId)
        {
            Event ev = eventRepo.GetEventById(eventId);
            ev.CancelEvent(userId, eventId, graphClient);

            return Redirect(Request.UrlReferrer.ToString());
        }

        public ViewResult EditEvent(string eventId)
        {
            EditEventViewModel vm = new EditEventViewModel();
            vm.Ev = eventRepo.GetEventById(eventId);
            vm.CurrentCinemaId = vm.Ev.ReturnEventLocationId(eventId, graphClient);
            vm.CinemasSelectList = (from c in cinemaRepo.GetCinemas() select new SelectListItem { Text = c.CinemaName, Value = c.CinemaId });

            return View(vm);

        }

        [HttpPost]
        public ActionResult EditEvent(EditEventViewModel vm)
        {
            if(vm.Ev.EventId == null)
            {
                // Create
                eventRepo.InsertEvent(vm.Ev);
                vm.Ev.AddEventLocation(vm.Ev.EventId, vm.SelectedCinemaId, graphClient);

            }
            else
            {
                // Update
                eventRepo.UpdateEvent(vm.Ev);
                vm.Ev.RemoveEventLocation(vm.Ev.EventId, graphClient);
                vm.Ev.AddEventLocation(vm.Ev.EventId, vm.SelectedCinemaId, graphClient);
            }

            return RedirectToAction("Index");
        }

        public ViewResult CreateEvent()
        {
            EditEventViewModel vm = new EditEventViewModel();
            vm.CinemasSelectList = (from c in cinemaRepo.GetCinemas() select new SelectListItem { Text = c.CinemaName, Value = c.CinemaId });

            return View("EditEvent", vm);
        }

        [HttpPost]
        public ActionResult DeleteEvent(string eventId)
        {
            if(eventId != null)
            {
                eventRepo.DeleteEvent(eventId);
            }

            return RedirectToAction("Index");
        }
    }
}