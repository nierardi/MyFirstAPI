using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;

namespace MyFirstAPI.Models
{
    public class User
    {
        public int ID { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }

        [ForeignKey("Permission")]
        public List<Permission> Permissions { get; set; }

        [ForeignKey("Session")]
        public List<Session> Sessions { get; set; }
    }
}