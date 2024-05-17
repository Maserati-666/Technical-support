using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Data;
using Technical_support.Data;
using Technical_support.Models;
using Technical_support.ViewModel;

namespace Technical_support.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> userManager;
        private RoleManager<IdentityRole> roleManager;
        private readonly ApplicationDbContext _context;
        private IPasswordHasher<IdentityUser> passwordHasher;

        public AdminController(UserManager<IdentityUser> userMrg, IPasswordHasher<IdentityUser> passwordHash, ApplicationDbContext context, RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMrg;
            passwordHasher = passwordHash;
            _context = context;
            roleManager = roleMgr;
        }

        // Метод загрузки главной страницы GET
        public ActionResult Index()
        {
            DataAdminIndex newData = new DataAdminIndex();
            VidRequest listVidRequest = new VidRequest();
            TypeUser listTypeUser = new TypeUser();
            listVidRequest.RequestOpen = _context.Request.Where(c => c.StatusRequestId == 1).Count();
            listVidRequest.RequestProg = _context.Request.Where(c => c.StatusRequestId == 2).Count(); 
            listVidRequest.RequestClose = _context.Request.Where(c => c.StatusRequestId == 3).Count();
            listVidRequest.RequestAnnul = _context.Request.Where(c => c.StatusRequestId == 4).Count();
            foreach (var item in _context.Roles)
            {
                var role1 = from role in _context.Roles
                                       .Where(c => c.Name == item.Name)
                            join userRole in _context.UserRoles on role.Id equals userRole.RoleId
                            select new
                            {
                                role.Name
                            };
                listTypeUser.Client = role1.Count();
                if (item.Name == "Client")
                {
                    listTypeUser.Client = role1.Count();
                }
                else if (item.Name == "Manager")
                {
                    listTypeUser.Manager = role1.Count();
                }
                else if (item.Name == "Admin")
                {
                    listTypeUser.Admin = role1.Count();
                }
            }
            newData.ListVidRequest = listVidRequest;
            newData.ListTypeUser = listTypeUser;
            List<SelectListItem> period = new List<SelectListItem>();
            period.Add(item: new SelectListItem { Value = "1", Text = "За весь период" });
            period.Add(item: new SelectListItem { Value = "2", Text = "За последний день" });
            period.Add(item: new SelectListItem { Value = "3", Text = "За последнюю неделю" });
            period.Add(item: new SelectListItem { Value = "4", Text = "За последний месяц" });
            ViewData["Period"] = period;
            return View(newData);
        }


        // Метод загрузки главной страницы POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string period)
        {
            DataAdminIndex newData = new DataAdminIndex();
            VidRequest listVidRequest = new VidRequest();
            TypeUser listTypeUser = new TypeUser();
            if (period == "1")
            {
                listVidRequest.RequestOpen = _context.Request.Where(c => c.StatusRequestId == 1).Count();
                listVidRequest.RequestProg = _context.Request.Where(c => c.StatusRequestId == 2).Count();
                listVidRequest.RequestClose = _context.Request.Where(c => c.StatusRequestId == 3).Count();
                listVidRequest.RequestAnnul = _context.Request.Where(c => c.StatusRequestId == 4).Count();
            }
            else 
            {
                DateTime per = DateTime.Now;
                if (period == "2")
                    per = DateTime.Today;
                else if (period == "3")
                    per = DateTime.Today.AddDays(-7);
                else if (period == "4")
                    per = DateTime.Today.AddMonths(-1);
                listVidRequest.RequestOpen = _context.Request.Where(c => c.StatusRequestId == 1 && c.DateOpen >= per).Count();
                listVidRequest.RequestProg = _context.Request.Where(c => c.StatusRequestId == 2 && c.DateOpen >= per).Count();
                listVidRequest.RequestClose = _context.Request.Where(c => c.StatusRequestId == 3 && c.DateOpen >= per).Count();
                listVidRequest.RequestAnnul = _context.Request.Where(c => c.StatusRequestId == 4 && c.DateOpen >= per).Count();
            }
            foreach (var item in _context.Roles)
            {
                var role1 = from role in _context.Roles
                                       .Where(c => c.Name == item.Name)
                            join userRole in _context.UserRoles on role.Id equals userRole.RoleId
                            select new
                            {
                                role.Name
                            };
                listTypeUser.Client = role1.Count();
                if (item.Name == "Client")
                {
                    listTypeUser.Client = role1.Count();
                }
                else if (item.Name == "Manager")
                {
                    listTypeUser.Manager = role1.Count();
                }
                else if (item.Name == "Admin")
                {
                    listTypeUser.Admin = role1.Count();
                }
            }
            newData.ListVidRequest = listVidRequest;
            newData.ListTypeUser = listTypeUser;
            List<SelectListItem> periods = new List<SelectListItem>();
            periods.Add(item: new SelectListItem { Value = "1", Text = "За весь период" });
            periods.Add(item: new SelectListItem { Value = "2", Text = "За последний день" });
            periods.Add(item: new SelectListItem { Value = "3", Text = "За последнюю неделю" });
            periods.Add(item: new SelectListItem { Value = "4", Text = "За последний месяц" });
            ViewData["Period"] = new SelectList(periods, "Value", "Text", period);
            return View(newData);
        }

            private void Errors(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
                ModelState.AddModelError("", error.Description);
        }

        // Метод для создания нового пользователя
        public IActionResult CreateUser()
        {
            var roles = _context.Roles;
            ViewData["RoleId"] = new SelectList(roles, "Name");
            return View();
        }

        // Метод для создания нового пользователя POST
        [HttpPost]
        public async Task<IActionResult> CreateUser(CreateUser model)
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
                string roleName = model.RoleId;
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
            var roles = _context.Roles;
            ViewData["RoleId"] = new SelectList(roles, "Name");
            return View(model);
        }

        // Метод для отображения учетных записей
        public IActionResult Accounts()
        {
            return View(userManager.Users);
        }

        // Метод для обновления данных пользователя
        public async Task<IActionResult> UpdateUser(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                UpdateUser upUser = new();
                upUser.Id = user.Id;
                upUser.Email = user.Email;
                upUser.UserName = user.UserName;
                upUser.Phone = user.PhoneNumber;
                return View(upUser);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        // Метод для обновления данных пользователя POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser(string id, [Bind("Id,UserName,Email,Password,Phone")] UpdateUser upUser)
        {
            var userDuplicate = _context.Users
                  .Where(c => c.NormalizedUserName == upUser.Email.ToUpper()).FirstOrDefault();
            //foreach (var item in userDuplicate)
            //{
            //    dublicateId = item.Id;
            //}
            if (userDuplicate == null || userDuplicate.Id == upUser.Id)
            {
                var user = await _context.Users.FindAsync(upUser.Id);

                if (upUser.Id != user.Id)
                {
                    return NotFound();
                }
                if (ModelState.IsValid)
                {
                    try
                    {
                        user.UserName = upUser.UserName;
                        user.PhoneNumber = upUser.Phone;
                        user.Email = upUser.Email;
                        user.NormalizedUserName = upUser.Email.ToUpper();
                        user.NormalizedEmail = upUser.Email.ToUpper();
                        if (!string.IsNullOrEmpty(upUser.Password))
                            user.PasswordHash = passwordHasher.HashPassword(user, upUser.Password);
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
                    return RedirectToAction("Accounts");
                }
                return View(upUser);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Такой E-mail уже существует");
                var user = await _context.Users.FindAsync(upUser.Id);
                upUser.UserName = user.UserName;
                upUser.Phone = user.PhoneNumber;
                upUser.Email = user.Email;
                return View(upUser);
            }
        }

        // Метод для удаления пользователя
        public async Task<IActionResult> DeleteUser(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            UpdateUser upUser = new();
            if (user != null)
            {
                upUser.Id = user.Id;
                upUser.Email = user.Email;
                upUser.UserName = user.UserName;
                upUser.Phone = user.PhoneNumber;
                return View(upUser);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(upUser);
        }

        // Метод для удаления пользователя POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserConfirmed(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await userManager.DeleteAsync(user);
                if (result.Succeeded)
                    return RedirectToAction("Accounts");
                else
                    Errors(result);
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return RedirectToAction("Accounts");
        }

        private bool ResponseExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        // Метод для отображения учетных записей с ролями
        public IActionResult AccountsRoles()
        {
            List<AccountsRoles> accountsRoles = new();
            var usersRoles = from userRole in _context.UserRoles
                             join users in _context.Users on userRole.UserId equals users.Id
                             join roles in _context.Roles on userRole.RoleId equals roles.Id
                             select new
                             {
                                 users.Id,
                                 users.UserName,
                                 users.Email,
                                 users.PhoneNumber,
                                 roles.Name,
                             };
            foreach (var item in usersRoles) 
            {
                accountsRoles.Add(new AccountsRoles 
                {
                    Id = item.Id,
                    UserName = item.UserName,
                    Email = item.Email,
                    Phone = item.PhoneNumber,
                    Role = item.Name
                });
            }
            return View(accountsRoles);
        }


        // Метод для создания новой роли
        public IActionResult CreateRole()
        { 
            return View();
        }

        // Метод для создания новой роли POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateRole([Required] string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await roleManager.CreateAsync(new IdentityRole(name));
                if (result.Succeeded)
                    return RedirectToAction("Index");
                else
                    Errors(result);
            }
            return View(name);
        }

        // Метод для обновления роли пользователя
        public async Task<IActionResult> UpdateRole(string id)
        {
            IdentityUser user = await userManager.FindByIdAsync(id);
            AccountsRoles upUser = new();
            IList<string> roles = await userManager.GetRolesAsync(user);
            string oldRole = roles[0];
            if (user != null)
            {
                upUser.Id = user.Id;
                upUser.Email = user.Email;
                upUser.UserName = user.UserName;
                upUser.Phone = user.PhoneNumber;
                upUser.Role = oldRole;
                var listroles = _context.Roles;
                ViewData["Role"] = new SelectList(listroles, "Name");
                return View(upUser);
            }
            else
            {
                ModelState.AddModelError("", "User Not Found");
                ViewData["Role"] = new SelectList(_context.Roles, "Name");
                return View(upUser);
            }
        }

        // Метод для обновления роли пользователя POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateRole(AccountsRoles upUser)
        {
                IdentityUser user = await userManager.FindByIdAsync(upUser.Id);
                IList<string> roles = await userManager.GetRolesAsync(user);
                string oldRole = roles[0];
                IdentityRole role = await roleManager.FindByNameAsync(upUser.Role);
                string newRole = role.Name;
                if (oldRole != newRole)
                {
                    IdentityResult result = await userManager.RemoveFromRoleAsync(user, oldRole);
                    if (result.Succeeded)
                    {
                        result = await userManager.AddToRoleAsync(user, newRole);
                        if (!result.Succeeded)
                            Errors(result);
                        return RedirectToAction("AccountsRoles");
                }
                    else
                        Errors(result);
                }
            return RedirectToAction("AccountsRoles");
        }

        
        // Метод для отображения общего списка заявок GET
        public IActionResult ListRequests()
        {
            var requests = from request in _context.Request
                           join user in _context.Users on request.UserId equals user.Id
                           join theme in _context.ThemeRequests on request.ThemeId equals theme.ThemeId
                           join prioritet in _context.Prioritets on request.PrioritetId equals prioritet.PrioritetId
                           join status in _context.StatusRequests on request.StatusRequestId equals status.StatusRequestId
                           select new
                           {
                               request.RequestId,
                               user.UserName,
                               theme.NameTheme,
                               prioritet.PrioritetRequest,
                               request.TextRequest,
                               request.ManagerId,
                               request.DateOpen,
                               request.DateClose,
                               status.Status,
                           };
            List<RequestAdminList> adminList = new();
            foreach (var item in requests)
            {
                adminList.Add(new RequestAdminList
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    ManagerName = item.ManagerId,
                    DateOpen = item.DateOpen,
                    DateClose = item.DateClose,
                    Status = item.Status
                });
            }
            foreach (var item in adminList)
            {
                foreach (var item1 in _context.Users)
                {
                    if (item1.Id == item.ManagerName)
                    {
                        item.ManagerName = item1.UserName;
                    }
                }
            }
            var statusReq = from status in _context.StatusRequests
                         select new
                         {
                             status.Status
                         };
            List<string> statusRequest = new();
            statusRequest.Add("Все");
            foreach (var item in statusReq) 
            {
                statusRequest.Add(item.Status);
            }
            List<SelectListItem> statusR = new List<SelectListItem>();
            foreach (var item in statusRequest)
            {
                statusR.Add(new SelectListItem { Value = item, Text = item });
            }
            ViewData["StatusRequestId"] = statusR;
            return View(adminList);
        }


        // Метод для отображения общего списка заявок POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ListRequests(string statusRequest)
        {
            int stat = 0;
            if (statusRequest == "Открыта")
                stat = 1;
            else if (statusRequest == "В работе")
                stat = 2;
            else if (statusRequest == "Исполнена")
                stat = 3;
            else if (statusRequest == "Аннулирована")
                stat = 4;
            else if (statusRequest == "Все")
                stat = 5;
            List<RequestAdminList> adminList = new();
            if (stat == 5)
            {
                var requests = from request in _context.Request
                               join user in _context.Users on request.UserId equals user.Id
                               join theme in _context.ThemeRequests on request.ThemeId equals theme.ThemeId
                               join prioritet in _context.Prioritets on request.PrioritetId equals prioritet.PrioritetId
                               join status in _context.StatusRequests on request.StatusRequestId equals status.StatusRequestId
                               select new
                               {
                                   request.RequestId,
                                   user.UserName,
                                   theme.NameTheme,
                                   prioritet.PrioritetRequest,
                                   request.TextRequest,
                                   request.ManagerId,
                                   request.DateOpen,
                                   request.DateClose,
                                   status.Status,
                               };
                foreach (var item in requests)
                {
                    adminList.Add(new RequestAdminList
                    {
                        RequestId = item.RequestId,
                        UserName = item.UserName,
                        NameTheme = item.NameTheme,
                        Prioritet = item.PrioritetRequest,
                        TextRequest = item.TextRequest,
                        ManagerName = item.ManagerId,
                        DateOpen = item.DateOpen,
                        DateClose = item.DateClose,
                        Status = item.Status
                    });
                }
            }
            else 
            {
                var requests = from request in _context.Request
                               .Where(c => c.StatusRequestId == stat)
                               join user in _context.Users on request.UserId equals user.Id
                               join theme in _context.ThemeRequests on request.ThemeId equals theme.ThemeId
                               join prioritet in _context.Prioritets on request.PrioritetId equals prioritet.PrioritetId
                               join status in _context.StatusRequests on request.StatusRequestId equals status.StatusRequestId
                               select new
                               {
                                   request.RequestId,
                                   user.UserName,
                                   theme.NameTheme,
                                   prioritet.PrioritetRequest,
                                   request.TextRequest,
                                   request.ManagerId,
                                   request.DateOpen,
                                   request.DateClose,
                                   status.Status,
                               };
                foreach (var item in requests)
                {
                    adminList.Add(new RequestAdminList
                    {
                        RequestId = item.RequestId,
                        UserName = item.UserName,
                        NameTheme = item.NameTheme,
                        Prioritet = item.PrioritetRequest,
                        TextRequest = item.TextRequest,
                        ManagerName = item.ManagerId,
                        DateOpen = item.DateOpen,
                        DateClose = item.DateClose,
                        Status = item.Status
                    });
                }
            }
           
            foreach (var item in adminList)
            {
                foreach (var item1 in _context.Users)
                {
                    if (item1.Id == item.ManagerName)
                    {
                        item.ManagerName = item1.UserName;
                    }
                }
            }
            var statusReq = from status in _context.StatusRequests
                            select new
                            {
                                status.Status
                            };
            List<string> statusRequests = new();
            statusRequests.Add("Все");
            foreach (var item in statusReq)
            {
                statusRequests.Add(item.Status);
            }
            List<SelectListItem> statusR = new List<SelectListItem>();
            foreach (var item in statusRequests)
            {
                statusR.Add(new SelectListItem { Value = item, Text = item });
            }
            ViewData["StatusRequestId"] = new SelectList(statusR, "Value", "Text", statusRequest);
            return View(adminList);
        }

        // Метод для отображения открытых заявок
        public IActionResult ListOpenRequests()
        {
            var requests = from request in _context.Request
                           .Where(c => c.StatusRequestId == 1)
                           join user in _context.Users on request.UserId equals user.Id
                           join manager in _context.Users on request.ManagerId equals manager.Id
                           join theme in _context.ThemeRequests on request.ThemeId equals theme.ThemeId
                           join prioritet in _context.Prioritets on request.PrioritetId equals prioritet.PrioritetId
                           join status in _context.StatusRequests on request.StatusRequestId equals status.StatusRequestId
                           select new
                           {
                               request.RequestId,
                               user.UserName,
                               theme.NameTheme,
                               prioritet.PrioritetRequest,
                               request.TextRequest,
                               manager.Email,
                               request.DateOpen,
                               request.DateClose,
                               status.Status,
                           };
            List<RequestAdminList> adminList = new();
            foreach (var item in requests)
            {
                adminList.Add(new RequestAdminList
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    ManagerName = item.Email,
                    DateOpen = item.DateOpen,
                    DateClose = item.DateClose,
                    Status = item.Status
                });
            }
            foreach (var item in adminList)
            {
                foreach (var item1 in _context.Users)
                {
                    if (item1.Email == item.ManagerName)
                    {
                        item.ManagerName = item1.UserName;
                    }
                }
            }
            return View(adminList);
        }


        // Метод для отображения выполненных заявок
        public IActionResult ListCloseRequests()
        {
            var requests = from request in _context.Request
                           .Where(c => c.StatusRequestId == 3)
                           join user in _context.Users on request.UserId equals user.Id
                           join manager in _context.Users on request.ManagerId equals manager.Id
                           join theme in _context.ThemeRequests on request.ThemeId equals theme.ThemeId
                           join prioritet in _context.Prioritets on request.PrioritetId equals prioritet.PrioritetId
                           join status in _context.StatusRequests on request.StatusRequestId equals status.StatusRequestId
                           select new
                           {
                               request.RequestId,
                               user.UserName,
                               theme.NameTheme,
                               prioritet.PrioritetRequest,
                               request.TextRequest,
                               manager.Email,
                               request.DateOpen,
                               request.DateClose,
                               status.Status,
                           };
            List<RequestAdminList> adminList = new();
            foreach (var item in requests)
            {
                adminList.Add(new RequestAdminList
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    ManagerName = item.Email,
                    DateOpen = item.DateOpen,
                    DateClose = item.DateClose,
                    Status = item.Status
                });
            }
            foreach (var item in adminList)
            {
                foreach (var item1 in _context.Users)
                {
                    if (item1.Email == item.ManagerName)
                    {
                        item.ManagerName = item1.UserName;
                    }
                }
            }
            return View(adminList);
        }


        // Метод для отображения заявок в работе
        public IActionResult ListProgressRequests()
        {
            var requests = from request in _context.Request
                           .Where(c => c.StatusRequestId == 2)
                           join user in _context.Users on request.UserId equals user.Id
                           join manager in _context.Users on request.ManagerId equals manager.Id
                           join theme in _context.ThemeRequests on request.ThemeId equals theme.ThemeId
                           join prioritet in _context.Prioritets on request.PrioritetId equals prioritet.PrioritetId
                           join status in _context.StatusRequests on request.StatusRequestId equals status.StatusRequestId
                           select new
                           {
                               request.RequestId,
                               user.UserName,
                               theme.NameTheme,
                               prioritet.PrioritetRequest,
                               request.TextRequest,
                               manager.Email,
                               request.DateOpen,
                               request.DateClose,
                               status.Status,
                           };
            List<RequestAdminList> adminList = new();
            foreach (var item in requests)
            {
                adminList.Add(new RequestAdminList
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    ManagerName = item.Email,
                    DateOpen = item.DateOpen,
                    DateClose = item.DateClose,
                    Status = item.Status
                });
            }
            foreach (var item in adminList)
            {
                foreach (var item1 in _context.Users)
                {
                    if (item1.Email == item.ManagerName)
                    {
                        item.ManagerName = item1.UserName;
                    }
                }
            }
            return View(adminList);
        }
    }
}
