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
    public class AccountController : Controller
    {
        private IUserRepository userRepo;
        private IGraphClient graphClient;

        public string CinemaUserAuthKey = "CinemaUserAuthKey";

        public AccountController(IUserRepository usRepo, IGraphClient client)
        {
            userRepo = usRepo;
            graphClient = client;

        }

        public ActionResult SignOut()
        {
            HttpCookieCollection cookies = Request.Cookies;

                HttpCookie myCookie = new HttpCookie(CinemaUserAuthKey);
                myCookie.Expires = DateTime.Now.AddDays(-1d);
                Response.Cookies.Add(myCookie);

                return RedirectToAction("Index", "Home");

            

        }

        [HttpPost]
        public ActionResult SignIn(string username, string password)
        {
            User user = userRepo.GetUserByUsername(username);
            
            if (user != null && user.AuthenticatePassword(password) == true)
            {
                HttpCookie userCookie = new HttpCookie(CinemaUserAuthKey);
                userCookie.Value = user.Username;
                userCookie.Expires = DateTime.Now.AddMinutes(20);
                Response.Cookies.Add(userCookie);

                return RedirectToAction("Index", "MainDashboard");
            }

            else
            {
                ViewBag.Message = "Wrong username or password.";

                return View("Welcome");
            }
            
        }

        [HttpPost]
        public ActionResult RegisterUser(User user)
        {
            if(user.UsernameExists(user.Username, graphClient) == false && user.EmailAddressExists(user.EmailAddress, graphClient) == false)
            {
                userRepo.InsertUser(user);

                return RedirectToAction("Index", "Home");
            }

            else
            {
                ViewBag.Message = "USER ALREADY EXISTS WITH THAT INFORMATION!";

                return View("Welcome");
            }
        }

        public ViewResult EditUser(string userId)
        {
            AccountDashboardViewModel vm = new AccountDashboardViewModel();
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.Following = vm.CurrentUser.ReturnFollowing(userId, graphClient);
            vm.Followers = vm.CurrentUser.ReturnFollowers(userId, graphClient);
            vm.FoundUsers = Enumerable.Empty<User>();
            vm.SearchUsername = null;
            return View(vm);
        }



        [HttpPost]
        public ActionResult UpdateInformation(AccountDashboardViewModel vm, string userId)
        {
            User tempUser = userRepo.GetUserById(userId);

            if(tempUser.EmailBelongsToAnotherUser(vm.CurrentUser.EmailAddress, userId, graphClient) == false)
            {
                tempUser.FirstName = vm.CurrentUser.FirstName;
                tempUser.LastName = vm.CurrentUser.LastName;
                tempUser.EmailAddress = vm.CurrentUser.EmailAddress;
                userRepo.UpdateUser(tempUser);
            }

            else
            {
                ViewBag.Message = "EMAIL ADDRESS BELONGS TO ANOTHER USER!";
                return View("Welcome");
            }

            return Redirect(Request.UrlReferrer.ToString());
        }



        [HttpPost]
        public ActionResult UpdatePassword(AccountDashboardViewModel vm, string userId)
        {
            vm.CurrentUser = userRepo.GetUserById(userId);

            if(vm.CurrentUser.Password == vm.OldPassword)
            {
                vm.CurrentUser.Password = vm.NewPassword;
                userRepo.UpdateUser(vm.CurrentUser);
                return RedirectToAction("SignOut");
            }
            else
            {
                ViewBag.Message = "PASSWORDS DO NOT MATCH";
                return View("Welcome");
            }

        }

        public ActionResult FindUserByUsername(string userId, AccountDashboardViewModel vm)
        {
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.Following = vm.CurrentUser.ReturnFollowing(userId, graphClient);
            vm.Followers = vm.CurrentUser.ReturnFollowers(userId, graphClient);
            vm.FoundUsers = userRepo.GetUsersThatContainUsername(vm.SearchUsername).Where(u => u.UserId != userId);

            return View("EditUser", vm);
        }

        public ActionResult FollowUser(string userId, string followedUsername)
        {
            AccountDashboardViewModel vm = new AccountDashboardViewModel();
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.CurrentUser.FollowUser(userId, followedUsername, graphClient);

            return RedirectToAction("EditUser", "Account", new { vm.CurrentUser.UserId });
        }

        public ActionResult UnfollowUser(string userId, string unfollowedUsername)
        {
            AccountDashboardViewModel vm = new AccountDashboardViewModel();
            vm.CurrentUser = userRepo.GetUserById(userId);
            vm.CurrentUser.UnfollowUser(userId, unfollowedUsername, graphClient);

            return RedirectToAction("EditUser", "Account", new { vm.CurrentUser.UserId });
        }


    }
}