using DangoAPI.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DangoAPI.Data
{
    public class Seed
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;

        public Seed(UserManager<User> userManager,RoleManager<Role> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public void SeedUsers() {
            if (!_userManager.Users.Any())
            {
                string userData = File.ReadAllText("Data/UserSeedData.json");
                List<User> users = JsonConvert.DeserializeObject<List<User>>(userData);

                List<Role> roles = new List<Role> {
                    new Role{ Name = "Member"},
                    new Role{ Name = "Admin"},
                    new Role{ Name = "Moderator"},
                    new Role{ Name = "VIP"}
                };

                foreach (Role role in roles)
                {
                    _roleManager.CreateAsync(role).Wait();
                }
                foreach (User user in users)
                {
                    //custom authorization
                    //Byte[] passwordHash, passwordSalt;
                    //CreatePasswordHash("password", out passwordHash, out passwordSalt);

                    //user.PasswordHash = passwordHash;
                    //user.PasswordSalt = passwordSalt;

                    //authorization using .net Identity
                    _userManager.CreateAsync(user,"password").Wait();
                    _userManager.AddToRoleAsync(user, "Member").Wait();
                }

                // create a admin user
                User adminUser = new User
                {
                    UserName = "Admin"
                };

                IdentityResult result = _userManager.CreateAsync(adminUser, "password").Result;

                if (result.Succeeded) {
                    User admin = _userManager.FindByNameAsync("Admin").Result;
                    _userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" }).Wait();
                }
            }
        }

        //private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        //{
        //    using (System.Security.Cryptography.HMACSHA512 hmac = new System.Security.Cryptography.HMACSHA512())
        //    {
        //        passwordSalt = hmac.Key;
        //        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        //    }
        //}
    }
}
