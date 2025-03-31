using Microsoft.AspNetCore.Mvc.Filters;

namespace CarRentalPortal.Helpers
{
    public class SuppressModelStateInvalidFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            // Do nothing; this suppresses the automatic model state validation
        }
    }
}
