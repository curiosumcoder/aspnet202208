using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Northwind.Store.Services.Filters
{
    public class LogRequestAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            System.Diagnostics.Debug.WriteLine($"{context.ActionArguments.ToArray()[0]}");
        }
    }
}
