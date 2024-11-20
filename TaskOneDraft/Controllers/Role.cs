using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace TaskOneDraft.Controllers
{
    [Authorize(Roles = "Admin")]  // Only Admins can access this controller
    public class RoleController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;

        public RoleController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // Display existing roles
        public IActionResult Index()
        {
            var roles = _roleManager.Roles;
            return View(roles);
        }

        // Display a form to create a new role
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string roleName)
        {
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Error while creating role");
                }
            }
            return View(roleName);
        }
    }

}
