using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace project_c.Helpers
{
    public static class HtmlHelpers
    {
        public static string IsSelected(this IHtmlHelper htmlHelper, string controllers, string actions,
            string cssClass = "bg-green-800")
        {
            string currentAction = htmlHelper.ViewContext.RouteData.Values["action"] as string;
            string currentController = htmlHelper.ViewContext.RouteData.Values["controller"] as string;

            IEnumerable<string> acceptedActions = (actions ?? currentAction).Split(',');
            IEnumerable<string> acceptedControllers = (controllers ?? currentController).Split(',');

            return acceptedActions.Contains(currentAction) && acceptedControllers.Contains(currentController)
                ? cssClass
                : String.Empty;
        }

        public static string SetOptionSelected(this IHtmlHelper htmlHelper, bool isSelected)
        {
            return isSelected ? "selected" : String.Empty;
        }
    }
}