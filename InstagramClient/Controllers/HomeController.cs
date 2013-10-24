using DotNetOpenAuth.AspNet;
using DotNetOpenAuth.AspNet.Clients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TwitterClientExtended.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Login()
        {
            DotNetOpenAuth.AspNet.Clients.InstagramClient client = new DotNetOpenAuth.AspNet.Clients.InstagramClient("", "", new string[] { "basic", "comments", "relationships", "likes" });
            AuthenticationResult result = client.VerifyAuthentication(this.HttpContext,new Uri(Request.Url.AbsoluteUri));
            if (!result.IsSuccessful)
                client.RequestAuthentication(this.HttpContext, new Uri(Request.Url.AbsoluteUri));
            else
            {
                ViewBag.Message = JsonConvert.SerializeObject(result.ExtraData).ToString();
            }
            return View();
        }
    }
}