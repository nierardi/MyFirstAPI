using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyFirstAPI.Models;
using MyFirstAPI.Models.DTO;

namespace MyFirstAPI.Controllers
{
    [ApiController, Route("api/[controller]")]
    public class AuthenticationController : ControllerBase
    {
        private readonly TodoContext dbContext;

        public AuthenticationController(/* DI */ TodoContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("login")]
        public async Task<ActionResult> PostLogin(LoginDTO loginData)
        {
            var username = loginData.Username;
            var password = loginData.Password;

            if (username == null || password == null)
            {
                return BadRequest(new {error = "Username or password not specified"});
            }

            // Try to find the user
            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
            if (user == null)
            {
                return Unauthorized(new {error = "Incorrect username or password"});
            }
            else
            {
                // Create a session for this user
                var token = GenerateRandomToken();
                await dbContext.AddAsync(new Session()
                {
                    Token = token,
                    Username = username
                });
                await dbContext.SaveChangesAsync();

                Response.Cookies.Append("SessionToken", token);
                return Ok();
            }
        }

        private string GenerateRandomToken()
        {
            var result = "";
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890";
            var rng = new Random();
            for (var i = 0; i < 32; i++)
            {
                result += chars[rng.Next(0, chars.Length)];
            }

            return result;
        }

        [HttpPost("logout")]
        public async Task<ActionResult> PostLogout()
        {
            var token = Request.Cookies["SessionToken"];
            if (token == null)
            {
                return BadRequest(new {error = "Not logged in"});
            }
            
            var session = await dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
            if (session == null)
            {
                return BadRequest(new {error = "Invalid token"});
            }

            var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == session.Username);
            if (user == null)
            {
                return BadRequest(new {error = "User not found"});
            }

            dbContext.Remove(session);
            await dbContext.SaveChangesAsync();

            Response.Cookies.Delete("SessionToken");
            return Ok();
        }
    }
}