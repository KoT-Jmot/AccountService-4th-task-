using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Zad4.Data;
using Zad4.Models;

namespace Zad4.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(User model)
        {
            if (IsUnBlock(model.Login))
            {
                if (model.Login != null && model.Password != null)
                {
                    var testDBContext = new TestDBContext();
                    var person = testDBContext.Users.FirstOrDefault(a => a.Login == model.Login && a.Password == model.Password);
                    if (person != null)
                    {
                        person.LastLogDate = DateTime.Now;
                        testDBContext.SaveChanges();
                        ViewData["User"] = person.Login;
                        return View("MainMenu", testDBContext.Users.ToList());
                    }
                }
            }
            ModelState.AddModelError("Name", "Enter correct data!");
            return View();
        }
        [HttpGet]
        public IActionResult Sign_Up()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Sign_Up(User model)
        {
            if (model.Name != null && model.Password != null && model.Login != null && model.Password.Length > 0 && model.Name.Length >= 0 && model.Login.Contains("@") && isValid(model.Login))
            {
                model.RegDate = model.LastLogDate = DateTime.Now;
                model.Status = "UnBlock";
                var testDBContext = new TestDBContext();
                if (testDBContext.Users.FirstOrDefault(a => a.Login == model.Login) == null)
                {
                    int x = 1;
                    while (testDBContext.Users.FirstOrDefault(a => a.Id == x) != null)
                        x++;
                    model.Id = x;
                    testDBContext.Users.Add(model);
                    testDBContext.SaveChanges();
                    return View("Index");
                }
                else
                    ModelState.AddModelError("Name", "This account already exists!");
            }
            else
                ModelState.AddModelError("Name", "Enter correct data!");
            return View();
        }
        bool isValid(string email)
        {
            string pattern = "[.\\-_a-z0-9]+@([a-z0-9][\\-a-z0-9]+\\.)+[a-z]{2,6}";
            Match isMatch = Regex.Match(email, pattern, RegexOptions.IgnoreCase);
            return isMatch.Success;
        }

        [HttpGet]
        public IActionResult MainMenu()
        {
            var testDBContext = new TestDBContext();
            return View(testDBContext.Users.ToList());
        }
        [HttpPost]
        public IActionResult MainMenu(string login)
        {
            if (IsUnBlock(login))
            {
                var testDBContext = new TestDBContext();
                ViewData["User"] = login;
                return View(testDBContext.Users.ToList());
            }
            else
            {
                ModelState.AddModelError("Name", "Enter correct data!");
                return View("Index");
            }
        }


        [HttpPost]
        public IActionResult ActionWithAccaunt(int[] selectedUsers,string login, string actions)
        {
            if (IsUnBlock(login))
            {
                var testDBContext = new TestDBContext();
                if (selectedUsers != null)
                {

                    for (int i = 0; i < selectedUsers.Length; i++)
                    {
                        if (testDBContext.Users.FirstOrDefault(a => a.Id == selectedUsers[i]) != null)
                        {
                            switch (actions)
                            {
                                case "Delete":
                                    testDBContext.Users.Remove(testDBContext.Users.FirstOrDefault(a => a.Id == selectedUsers[i]));
                                    break;
                                default:
                                    testDBContext.Users.FirstOrDefault(a => a.Id == selectedUsers[i]).Status = actions;
                                    break;
                            }

                        }
                    }
                    testDBContext.SaveChanges();
                }
                return View("MainMenu", testDBContext.Users.ToList());
            }
            else
            {
                ModelState.AddModelError("Name", "Enter correct data!");
                return View("Index");
            }
        }
        [HttpGet]
        public bool IsUnBlock(string login)
        {
            try
            {
                var testDBContext = new TestDBContext();
                if (testDBContext.Users.FirstOrDefault(a => a.Login == login && a.Status == "UnBlock") != null)
                {
                    ViewData["User"] = login;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch { return false; }
        }
        public IActionResult Message(string login)
        {
            if (IsUnBlock(login))
            {
                var testDBContext = new TestDBContext();
                List<Message> Messages = new List<Message>();
                foreach (var mes in testDBContext.Messages)
                {
                    if (mes.ForUser == login || mes.FromUser == login)
                        Messages.Add(new Message(mes));
                    if (mes.ForUser == login)
                        mes.Status = "Readed";
                }
                testDBContext.SaveChanges();
                ViewData["User"] = login;
                return View("Message", Messages);

            }
            return View("Index");
        }
        [HttpPost]
        public IActionResult AddMessage(string ForUser, string Title, string Text, string FromUser)
        {
            try
            {
                if (IsUnBlock(FromUser))
                {
                    var testDBContext = new TestDBContext();
                    if (testDBContext.Users.FirstOrDefault(a => a.Login == ForUser) != null && Title != null && Text != null)
                    {
                        Message mes = new Message();
                        mes.ForUser = ForUser;
                        mes.FromUser = FromUser;
                        mes.Title = Title;
                        mes.Text = Text;
                        mes.SendDate = DateTime.Now;
                        int z = 1;
                        while (testDBContext.Messages.FirstOrDefault(a => a.Id == z) != null)
                        {
                            z++;
                        }
                        mes.Id = z;
                        mes.Status = "UnRead";
                        testDBContext.Messages.Add(mes);
                        testDBContext.SaveChanges();
                    }
                    else
                    {
                        Exception q = new Exception();
                        throw q;
                    }
                }
                else
                {
                    Exception q = new Exception();
                    throw q;
                }
            }
            catch {
                ModelState.AddModelError("Name", "Enter correct data!");
            };
            return Redirect("Message?login="+FromUser);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
