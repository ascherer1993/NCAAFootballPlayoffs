using NCAAFootballPlayoffs.Models;
using NCAAFootballPlayoffs.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NCAAFootballPlayoffs
{
    //This class creates the attribute to check if people are authorized to access certain pages
    //[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    [AttributeUsage(validOn: AttributeTargets.Method)]
    public class AuthorizeUser : AuthorizeAttribute
    {
        private bool loggedIn = false;

        private string[] roles = new string[0];


        //Constructor that takes in a single role
        public AuthorizeUser(string role) : this(new string[] { role })
        {
        }

        //Overload that takes in multiple roles
        public AuthorizeUser(string[] allowedRoles = null)
        {
            roles = allowedRoles ?? new string[0];
        }

        /// The logic that determines whether a user is authorized or not. 
        /// If no roles were specified, authorize based on whether the user is logged in or not.
        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            loggedIn = Authentication.IsSignedIn();

            if (!loggedIn)
            {
                return false;
            }
            //We have roles to check for, so we're not done yet.
            if (roles.Length != 0)
            {
                return Authentication.IsMemberOf(roles);
            }
            else //Else, with no roles to check, just return our login check.
            {
                return loggedIn;
            }
        }

        //This is run if authorize core returns false. If so, redirect to the appropriate page.
        protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
        {
            if (loggedIn)
            {
                filterContext.Result = new RedirectResult("~/Error/Unauthorized");
            }
            else
            {
                filterContext.Result = new RedirectResult("~/UserAccount/SignIn", false);
            }
        }
    }

    //Attribute that checks to make sure a method is only called via ajax
    public class AjaxOnlyAttribute : ActionMethodSelectorAttribute
    {
        public override bool IsValidForRequest(ControllerContext controllerContext, System.Reflection.MethodInfo methodInfo)
        {
            return controllerContext.RequestContext.HttpContext.Request.IsAjaxRequest();
        }
    }
}