using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Eventis_web_app.Models;
using Eventis_web_app.Services;

namespace Eventis_web_app.Controllers
{
    public class EventsController : Controller
    {
        private readonly EventisContext _context;
        private readonly BlobStorageService _blobService;
        private readonly string _containerName;

        public EventsController(EventisContext context, BlobStorageService blobService, IConfiguration configuration)
        {
            _context = context;
            _blobService = blobService;
            _containerName = configuration["AzureBlobStorage:EventImagesContainer"] ?? "event-images";
        }

        // GET: Events
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Events/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("EventId,EventName,Description,StartDate,EndDate")] Event @event, IFormFile? imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    @event.ImageUrl = await _blobService.UploadImageAsync(_containerName, imageFile);
                }
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            return View(@event);
        }

        // POST: Events/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("EventId,EventName,Description,StartDate,EndDate")] Event @event, IFormFile? imageFile)
        {
            if (id != @event.EventId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var existingEvent = await _context.Events.AsNoTracking().FirstOrDefaultAsync(e => e.EventId == id);

                    if (imageFile != null && imageFile.Length > 0)
                    {
                        // Delete old image if exists
                        if (existingEvent?.ImageUrl != null)
                        {
                            await _blobService.DeleteImageAsync(existingEvent.ImageUrl);
                        }
                        @event.ImageUrl = await _blobService.UploadImageAsync(_containerName, imageFile);
                    }
                    else
                    {
                        // Keep existing image URL
                        @event.ImageUrl = existingEvent?.ImageUrl;
                    }

                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.EventId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(@event);
        }

        // GET: Events/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .FirstOrDefaultAsync(m => m.EventId == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                // Check for active bookings
                var hasBookings = await _context.Bookings.AnyAsync(b => b.EventId == id);
                if (hasBookings)
                {
                    TempData["ErrorMessage"] = "Cannot delete this event because it has active bookings. Please remove the bookings first.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                // Delete image from Azure Blob Storage
                if (!string.IsNullOrEmpty(@event.ImageUrl))
                {
                    await _blobService.DeleteImageAsync(@event.ImageUrl);
                }
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.EventId == id);
        }
    }
}
