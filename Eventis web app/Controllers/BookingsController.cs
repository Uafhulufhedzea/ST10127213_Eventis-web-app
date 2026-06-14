using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eventis_web_app.Models;

namespace Eventis_web_app.Controllers
{
    public class BookingsController : Controller
    {
        private readonly EventisContext _context;

        public BookingsController(EventisContext context)
        {
            _context = context;
        }
// GET: Bookings
public async Task<IActionResult> Index(string? searchString, int? eventTypeId, DateTime? startDate, DateTime? endDate, bool? isAvailable)
{
    // Preserve active filters in ViewData for the UI form state
    ViewData["CurrentSearch"] = searchString;
    ViewData["SelectedEventType"] = eventTypeId;
    ViewData["SelectedStartDate"] = startDate?.ToString("yyyy-MM-dd");
    ViewData["SelectedEndDate"] = endDate?.ToString("yyyy-MM-dd");
    ViewData["SelectedAvailability"] = isAvailable;

    // Populates the Event Type filter dropdown menu option list
    ViewData["EventTypesList"] = new SelectList(await _context.EventTypes.ToListAsync(), "EventTypeId", "TypeName", eventTypeId);

    // Core Query running against the registered Database View
    var bookingsQuery = _context.BookingSummaryViews.AsQueryable();

    // 1. Text Search Filter (Booking ID or Event Name)
    if (!string.IsNullOrEmpty(searchString))
    {
        bookingsQuery = bookingsQuery.Where(b =>
            b.BookingId.ToString().Contains(searchString) ||
            b.EventName.Contains(searchString));
    }

    // 2. Event Type Dropdown Filter
    if (eventTypeId.HasValue)
    {
        bookingsQuery = bookingsQuery.Where(b => b.EventTypeId == eventTypeId.Value);
    }

    // 3. Start Date Filter (Booking occurs on or after this date)
    if (startDate.HasValue)
    {
        bookingsQuery = bookingsQuery.Where(b => b.BookingDate >= startDate.Value.Date);
    }

    // 4. End Date Filter (Booking occurs on or before this date)
    if (endDate.HasValue)
    {
        bookingsQuery = bookingsQuery.Where(b => b.BookingDate <= endDate.Value.Date);
    }

    // 5. Venue Availability Toggle Filter
    if (isAvailable.HasValue)
    {
        bookingsQuery = bookingsQuery.Where(b => b.IsAvailable == isAvailable.Value);
    }

    return View(await bookingsQuery.ToListAsync());
}


        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // GET: Bookings/Create
        public IActionResult Create()
        {
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName");
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName");
            return View();
        }

        // POST: Bookings/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
      [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Create([Bind("BookingId,EventId,VenueId,BookingDate,Status")] Booking booking)
{
    if (ModelState.IsValid)
    {
        // FIX 4 IMPLEMENTATION: Force a clean midnight date timestamp layout
        if (booking.BookingDate.HasValue)
        {
            booking.BookingDate = booking.BookingDate.Value.Date;
        }

        // Check for double booking: same venue on the same standardized date
        var conflict = await _context.Bookings.AnyAsync(b =>
            b.VenueId == booking.VenueId &&
            b.BookingDate == booking.BookingDate);

        if (conflict)
        {
            ModelState.AddModelError("", "This venue is already booked on the selected date. Please choose a different date or venue.");
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        booking.BookingId = Guid.NewGuid();
        _context.Add(booking);
        await _context.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }
    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
    return View(booking);
}


        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        // POST: Bookings/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> Edit(Guid id, [Bind("BookingId,EventId,VenueId,BookingDate,Status")] Booking booking)
{
    if (id != booking.BookingId)
    {
        return NotFound();
    }

    if (ModelState.IsValid)
    {
        // FIX 4 IMPLEMENTATION: Enforce standardized midnight values on modifications
        if (booking.BookingDate.HasValue)
        {
            booking.BookingDate = booking.BookingDate.Value.Date;
        }

        // Check for double booking: exclude this current booking ID from conflict matching
        var conflict = await _context.Bookings.AnyAsync(b =>
            b.BookingId != booking.BookingId &&
            b.VenueId == booking.VenueId &&
            b.BookingDate == booking.BookingDate);

        if (conflict)
        {
            ModelState.AddModelError("", "This venue is already booked on the selected date. Please choose a different date or venue.");
            ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
            ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
            return View(booking);
        }

        try
        {
            _context.Update(booking);
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!BookingExists(booking.BookingId)) { return NotFound(); } else { throw; }
        }
        return RedirectToAction(nameof(Index));
    }
    ViewData["EventId"] = new SelectList(_context.Events, "EventId", "EventName", booking.EventId);
    ViewData["VenueId"] = new SelectList(_context.Venues, "VenueId", "VenueName", booking.VenueId);
    return View(booking);
}


        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Event)
                .Include(b => b.Venue)
                .FirstOrDefaultAsync(m => m.BookingId == id);
            if (booking == null)
            {
                return NotFound();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(Guid id)
        {
            return _context.Bookings.Any(e => e.BookingId == id);
        }
    }
}
