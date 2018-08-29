using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;
using System.Web.Mvc;

namespace Neo4jCinema.Models
{
    public class EditEventViewModel
    {
        public Event Ev { get; set; }
        public IEnumerable<SelectListItem> CinemasSelectList { get; set; }
        public string SelectedCinemaId { get; set; }
        public string CurrentCinemaId { get; set; }
    }
}