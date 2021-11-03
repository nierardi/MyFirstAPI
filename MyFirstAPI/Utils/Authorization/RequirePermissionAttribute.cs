using System;
using Microsoft.AspNetCore.Mvc.Filters;

namespace MyFirstAPI.Utils.Authorization
{
    public class RequirePermissionAttribute : Attribute, IFilterFactory
    {
        public string Permission { get; set; }
        public bool IsReusable { get => false; }

        public IFilterMetadata CreateInstance(IServiceProvider serviceProvider)
        {
            var permissionFilter = (RequirePermissionActionFilter) serviceProvider.GetService(typeof(RequirePermissionActionFilter));
            permissionFilter.Permission = Permission;

            return permissionFilter;
        }

    }
}