using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Technical_support.Data;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Technical_support.ViewModel;

namespace Technical_support.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private IPasswordHasher<IdentityUser> passwordHasher;

        public UserController(UserManager<IdentityUser> userMrg, IPasswordHasher<IdentityUser> passwordHash)
        {
            userManager = userMrg;
            passwordHasher = passwordHash;
        }

        public IActionResult Index()
        {
            return View(userManager.Users);
        }

        public IActionResult Create() => View();

        private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        // Метод для создания нового пользователя
        [HttpPost]
        public async Task<IActionResult> Create(CreateUser model)
        {
            if (ModelState.IsValid)
            {
                IdentityUser user = new IdentityUser
                {
                    UserName = model.Name,
                    Email = model.Email,
                    NormalizedUserName = model.Email.ToUpper(),
                    EmailConfirmed = true
                };
                IdentityResult result = await userManager.CreateAsync(user, model.Password);
                string roleName = "Client";
                IdentityResult result1 = await userManager.AddToRoleAsync(user, roleName);
                if (result.Succeeded && result1.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    Errors(result);
                }
            }
            return View(model);
        }

        public IActionResult Update(string email)
        {
            return View();
        }

        // Метод для обновления данных пользователя
        [HttpPost]
        public async Task<IActionResult> Update(string id, string email, string password)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                if (!string.IsNullOrEmpty(email))
                    user.Email = email;
                else
                    ModelState.AddModelError("", "Email cannot be empty");

                if (!string.IsNullOrEmpty(password))
                    user.PasswordHash = passwordHasher.HashPassword(user, password);
                else
                    ModelState.AddModelError("", "Password cannot be empty");

                if (!string.IsNullOrEmpty(email) && !string.IsNullOrEmpty(password))
                {
                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        return RedirectToAction("Index");
                    else
                        Errors(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        // Метод для удаления пользователя
        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View("Index", userManager.Users);
        }
    }
}
