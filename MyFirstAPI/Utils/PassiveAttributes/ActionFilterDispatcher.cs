using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyFirstAPI.Utils.PassiveAttributes
{
    // https://blogs.cuttingedge.it/steven/posts/2014/dependency-injection-in-attributes-dont-do-it/

    public sealed class ActionFilterDispatcher : IAsyncActionFilter
    {
        private readonly Func<Type, IEnumerable> container;

        public ActionFilterDispatcher(Func<Type, IEnumerable> container)
        {
            this.container = container;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            IEnumerable<object> attributes = context.Controller.GetType().GetTypeInfo().GetCustomAttributes(true);

            var descriptor = context.ActionDescriptor as ControllerActionDescriptor;

            if (descriptor != null)
            {
                attributes = attributes.Concat(descriptor.MethodInfo.GetCustomAttributes(true));
            }

            foreach (var attribute in attributes)
            {
                Type filterType = typeof(IAsyncActionFilter<>).MakeGenericType(attribute.GetType());
                var filters = this.container.Invoke(filterType);

                foreach (dynamic actionFilter in filters)
                {
                    actionFilter.OnActionExecuting((dynamic) attribute, context);
                }
            }
        }
    }
}