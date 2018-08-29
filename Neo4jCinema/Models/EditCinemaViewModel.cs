using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using System.Web.Mvc;

namespace Neo4jCinema.Models
{
    public class EditCinemaViewModel
    {
        public Cinema Cinema { get; set; }
        public IEnumerable<SelectListItem> CitiesSelectList { get; set; }
        public string SelectedCinemaLocationId { get; set; }
        public string CurrentCinemaLocationId { get; set; }
    }
}