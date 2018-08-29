using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Domain.Entities;

namespace Neo4jCinema.Models
{
    public class AccountDashboardViewModel : BaseViewModel
    {
        // Used to change password
        public string OldPassword { get; set; }
        public string NewPassword { get; set; }

        // Used to find users
        public string SearchUsername { get; set; }
        public IEnumerable<User> FoundUsers { get; set; }

        // Followers and following
        public IEnumerable<User> Following { get; set; }
        public IEnumerable<User> Followers { get; set; }
    }
}