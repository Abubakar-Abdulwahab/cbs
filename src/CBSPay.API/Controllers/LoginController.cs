
using System;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using CBSPay.API.App_Start;
using CBSPay.API.Models;
using CBSPay.Core.Helpers;
using CBSPay.Core.ViewModels;
using Microsoft.AspNet.Identity.Owin;

namespace CBSPay.API.Controllers
{
    public class LoginController : Controller
    {
        public SignInManager SignInManager
        {
            get { return HttpContext.GetOwinContext().Get<SignInManager>(); }
        }
        public UserManager UserManager
        {
            get { return HttpContext.GetOwinContext().GetUserManager<UserManager>(); }
        } 
        public LoginController()
        {
            
        }
        // GET: Login
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Home()
        {
            ViewBag.Login = "_StaffLogin";
            ViewBag.Title = "Login";
            return View();
        }

        [Authorize]
        public ActionResult RegisterStaffUser()
        {
            ViewBag.Login = "_Register";
            ViewBag.Title = "Register Staff User";
            return View();
        }
         

        [HttpPost]
        [Route("login/LoginUser")]
        public ActionResult LoginUser(LoginViewModel model)
        {
            try
            {
                //SignInManager
                var result = SignInManager.PasswordSignIn(model.UserName, model.Password, false, false);
                if (result == SignInStatus.Success)
                {
                    return RedirectToAction("Index", "Report");
                }
                else
                {
                    throw new Exception("You are not authorized to view this page");
                }
                 
            }
            catch (Exception ex)
            {
                ViewBag.ResponseCode = "505";
                ViewBag.ErrorMessage = ex.Message;
                return View("ErrorPage");
            }
             
        }
        
        public ActionResult Logout()
        {
            SignInManager.SignOut();
            return RedirectToAction("Index", "Home");

        }
        public ActionResult SignUp()
        {
            return View();
        }

    }
}