using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using EventEaseProject.Data;
using EventEaseProject.Models;

namespace EventEaseProject.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EEDbContext _context;

        public BookingsController(EEDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string searchTerm)
        {
            try
            {
                var query = from b in _context.Bookings
                            join e in _context.Events on b.EventId equals e.EventId
                            join v in _context.Venues on b.VenueId equals v.VenueId
                            select new BookingView
                            {
                                BookingID = b.BookingId,
                                VenueName = v.VenueName,
                                VenueLocation = v.VenueLocation,
                                VenueCapacity = v.VenueCapacity,
                                EventDate = e.EventDate,
                                EventDescription = e.EventDescription,
                                BookingDate = b.BookingDate
                            };

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    searchTerm = searchTerm.ToLower().Trim();
                    query = query.Where(b => b.BookingID.ToString().Contains(searchTerm) ||
                                          b.EventDescription.ToLower().Contains(searchTerm) ||
                                          b.VenueName.ToLower().Contains(searchTerm) ||
                                          b.VenueLocation.ToLower().Contains(searchTerm));
                }

                var bookings = await query.OrderBy(b => b.BookingDate).ToListAsync();
                ViewBag.SearchTerm = searchTerm;
                TempData["SearchMessage"] = bookings.Any() ? null : "No bookings found matching the search term.";
                return View(bookings);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading bookings: {ex.Message}";
                return View(new List<BookingView>());
            }
        }

        public IActionResult Create()
        {
            try
            {
                ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName");
                ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventDescription");
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading create form: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,VenueId,BookingDate")] Booking booking)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var existingBooking = await _context.Bookings
                        .AnyAsync(b => b.VenueId == booking.VenueId &&
                                       b.BookingDate.Date == booking.BookingDate.Date);

                    if (existingBooking)
                    {
                        ModelState.AddModelError("BookingDate", "This venue is already booked on the selected date.");
                    }
                    else
                    {
                        _context.Add(booking);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Booking created successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("CHK_UniqueBooking") == true)
            {
                ModelState.AddModelError("BookingDate", "This combination of event, venue, and date is already booked.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error creating booking: {ex.Message}";
            }

            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventDescription", booking.EventId);
            return View(booking);
        }

        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var booking = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .FirstOrDefaultAsync(m => m.BookingId == id);

                if (booking == null)
                    return NotFound();

                return View(booking);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading booking details: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                    return NotFound();

                ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
                ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventDescription", booking.EventId);
                return View(booking);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading booking for edit: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BookingId,EventId,VenueId,BookingDate")] Booking booking)
        {
            try
            {
                if (id != booking.BookingId)
                    return NotFound();

                if (ModelState.IsValid)
                {
                    var existingBooking = await _context.Bookings
                        .AnyAsync(b => b.VenueId == booking.VenueId &&
                                       b.BookingDate.Date == booking.BookingDate.Date &&
                                       b.BookingId != booking.BookingId);

                    if (existingBooking)
                    {
                        ModelState.AddModelError("BookingDate", "This venue is already booked on the selected date.");
                    }
                    else
                    {
                        _context.Update(booking);
                        await _context.SaveChangesAsync();
                        TempData["SuccessMessage"] = "Booking updated successfully.";
                        return RedirectToAction(nameof(Index));
                    }
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await BookingExists(booking.BookingId))
                    return NotFound();
                throw;
            }
            catch (DbUpdateException ex) when (ex.InnerException?.Message.Contains("CHK_UniqueBooking") == true)
            {
                ModelState.AddModelError("BookingDate", "This combination of event, venue, and date is already booked.");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error updating booking: {ex.Message}";
            }

            ViewBag.VenueId = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            ViewBag.EventId = new SelectList(_context.Events, "EventId", "EventDescription", booking.EventId);
            return View(booking);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var booking = await _context.Bookings
                    .Include(b => b.Event)
                    .Include(b => b.Venue)
                    .FirstOrDefaultAsync(m => m.BookingId == id);

                if (booking == null)
                    return NotFound();

                return View(booking);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error loading booking for deletion: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var booking = await _context.Bookings.FindAsync(id);
                if (booking == null)
                    return RedirectToAction(nameof(Index));

                _context.Bookings.Remove(booking);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Booking deleted successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Error deleting booking: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }

        private async Task<bool> BookingExists(int id)
        {
            try
            {
                return await _context.Bookings.AnyAsync(b => b.BookingId == id);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}