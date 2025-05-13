using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseProject.Data;
using EventEaseProject.Models;

namespace EventEaseProject.Controllers
{
    public class HomeController : Controller
    {
        private readonly EEDbContext _context;

        public HomeController(EEDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Search(string searchTerm)
        {
            try
            {
                var query = _context.BookingViews.AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    query = query.Where(b => b.BookingID.ToString().Contains(searchTerm) ||
                                          (b.EventDescription != null && b.EventDescription.Contains(searchTerm)));
                }

                var results = await query.OrderBy(b => b.BookingDate).ToListAsync();
                return View(results ?? new List<BookingView>());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error searching bookings: {ex.Message}";
                return View(new List<BookingView>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}