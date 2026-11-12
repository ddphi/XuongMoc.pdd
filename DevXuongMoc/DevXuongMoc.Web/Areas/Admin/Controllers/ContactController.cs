using DevXuongMoc.Domain.Entities;
using DevXuongMoc.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DevXuongMoc.Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ContactController : Controller
    {
        private readonly AppDbContext _context;

        public ContactController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int? status)
        {
            var query = _context.ContactRequests
                .Include(c => c.Product)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(c => c.Status == status.Value);
            }

            var contacts = await query.OrderByDescending(c => c.CreatedAt).ToListAsync();
            ViewBag.CurrentStatus = status;
            return View(contacts);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var contact = await _context.ContactRequests
                .Include(c => c.Product)
                .Include(c => c.ContactNotes)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (contact == null) return NotFound();

            return View(contact);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, byte status)
        {
            var contact = await _context.ContactRequests.FindAsync(id);
            if (contact != null)
            {
                contact.Status = status;
                contact.UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã cập nhật trạng thái!";
            }
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddNote(int contactId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var note = new ContactNote
                {
                    ContactRequestId = contactId,
                    Content = content,
                    IsSystem = false, // Ghi chú do Admin nhập
                    CreatedAt = DateTime.Now,
                    CreatedBy = "Admin"
                };
                _context.ContactNotes.Add(note);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Đã thêm ghi chú!";
            }
            return RedirectToAction(nameof(Details), new { id = contactId });
        }

        // gia lap du lieu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> FakeData()
        {
            var fakeContact = new ContactRequest
            {
                Type = 0,
                Status = 0,
                Message = "Tôi muốn hỏi....",
                IpAddress = "127.0.0.1",
                CreatedAt = DateTime.Now
            };
            _context.ContactRequests.Add(fakeContact);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}