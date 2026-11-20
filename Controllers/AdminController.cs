using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using GestionEventiMagazzino.Services;

namespace GestionEventiMagazzino.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserService userService;

        public AdminController(UserService userService)
        {
            this.userService = userService;
        }
        public async Task<IActionResult> Index() //task indica un'operazione asincrona, IActionResult manda al pannello admin
        {
            var usernames = await userService.GetUsernamesAsync();
            return View(usernames);
        }
    }
}
