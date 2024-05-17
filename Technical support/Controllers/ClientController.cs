using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Technical_support.ViewModel;
using Microsoft.EntityFrameworkCore;
using Technical_support.Data;
using System.Runtime.ConstrainedExecution;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Technical_support.Controllers
{
    public class ClientController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ClientController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles = "Client")]
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(Roles = "Client,Manager,Admin")]
        public async Task<IActionResult> Profile(string id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            DataUser dataUser = new();
            dataUser.Id = id;
            dataUser.Name = user.UserName;
            if (user.PhoneNumber != null)
                dataUser.Phone = user.PhoneNumber;
            else
                dataUser.Phone = "Не указан";
            dataUser.Email = user.Email;
            return View(dataUser);
        }

        [Authorize(Roles = "Client,Manager,Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile([Bind("Id,Name,Phone,Email")] DataUser dataUser)
        {
            var userDuplicate = _context.Users
               .Where(c => c.NormalizedUserName == dataUser.Email.ToUpper());
            string dublicateId = "";
            foreach (var item in userDuplicate)
            {
                dublicateId = item.Id;
            }
                if (userDuplicate == null || dublicateId == dataUser.Id)
            {
                var user = await _context.Users.FindAsync(dataUser.Id);

                if (dataUser.Id != user.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        user.UserName = dataUser.Name;
                        user.PhoneNumber = dataUser.Phone;
                        user.Email = dataUser.Email;
                        user.NormalizedUserName = dataUser.Email.ToUpper();
                        user.NormalizedEmail = dataUser.Email.ToUpper();
                        _context.Update(user);
                        var result = await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ResponseExists(user.Id))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("Profile", new RouteValueDictionary(new
                    {
                        Controller = "Client",
                        Action = "Profile",
                        id = user.Id
                    }));
                }
                return View(dataUser);
            }
            else 
            {
                ModelState.AddModelError(string.Empty, "Такой E-mail уже существует");
                var user = await _context.Users.FindAsync(dataUser.Id);
                dataUser.Name = user.UserName;
                dataUser.Phone = user.PhoneNumber;
                dataUser.Email = user.Email;
                return View(dataUser);
            }
            

        }
        private bool ResponseExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }

}
