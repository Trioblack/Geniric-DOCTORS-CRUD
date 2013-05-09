using System;
using System.Web.Mvc;
using System.Web.Mvc.Ajax;

//<summary>
//The Class has been made to Extend the functionality of default AjaxActionLink//
//Here, the code for extending the default AjaxActionLink written as method of the class//
//this class is  a generic class applicable to all the Views
//</summary>

namespace AB2EDEMO.Controllers
{
    public static class MyAjaxActionLink
    {
        public static MvcHtmlString RawActionLink(this AjaxHelper ajaxHelper, string linkText, string actionName,
                                                  object routeValues, AjaxOptions ajaxOptions)
        {
            var repId = Guid.NewGuid().ToString();
            var lnk = ajaxHelper.ActionLink(repId, actionName, routeValues, ajaxOptions);
            return MvcHtmlString.Create(lnk.ToString().Replace(repId, linkText));
        }
    }
}