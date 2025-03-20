using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using _20date_MVC.Models;
using _20date_MVC.Data;
using Microsoft.AspNetCore.Authorization;

namespace _20date_MVC.Controllers
{
    [Authorize]
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public UserController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }

        public IActionResult Create() => View();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(User user, IFormFile? image)
        {
            if (!ModelState.IsValid) return View(user);

            if (image != null && image.Length > 0)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads");
                if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);

                var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(uploadDir, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                user.ImagePath = $"/uploads/{uniqueFileName}";
            }

            _context.Add(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();
            return View(user);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, User user, IFormFile? image)
        {
            if (id != user.Id) return NotFound();

            var existingUser = await _context.Users.FindAsync(id);
            if (existingUser == null) return NotFound();

            existingUser.Name = user.Name;
            existingUser.Email = user.Email;
            existingUser.Address = user.Address;
            existingUser.Gender = user.Gender;

            if (image != null)
            {
                var uploadDir = Path.Combine(_env.WebRootPath, "uploads");

                if (!string.IsNullOrEmpty(existingUser.ImagePath))
                {
                    var oldImagePath = Path.Combine(_env.WebRootPath, existingUser.ImagePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldImagePath)) System.IO.File.Delete(oldImagePath);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{image.FileName}";
                var filePath = Path.Combine(uploadDir, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(fileStream);
                }

                existingUser.ImagePath = $"/uploads/{uniqueFileName}";
            }

            _context.Update(existingUser);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (!string.IsNullOrEmpty(user.ImagePath))
            {
                var imagePath = Path.Combine(_env.WebRootPath, user.ImagePath.TrimStart('/'));
                if (System.IO.File.Exists(imagePath)) System.IO.File.Delete(imagePath);
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
