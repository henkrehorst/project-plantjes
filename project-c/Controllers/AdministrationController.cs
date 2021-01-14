using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using project_c.Models.Plants;
using project_c.Models.Users;
using project_c.ViewModels;
using SQLitePCL;

namespace project_c.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdministrationController : Controller
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly DataContext _context;

        public AdministrationController(RoleManager<IdentityRole> roleManager, UserManager<User> userManager,
            DataContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public IActionResult ListUsers()
        {
            var users = _userManager.Users;
            var orderedUsers = users.OrderBy(u => u.Id);
            ViewBag.userIsEdited = TempData["userIsEdited"] == null ? false : TempData["userIsEdited"];
            ViewBag.userIsDeleted = TempData["userIsDeleted"] == null ? false : TempData["userIsDeleted"];
            return View(orderedUsers);
        }
        
        [HttpGet]
        public async Task<IActionResult> ListReports()
        {
            var reports = await _context.Reports.Include(r => r.User).Include(r => r.Plant).ToListAsync();

            UserReportViewModel model = new UserReportViewModel();
            model.Report = reports;

            return View(reports);
        }

        [HttpPost]
        public IActionResult DeleteReport(int id)
        {
            try 
            { 
                var report = _context.Reports.Find(id);
                _context.Reports.Remove(report);
                _context.SaveChanges();
                return RedirectToAction("ListReports");
            }
            catch
            {
                return RedirectToAction("ListReports");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return View("NotFound");
            }

            // GetClaimsAsync retunrs the list of user Claims
            var userClaims = await _userManager.GetClaimsAsync(user);
            // GetRolesAsync returns the list of user Roles
            var userRoles = await _userManager.GetRolesAsync(user);

            if (userRoles != null)
            {
                var model = new EditUserViewModel
                {
                    Id = user.Id,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Zipcode = user.ZipCode,
                    Email = user.Email,
                    UserName = user.UserName,
                    Claims = userClaims.Select(c => c.Value).ToList(),
                    Roles = userRoles
                };

                return View(model);
            }

            return Content("Error");
        }

        [HttpPost]
        public async Task<IActionResult> EditUser(EditUserViewModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {model.Id} cannot be found";
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.ZipCode = model.Zipcode;
            user.Email = model.Email;
            user.UserName = model.UserName;

            var result = await _userManager.UpdateAsync(user);

            if (result.Succeeded)
            {
                TempData["userIsEdited"] = true;
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                
                ModelState.AddModelError("", error.Description);
            }

            return View(model);
        }

        public IActionResult ApprovePlants()
        {
            var plants = from p in _context.Plants orderby p.PlantId descending select p;
            return View(plants);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var plants = from p in _context.Plants where p.UserId == user.Id select p;
            var roles = from r in _context.IdentityUserRoles where r.UserId == user.Id select r;
            
            
            if (user == null)
            {
                ViewBag.ErrorMessage = $"User with Id = {id} cannot be found";
                return NotFound();
            }

            if (!await _userManager.IsInRoleAsync(user, "Admin"))
            {
                _context.Plants.RemoveRange(plants);
                _context.IdentityUserRoles.RemoveRange(roles);
            }
            else
            {
                return Content("Cannot remove Admin");
            }

            await _context.SaveChangesAsync();
            
            var result = await _userManager.DeleteAsync(user);

            if (result.Succeeded)
            {
                TempData["userIsDeleted"] = true;
                return RedirectToAction("ListUsers");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            return View("ListUsers");
        }

        [HttpGet]
        public IActionResult CreateRole()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRole(CreateRoleViewModel model)
        {
            if (ModelState.IsValid)
            {
                // We just need to specify a unique role name to create a new role
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };

                // Saves the role in the underlying AspNetRoles table
                IdentityResult result = await _roleManager.CreateAsync(identityRole);

                if (result.Succeeded)
                {
                    TempData["isCreated"] = true;
                    return RedirectToAction("ListRoles", "Administration");
                }

                foreach (IdentityError error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
        
            return View(model);
        }

        [HttpGet]
        public IActionResult ListRoles()
        {
            var roles = _roleManager.Roles;
            ViewBag.isCreated = TempData["isCreated"] == null ? false : TempData["isCreated"];
            ViewBag.isDeleted = TempData["isDeleted"] == null ? false : TempData["isDeleted"];
            ViewBag.isEdited = TempData["isEdited"] == null ? false : TempData["isEdited"];
            return View(roles);
        }

        [HttpGet]
        public async Task<IActionResult> EditRole(string id)
        {
            // Find the role by Role ID
            var role = await _roleManager.FindByIdAsync(id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return NotFound();
            }

            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };

            // Retrieve all the Users
            foreach (var user in _userManager.Users.ToList())
            {
                // If the user is in this role, add the username to
                // Users property of EditRoleViewModel. This model
                // object is then passed to the view for display
                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    model.Users.Add(user.UserName);
                }
            }
            ViewBag.roleIsEdited = TempData["roleIsEdited"] == null ? false : TempData["roleIsEdited"];
            return View(model);
        }

        // This action responds to HttpPost and receives EditRoleViewModel
        [HttpPost]
        public async Task<IActionResult> EditRole(EditRoleViewModel model)
        {
            var role = await _roleManager.FindByIdAsync(model.Id);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {model.Id} cannot be found";
                return NotFound();
            }

            else
            {
                role.Name = model.RoleName;

                // Update the Role using UpdateAsync
                var result = await _roleManager.UpdateAsync(role);

                if (result.Succeeded)
                {
                    TempData["isEdited"] = true;
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            var role = await _roleManager.FindByIdAsync(id);

            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {id} cannot be found";
                return NotFound();
            }
            else
            {
                var result = await _roleManager.DeleteAsync(role);

                if (result.Succeeded)
                {
                    TempData["isDeleted"] = true;
                    return RedirectToAction("ListRoles");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

                return View("ListRoles");
            }
        }

        [HttpGet]
        public async Task<IActionResult> EditUsersInRole(string roleId)
        {
            ViewBag.roleId = roleId;

            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return NotFound();
            }

            var model = new List<UserRoleViewModel>();
            foreach (var user in _userManager.Users.ToList())
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName
                };

                if (await _userManager.IsInRoleAsync(user, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                {
                    userRoleViewModel.IsSelected = false;
                }

                model.Add(userRoleViewModel);
            }
            
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> EditUsersInRole(List<UserRoleViewModel> model, string roleId)
        {
            var role = await _roleManager.FindByIdAsync(roleId);
            if (role == null)
            {
                ViewBag.ErrorMessage = $"Role with Id = {roleId} cannot be found";
                return NotFound();
            }

            for (int i = 0;
                i < model.Count;
                i++)
            {
                var user = await _userManager.FindByIdAsync(model[i].UserId);

                IdentityResult result = null;

                if (model[i].IsSelected && !(await _userManager.IsInRoleAsync(user, role.Name)))
                {
                    result = await _userManager.AddToRoleAsync(user, role.Name);
                }
                else if (!model[i].IsSelected && await _userManager.IsInRoleAsync(user, role.Name))
                {
                    result = await _userManager.RemoveFromRoleAsync(user, role.Name);
                }
                else
                {
                    continue;
                }

                if (result.Succeeded)
                {
                    if (i < (model.Count - 1))
                        continue;
                    
                    TempData["roleIsEdited"] = true;
                    return RedirectToAction("EditRole", new {Id = roleId});
                }
            }
            
            TempData["roleIsEdited"] = true;
            return RedirectToAction("EditRole", new
            {
                Id = roleId
            });
        }
    }
}