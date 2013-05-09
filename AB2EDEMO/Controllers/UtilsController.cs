using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Web.Mvc;

namespace AB2EDEMO.Controllers
{
    public class UtilsController : Controller
    {
        /// <summary>
        ///Contains Code for the Date and Time Displayed on the Layout Header
        ///Default Action:
        ///GET /WorkWithCustomer/GetServerTime
        /// </summary>
        /// <returns></returns>
        public ActionResult GetServerTime()
        {
            return Content(DateTime.Today.DayOfWeek + " " + DateTime.Now.ToString("dd/MM/yyyy hh:mm"));
        }

        public static NameValueCollection MessagesCol;

        /// <summary>
        /// Method to load custom messages from a message file
        /// </summary>
        public static void LoadMessages()
        {
            var sr = new StreamReader(System.Web.HttpContext.Current.Server.MapPath("~\\messages_en.properties"));
            var fileContents = sr.ReadToEnd();
            var sepr = new[] { "\r\n" };
            var lines = fileContents.Split(sepr, StringSplitOptions.RemoveEmptyEntries);
            MessagesCol = new NameValueCollection();
            foreach (var line in lines)
            {
                int index = line.IndexOf('=');
                if (index != -1)
                {
                    MessagesCol.Add(line.Substring(0, index), line.Substring(index + 1));
                }
            }
            sr.Close();
        }

        public NameValueCollection GetNavInfo()
        {
            if (Session["navigationHistory"] == null)
            {
                return new NameValueCollection() { { "SESSION_EXPIRED", "TRUE" } };
            }
            var navInfo = (Stack<NameValueCollection>)Session["navigationHistory"];
            if (navInfo.Count ==1 || navInfo.Count==0)
            {
                return new NameValueCollection {{"Login", "Index"}};
            }
            navInfo.Pop();
            return navInfo.Pop();
        }
        /// <summary>
        /// Contains Code for the Cancel Action on all EntryPanel and Panel Screens
        /// Default Action:
        /// GET /Utils/Cancel
        /// </summary>
        /// <returns></returns>
        public ActionResult Cancel()
        {
            TempData["Result"] = " ";
            var navigation = GetNavInfo();
            if (navigation.AllKeys[0] == "SESSION_EXPIRED")
                return RedirectToAction("Index", "Login");
            TempData["cancelSelect"] = true;
            return RedirectToAction(navigation[0], navigation.AllKeys[0]);
        }
    }
}