using Microsoft.AspNetCore.Mvc;
using PlaylistApp.Models.DTOs;

namespace PlaylistApp.Controllers
{
    public class AuthController : Controller
    {
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginDto());
        }

        [HttpPost]
        public IActionResult Login(LoginDto dto)
        {
            // Hardcoded user check for lab simplicity
            if (dto.Username == "admin" && dto.Password == "password123")
            {
                // Store user session key
                HttpContext.Session.SetString("UserSession", dto.Username);
                return RedirectToAction("Index", "Playlist");
            }

            ModelState.AddModelError("", "Invalid username or password.");
            return View(dto);
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
