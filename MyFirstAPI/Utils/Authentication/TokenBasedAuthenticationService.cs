using System;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Models;

namespace MyFirstAPI.Utils.Authentication
{
    public class TokenBasedAuthenticationService : IAuthenticationService
    {
        private readonly TodoContext dbContext;

        public TokenBasedAuthenticationService(/* DI */ TodoContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string? scheme)
        {
            // Check if we're logged in

            // Token exists?
            var token = context.Request.Cookies["SessionToken"];
            if (token == null)
            {
                return AuthenticateResult.Fail("Token not passed in request");
            }

            // Session exists?
            var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session == null)
            {
                return AuthenticateResult.Fail("User is not logged in");
            }

            // User exists?
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == session.Username);
            if (user == null)
            {
                return AuthenticateResult.Fail("User no longer exists, but their token does? Strange");
            }

            return AuthenticateResult.Success(null);
        }

        public Task ChallengeAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            // Check if we're logged in
            /*
            // Token exists?
            var token = context.Request.Cookies["SessionToken"];
            if (token == null)
            {
                return AuthenticateResult.Fail("Token not passed in request");
            }

            // Session exists?
            var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session == null)
            {
                return AuthenticateResult.Fail("User is not logged in");
            }

            // User exists?
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == session.Username);
            if (user == null)
            {
                return AuthenticateResult.Fail("User no longer exists, but their token does? Strange");
            }

            return AuthenticateResult.Success(null);


            */
            // Fail authentication
            context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            return Task.CompletedTask;
        }

        public Task ForbidAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            throw new System.NotImplementedException();
        }

        public Task SignInAsync(HttpContext context, string? scheme, ClaimsPrincipal principal, AuthenticationProperties? properties)
        {
            throw new System.NotImplementedException();
        }

        public Task SignOutAsync(HttpContext context, string? scheme, AuthenticationProperties? properties)
        {
            throw new System.NotImplementedException();
        }
    }
}