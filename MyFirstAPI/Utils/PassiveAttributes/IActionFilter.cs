using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyFirstAPI.Utils.PassiveAttributes
{
    public interface IAsyncActionFilter<TAttribute> where TAttribute : Attribute
    {
        Task OnActionExecutingAsync(TAttribute attribute, ActionExecutingContext context);
    }
}