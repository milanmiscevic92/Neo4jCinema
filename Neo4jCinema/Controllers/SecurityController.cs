using Domain.Abstract;
using Neo4jCinema.Controllers;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Neo4jCinema.Models
{
    public class SecurityController : AccountController
    {
        public string CinemaUserAuthValue;

        public SecurityController(IUserRepository usRepo, IGraphClient client) : base(usRepo, client)
        {
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpCookieCollection cookies = Request.Cookies;

            if (cookies[CinemaUserAuthKey] != null)
            {
                CinemaUserAuthValue = Request.Cookies["CinemaUserAuthKey"].Value;

                base.OnActionExecuting(filterContext);
            }
            else
            {
                filterContext.Result = new RedirectResult("/Home");
                return;
            }
        }
    }
}