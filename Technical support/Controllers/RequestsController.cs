using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Technical_support.Data;
using Technical_support.Models;
using Technical_support.ViewModel;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Routing.Constraints;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Azure.Core;
using Request = Technical_support.Models.Request;

namespace Technical_support.Controllers
{
    public class RequestsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public RequestsController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _environment = hostEnvironment;
        }

        // Метод для получения заявок пользователя
        public IActionResult MyRequests(string id)
        {
            List<MyRequests> myList = new();
            foreach (var item in _context.Request)
            {
                string theme = "";
                string? managerName = "";
                string status = "";
                string prioritet = "";
                if (item.UserId == id)
                {
                    foreach (var item1 in _context.ThemeRequests)
                    {
                        if (item1.ThemeId == item.ThemeId)
                            theme = item1.NameTheme;
                    }
                    foreach (var item2 in _context.StatusRequests)
                    {
                        if (item2.StatusRequestId == item.StatusRequestId)
                            status = item2.Status;
                    }
                    foreach (var item3 in _context.Users)
                    {
                        if (item3.Id == item.ManagerId)
                            managerName = item3.UserName;
                    }
                    foreach (var item4 in _context.Prioritets)
                    {
                        if (item4.PrioritetId == item.PrioritetId)
                            prioritet = item4.PrioritetRequest;
                    }
                    myList.Add(new MyRequests
                    {
                        RequestId = item.RequestId,
                        Theme = theme,
                        Prioritet = prioritet,
                        TextRequest = item.TextRequest,
                        DateOpen = item.DateOpen,
                        ManagerName = managerName,
                        DateClose = item.DateClose,
                        StatusRequest = status,
                    });
                }
            }
            return View(myList);
        }


        //Метод для просмотра выбранной заявки
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Request == null)
            {
                return NotFound();
            }

            var request = await _context.Request
                .Include(r => r.Manager)
                .Include(r => r.User)
                .Include(r => r.StatusRequest)
                .Include(r => r.Theme)
                 .Include(r => r.Prioritet)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }
            List<Response> responses = _context.Responses
                           .Where(c => c.RequestId == id)
                           .ToList();
            RequestDetail requestDetail = new RequestDetail();
            requestDetail.Request = request;
            requestDetail.ListResponses = responses;
            return View(requestDetail);
        }

        // Метод для создания заявки GET
        public IActionResult Create()
        {
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "StatusRequestId", "Status");
            ViewData["ThemeId"] = new SelectList(_context.ThemeRequests, "ThemeId", "DescriptionTheme");
            ViewData["PrioritetId"] = new SelectList(_context.Prioritets, "PrioritetId", "PrioritetRequest");
            return View();
        }

        // Метод для создания заявки POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RequestId,UserId,ThemeId,PrioritetId,TextRequest,Image,DateOpen,ManagerId,DateClose,StatusRequestId")] Request request)
        {
            string filename = "";
            if (request.Image != null)
            {
                string uploadfolder = Path.Combine(_environment.WebRootPath, "images");
                filename = Guid.NewGuid().ToString() + "_" + request.Image.FileName;
                string filepath = Path.Combine(uploadfolder, filename);
                request.Image.CopyTo(new FileStream(filepath, FileMode.Create));
                byte[]? FileContent = null;
                if (request.Image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        request.Image.CopyTo(ms);
                        FileContent = ms.ToArray();
                    }
                }
                request.File = FileContent;
            }
            string userId = request.UserId;
            request.DateOpen = DateTime.Now;
            ModelState.Remove("image");
            ModelState.Remove("DateOpen");
            ModelState.Remove("Manager");
            ModelState.Remove("User");
            ModelState.Remove("StatusRequest");
            ModelState.Remove("Theme");
            ModelState.Remove("Prioritet");
            if (ModelState.IsValid)
            {
                _context.Add(request);
                await _context.SaveChangesAsync();
                return RedirectToAction("MyRequests", new RouteValueDictionary(new
                {
                    Controller = "Requests",
                    Action = "MyRequests",
                    id = userId
                }));
            }
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "StatusRequestId", "Status", request.StatusRequestId);
            ViewData["ThemeId"] = new SelectList(_context.ThemeRequests, "ThemeId", "DescriptionTheme", request.ThemeId);
            ViewData["PrioritetId"] = new SelectList(_context.Prioritets, "PrioritetId", "PrioritetRequest");
            return View(request);
        }

        // Метод для редактирования заявки GET
        public async Task<IActionResult> Edit(string user, int id)
        {
            if (id == null || _context.Request == null)
            {
                return NotFound();
            }
            if (user == null)
            {
                return NotFound();
            }

            var request = await _context.Request.FindAsync(id);
            if (request == null)
            {
                return NotFound();
            }
            var managers = from role in _context.Roles
                           .Where(c => c.Name == "Manager" || c.Name == "Admin")
                           join roles in _context.UserRoles on role.Id equals roles.RoleId
                           join users in _context.Users on roles.UserId equals users.Id
                           select new
                           {
                               users.Id,
                               users.UserName
                           };
            ViewData["ManagerId"] = new SelectList(managers, "Id", "UserName", request.ManagerId);
            ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "StatusRequestId", "Status", request.StatusRequestId);
            ViewData["ThemeId"] = new SelectList(_context.ThemeRequests, "ThemeId", "DescriptionTheme", request.ThemeId);
            ViewData["PrioritetId"] = new SelectList(_context.Prioritets, "PrioritetId", "PrioritetRequest");
            return View(request);
        }

        // Метод для редактирования заявки POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string user, int id, [Bind("RequestId,UserId,ThemeId,PrioritetId,TextRequest,Image,DateOpen,ManagerId,DateClose,StatusRequestId")] Request request)
        {
            if (id != request.RequestId)
            {
                return NotFound();
            }
            if (request.Image != null)
            {
                //string uploadfolder = Path.Combine(_environment.WebRootPath, "images");
                //filename = Guid.NewGuid().ToString() + "_" + request.Image.FileName;
                //string filepath = Path.Combine(uploadfolder, filename);
                //request.Image.CopyTo(new FileStream(filepath, FileMode.Create));
                byte[]? FileContent = null;
                if (request.Image.Length > 0)
                {
                    using (var ms = new MemoryStream())
                    {
                        request.Image.CopyTo(ms);
                        FileContent = ms.ToArray();
                    }
                }
                request.File = FileContent;
            }
            string userId = request.UserId;
            if (request.StatusRequestId == 1 || request.StatusRequestId == 4)
            {
                request.ManagerId = null;
            }
            var responces = _context.Responses
                           .Where(c => c.RequestId == request.RequestId);
            int b = responces.Count();
            if (responces.Count() == 0 && request.StatusRequestId == 3) 
            {
                ModelState.AddModelError(string.Empty, "Нельзя закрыть заявку без ответа");
                var managers = from role in _context.Roles
                                           .Where(c => c.Name == "Manager" || c.Name == "Admin")
                               join roles in _context.UserRoles on role.Id equals roles.RoleId
                               join users in _context.Users on roles.UserId equals users.Id
                               select new
                               {
                                   users.Id,
                                   users.UserName
                               };
                ViewData["ManagerId"] = new SelectList(managers, "Id", "UserName", request.ManagerId);
                ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "StatusRequestId", "Status", request.StatusRequestId);
                ViewData["ThemeId"] = new SelectList(_context.ThemeRequests, "ThemeId", "DescriptionTheme", request.ThemeId);
                ViewData["PrioritetId"] = new SelectList(_context.Prioritets, "PrioritetId", "PrioritetRequest");
                return View(request);
            }
            else
            {
                ModelState.Remove("image");
                ModelState.Remove("Manager");
                ModelState.Remove("User");
                ModelState.Remove("StatusRequest");
                ModelState.Remove("Theme");
                ModelState.Remove("Prioritet");
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(request);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RequestExists(request.RequestId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    if (user == userId)
                    {
                        return RedirectToAction("MyRequests", new RouteValueDictionary(new
                        {
                            Controller = "Requests",
                            Action = "MyRequests",
                            id = userId
                        }));
                    }
                    else 
                    {
                        var role = from users in _context.UserRoles
                                           .Where(c => c.UserId == user)
                                       join roles in _context.Roles on users.RoleId equals roles.Id
                                       select new
                                       {
                                           roles.Name
                                       };
                        foreach (var item in role) 
                        {
                            if (item.Name == "Manager")
                            {
                                return RedirectToAction("RequestsInProgress", new RouteValueDictionary(new
                                {
                                    Controller = "Manager",
                                    Action = "RequestsInProgress",
                                    id = user
                                }));
                            }
                            else if (item.Name == "Admin")
                            {
                                return RedirectToAction("ListRequests", new RouteValueDictionary(new
                                {
                                    Controller = "Admin",
                                    Action = "ListRequests"
                                }));
                            }
                            else 
                            {
                                return RedirectToAction("MyRequests", new RouteValueDictionary(new
                                {
                                    Controller = "Requests",
                                    Action = "MyRequests",
                                    id = user
                                }));
                            }
                        }
                    }
                    //return RedirectToAction("MyRequests", new RouteValueDictionary(new
                    //{
                    //    Controller = "Requests",
                    //    Action = "MyRequests",
                    //    id = userId
                    //}));
                }
                var managers = from role in _context.Roles
                                           .Where(c => c.Name == "Manager" || c.Name == "Admin")
                               join roles in _context.UserRoles on role.Id equals roles.RoleId
                               join users in _context.Users on roles.UserId equals users.Id
                               select new
                               {
                                   users.Id,
                                   users.UserName
                               };
                ViewData["ManagerId"] = new SelectList(managers, "Id", "UserName", request.ManagerId);
                ViewData["StatusRequestId"] = new SelectList(_context.StatusRequests, "StatusRequestId", "Status", request.StatusRequestId);
                ViewData["ThemeId"] = new SelectList(_context.ThemeRequests, "ThemeId", "DescriptionTheme", request.ThemeId);
                ViewData["PrioritetId"] = new SelectList(_context.Prioritets, "PrioritetId", "PrioritetRequest");
                return View(request);
            }
        }

        // Метод для удаления заявки GET
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Request == null)
            {
                return NotFound();
            }

            var request = await _context.Request
                .Include(r => r.Manager)
                .Include(r => r.StatusRequest)
                .Include(r => r.Theme)
                .FirstOrDefaultAsync(m => m.RequestId == id);
            if (request == null)
            {
                return NotFound();
            }

            return View(request);
        }

        // Метод для удаления заявки POST
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Request == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Request'  is null.");
            }
            var request = await _context.Request.FindAsync(id);
            if (request != null)
            {
                _context.Request.Remove(request);
            }
            string userId = request.UserId;
            await _context.SaveChangesAsync();
            return RedirectToAction("MyRequests", new RouteValueDictionary(new
            {
                Controller = "Requests",
                Action = "MyRequests",
                id = userId
            }));
        }

        private bool RequestExists(int id)
        {
            return (_context.Request?.Any(e => e.RequestId == id)).GetValueOrDefault();
        }
    }
}
