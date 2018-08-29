using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using System.Web.Mvc;

namespace Neo4jCinema.Models
{
    public class RegistrationViewModel
    {
        public User User { get; set; }
    }
}