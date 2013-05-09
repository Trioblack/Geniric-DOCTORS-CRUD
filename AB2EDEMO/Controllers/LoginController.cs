using System;
using System.Data.Entity.Infrastructure;
using System.Web.Configuration;
using System.Web.Mvc;
using AB2EDEMO.Models;

namespace AB2EDEMO.Controllers
{
    public class LoginController : Controller
    {
        /// <summary>
        /// Contains code for the default action (index page call)
        /// Action Handled::
        /// GET: /Login/Index or GET: /Login or GET: /
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            Session.Abandon();
            return PartialView();
        }

        /// <summary>
        /// Contains code for the post action (index page call)
        /// Action Handled::
        /// POST: /Login/Index or POST: /Login or POST: /
        /// </summary>
        /// <param name="formCollection">Form Colletion Object to get Username and Password from the login screen</param>
        /// <returns>If Credentials are correct redirects to the first Grid Screen</returns>
        [HttpPost]
        public ActionResult Index(FormCollection formCollection)
         {
            string userName = formCollection[2].Trim();
            string password = formCollection[3].Trim();
            var dataConnection =
                WebConfigurationManager.OpenWebConfiguration("/").ConnectionStrings.ConnectionStrings[
                    "Ab2edemoEntities"].ConnectionString;
            //dataConnection = dataConnection.Substring(0, dataConnection.LastIndexOf("AB2EDEMO;") + "AB2EDEMO;".Length - 1) + ";USER =" +
            //                 userName + ";Password=" + password + ";" +
            //                 dataConnection.Substring(dataConnection.LastIndexOf("AB2EDEMO;") + "AB2EDEMO;".Length + 1, dataConnection.Length - dataConnection.LastIndexOf("AB2EDEMO;") - "AB2EDEMO;".Length);

            try
            {
                ((IObjectContextAdapter)new Ab2edemoEntities(dataConnection)).ObjectContext.Connection.Open();
                Session["username"] = userName;
                TempData["programCall"] = true;
                Session["connectionString"] = dataConnection;
                return RedirectToAction("HospitalGrid", "Hospital");
            }
            catch (Exception e)
            {
                if (e.InnerException.ToString().Contains("USERNAME"))
                {
                    TempData["error"] = "User Name and/or Password invalid";
                }
                else if (e.InnerException.Message.Contains("PASSWORD MISSING") ||
                         e.InnerException.Message.Contains("USERID MISSING"))
                {
                    TempData["error"] = "User Name and/or Password Missing";
                }
                else
                {
                    TempData["error"] = "An error occurred please try again later.";
                }
            }
            return PartialView();
        }
    }
}