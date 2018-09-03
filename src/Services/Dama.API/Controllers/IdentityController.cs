using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Dama.API.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    public class IdentityController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        public IdentityController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        [HttpGet]
        public IActionResult Get()
        {
            return new JsonResult(from c in User.Claims select new { c.Type, c.Value });
        }

        [HttpGet]
        [Route("getstaffusers")]
        public async Task<IActionResult> GetStaffUsers()
        {
            var users = await _userManager.GetUsersInRoleAsync("Staff");

            List<UsersViewModel> model = users.Select(x => new UsersViewModel
            {
                UserId = x.Id,
                Email = x.Email
            }).ToList();
            return Ok(model);
        }

        [HttpGet]
        [Route("{userid}")]
        public async Task<IActionResult> GetUserInfo(string userid)
        {
            var user = (await _userManager.GetUsersInRoleAsync("Staff")).SingleOrDefault(x => x.Id == userid);

            if (user == null)
                return NotFound();

            UsersViewModel model = new UsersViewModel
            {
                UserId = user.Id,
                Email = user.Email,
                Name = user.FirstName,
                LastName = user.LastName
            };
            return Ok(model);
        }

        [HttpGet]
        [Route("login/{username}/{password}")]
        public async Task<IActionResult> LoginStaffAsync(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user != null)
            {
                var base64EncodedBytes = System.Convert.FromBase64String(password);
                var passwordText =  System.Text.Encoding.UTF8.GetString(base64EncodedBytes);

                if (await _userManager.CheckPasswordAsync(user, passwordText))
                {
                    UsersViewModel model = new UsersViewModel
                    {
                        UserId = user.Id,
                        Email = user.Email,
                        Name = user.FirstName,
                        LastName = user.LastName
                    };
                    return Ok(model);
                }
                    
            }
            return new EmptyResult();
        }

        public class UsersViewModel
        {
            public string UserId { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public string LastName { get; set; }
        }
    }
}