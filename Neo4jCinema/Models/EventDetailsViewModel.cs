using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Neo4jCinema.Models
{
    public class EventDetailsViewModel : BaseViewModel
    {
        public Event Event { get; set; }
        public IEnumerable<User> PeopleIFollowThatAttend { get; set; }
    }
}