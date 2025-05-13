using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EventEaseProject.Data;
using EventEaseProject.Models;

namespace EventEaseProject.Controllers
{
    public class EventController : Controller
    {
        private readonly EEDbContext _context;

        public EventController(EEDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var query = _context.Events.Include(e => e.Bookings).AsQueryable();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower();
                    query = query.Where(e => e.EventDescription.ToLower().Contains(searchTerm) ||
                                          e.EventDate.ToString().Contains(searchTerm));
                }

                return View(await query.ToListAsync());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading events: {ex.Message}";
                return View(new List<Event>());
            }
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || !await EventExists(id.Value))
                return NotFound();

            try
            {
                var eventItem = await _context.Events
                    .Include(e => e.Bookings)
                    .FirstOrDefaultAsync(m => m.EventId == id);
                if (eventItem == null)
                    return NotFound();
                return View(eventItem);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading event details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventDate,EventDescription")] Event eventItem)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(eventItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event created successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error creating event: {ex.Message}";
                }
            }
            return View(eventItem);
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || !await EventExists(id.Value))
                return NotFound();

            try
            {
                var eventItem = await _context.Events.FindAsync(id);
                if (eventItem == null)
                    return NotFound();
                return View(eventItem);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading event for edit: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventDate,EventDescription")] Event eventItem)
        {
            if (id != eventItem.EventId)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(eventItem);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Event updated successfully.";
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await EventExists(eventItem.EventId))
                        return NotFound();
                    throw;
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Error updating event: {ex.Message}";
                }
            }
            return View(eventItem);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || !await EventExists(id.Value))
                return NotFound();

            try
            {
                var eventItem = await _context.Events
                    .Include(e => e.Bookings)
                    .FirstOrDefaultAsync(m => m.EventId == id);
                if (eventItem == null)
                    return NotFound();
                return View(eventItem);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading event for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var eventItem = await _context.Events.FindAsync(id);
                if (eventItem == null)
                    return RedirectToAction(nameof(Index));

                if (await _context.Bookings.AnyAsync(b => b.EventId == id))
                {
                    TempData["ErrorMessage"] = "Cannot delete event with active bookings.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Events.Remove(eventItem);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Event deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting event: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<bool> EventExists(int id)
        {
            try
            {
                return await _context.Events.AnyAsync(e => e.EventId == id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}