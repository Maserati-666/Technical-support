using Azure;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Technical_support.Data;
using Technical_support.Models;
using Technical_support.ViewModel;
using Request = Technical_support.Models.Request;
using Response = Technical_support.Models.Response;

namespace Technical_support.Controllers
{
    public class ManagerController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ManagerController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var requests = _context.Request;
            return View(requests);
        }

        // Метод для получения открытых заявок
        public IActionResult ListRequests()
        {
            List<ListRequest> openRequests = new();
            var requests = from request in _context.Request
                           .Where(c => c.StatusRequestId == 1)
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
                               request.File,
                               request.DateOpen,
                               status.Status
                           };
            foreach (var item in requests)
            {
                openRequests.Add(new ListRequest
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    File = item.File,
                    DateOpen = item.DateOpen,
                    Status = item.Status
                });
            }
            return View(openRequests);
        }

        // Метод для получения выполненных заявок
        public IActionResult CloseRequests(string id)
        {
            List<ListRequest> closeRequests = new();
            var requests = from request in _context.Request
                           .Where(c => c.StatusRequestId == 3 && c.ManagerId == id)
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
                               request.File,
                               request.DateOpen,
                               status.Status
                           };
            foreach (var item in requests)
            {
                closeRequests.Add(new ListRequest
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    File = item.File,
                    DateOpen = item.DateOpen,
                    Status = item.Status
                });
            }
            return View(closeRequests);
        }

        // Метод для принятия заявки
        public async Task<IActionResult> AcceptRequest(int id, string managerId)
        {
            var requests =_context.Request
                           .Where(c => c.RequestId == id).ToList();
            List<Request> editRequest = new();
            foreach (var item in requests)
            {
                editRequest.Add(item);
            }
            foreach (var item in editRequest)
            {
                if (item.RequestId == id)
                {
                    try
                    {
                        item.ManagerId = managerId;
                        item.StatusRequestId = 2;
                        _context.Update(item);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RequestExists(item.RequestId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("RequestsInProgress", new RouteValueDictionary(new
                    {
                        Controller = "Manager",
                        Action = "RequestsInProgress",
                        id = managerId
                    }));
                }  
            }

            return View();
        }

        // Метод для получения заявок в работе
        public IActionResult RequestsInProgress(string id)
        {
            List<ListRequest> progressRequests = new();
            var requests = from request in _context.Request
                           .Where(c => c.StatusRequestId == 2 && c.ManagerId == id)
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
                               request.File,
                               request.DateOpen,
                               status.Status
                           };
            foreach (var item in requests)
            {
                progressRequests.Add(new ListRequest
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    File = item.File,
                    DateOpen = item.DateOpen,
                    Status = item.Status
                });
            }
            return View(progressRequests);
        }

        private bool RequestExists(int id)
        {
            return (_context.Request?.Any(e => e.RequestId == id)).GetValueOrDefault();
        }

        // Метод для закрытия заявки
        public IActionResult RequestClose(int id)
        {
            List<ListRequest> progressRequests = new();
            var requests = from request in _context.Request
                           .Where(c => c.RequestId == id)
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
                               request.File,
                               request.DateOpen,
                               status.Status
                           };
            foreach (var item in requests)
            {
                progressRequests.Add(new ListRequest
                {
                    RequestId = item.RequestId,
                    UserName = item.UserName,
                    NameTheme = item.NameTheme,
                    Prioritet = item.PrioritetRequest,
                    TextRequest = item.TextRequest,
                    File = item.File,
                    DateOpen = item.DateOpen,
                    Status = item.Status
                });
            }
            if (progressRequests == null)
            {
                return NotFound();
            }
            List<Response> responses = _context.Responses
                           .Where(c => c.RequestId == id)
                           .ToList();
            List<RequestCloseInfo> requestClose = new();
            requestClose.Add(new RequestCloseInfo 
            {
                RequestAccept = progressRequests,
                ListResponses = responses
            });
            return View(requestClose);
        }


        // Метод для подтверждения закрытия заявки
        [HttpPost, ActionName("RequestClose")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestCloseConfirmed(int id)
        {
            var requests = _context.Request
                           .Where(c => c.RequestId == id).ToList();
            List<Request> editRequest = new();
            foreach (var item in requests)
            {
                editRequest.Add(item);
            }
            foreach (var item in editRequest)
            {
                if (item.RequestId == id)
                {
                    string managerId = item.ManagerId;
                    try
                    {
                        item.StatusRequestId = 3;
                        item.DateClose = DateTime.Now;
                        _context.Update(item);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!RequestExists(item.RequestId))
                        {
                            return NotFound();
                        }
                        else
                        {
                            throw;
                        }
                    }
                    return RedirectToAction("CloseRequests", new RouteValueDictionary(new {Controller = "Manager",
                        Action = "CloseRequests", id = managerId }));
                }
            }
            return View();
        }
    }
}
