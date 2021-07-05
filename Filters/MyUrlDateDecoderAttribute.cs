using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;

namespace MyRoverServiceAPI.Filters
{
    public class MyUrlDateDecoderAttribute : ActionFilterAttribute
    {
        private readonly string _parameter;

        public MyUrlDateDecoderAttribute(string parameter)
        {
            _parameter = parameter;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string encodedDateString = context.ActionArguments[_parameter] as string;
            context.ActionArguments[_parameter] = WebUtility.UrlDecode(encodedDateString);
            base.OnActionExecuting(context);
        }
    }

}
