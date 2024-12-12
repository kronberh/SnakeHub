using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace SnakeHub.Filters
{
    public class SinglePagePartialActionFilterAttribute : ActionFilterAttribute
    {
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (bool.TryParse(context.HttpContext.Request.Headers["IncludeLayout"], out bool includeLayout) && !includeLayout)
            {
                await next();
            }
            else
            {
                context.Result = new ViewResult() { ViewName = "_Loading" };
            }
        }
    }
}
