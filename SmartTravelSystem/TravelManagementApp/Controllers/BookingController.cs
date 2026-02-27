using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using TravelDataAccess.Models;

namespace TravelManagementApp.Controllers
{
    public class BookingController : Controller
    {
        private readonly TravelContext _context;

        public BookingController(TravelContext context)
        {
            _context = context;
        }

        // GET: Booking
        public async Task<IActionResult> Index(string filter)
        {
            var userRole = User.FindFirst("Role")?.Value;
            
            IQueryable<Booking> query;
            
            // Admin xem tất cả bookings, Customer chỉ xem của mình
            if (userRole == "Admin")
            {
                query = _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Trip);
                ViewBag.IsAdmin = true;
            }
            else
            {
                // Customer chỉ xem bookings của mình
                var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
                query = _context.Bookings
                    .Include(b => b.Customer)
                    .Include(b => b.Trip)
                    .Where(b => b.CustomerID == customerId);
                ViewBag.IsAdmin = false;
            }
            
            // Filter by status if requested
            if (!string.IsNullOrEmpty(filter) && filter.ToLower() == "pending")
            {
                query = query.Where(b => b.Status == "Pending");
                ViewBag.Filter = "pending";
            }
            
            var travelContext = await query
                .OrderBy(b => b.BookingDate) // Sắp xếp tăng dần
                .ToListAsync();
                
            return View(travelContext);
        }

        // GET: Booking/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            
            if (booking == null)
            {
                return NotFound();
            }

            // Check authorization: Admin can view all, Customer only their own
            var userRole = User.FindFirst("Role")?.Value;
            var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
            
            if (userRole != "Admin" && booking.CustomerID != customerId)
            {
                return Forbid(); // 403 Forbidden
            }

            return View(booking);
        }

        // GET: Booking/Create
        public async Task<IActionResult> Create()
        {
            await PopulateDropdownsAsync();
            return View();
        }

        // POST: Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TripID,Status,CustomerID")] Booking booking)
        {
            try
            {
                // Xác thực và set CustomerID
                var customerId = GetCustomerIdForBooking(booking);
                if (customerId == 0)
                {
                    TempData["Error"] = "Phiên đăng nhập đã hết. Vui lòng đăng nhập lại.";
                    return RedirectToAction("Index", "Login");
                }
                
                booking.CustomerID = customerId;
                booking.BookingDate = DateOnly.FromDateTime(DateTime.Now);

                // Validate business rules
                if (!await ValidateBookingAsync(booking))
                {
                    await PopulateDropdownsAsync(booking);
                    return View(booking);
                }

                // Remove navigation properties from validation
                ModelState.Remove("Customer");
                ModelState.Remove("Trip");

                if (!ModelState.IsValid)
                {
                    await PopulateDropdownsAsync(booking);
                    return View(booking);
                }

                // Save booking
                _context.Add(booking);
                await _context.SaveChangesAsync();
                
                TempData["Success"] = "Tạo booking thành công!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                TempData["Error"] = "Có lỗi xảy ra khi tạo booking. Vui lòng thử lại.";
                await PopulateDropdownsAsync(booking);
                return View(booking);
            }
        }


        // GET: Booking/Edit/5
        public async Task<IActionResult> Edit(int? id)
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

            // Check authorization: Admin can edit all, Customer only their own
            var userRole = User.FindFirst("Role")?.Value;
            var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
            
            if (userRole != "Admin" && booking.CustomerID != customerId)
            {
                return Forbid(); // 403 Forbidden
            }

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Code", booking.CustomerID);
            ViewData["TripID"] = new SelectList(_context.Trips, "TripID", "Code", booking.TripID);
            return View(booking);
        }

        // POST: Booking/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id,
            [Bind("BookingID,TripID,CustomerID,BookingDate,Status")] Booking booking)
        {
            if (id != booking.BookingID)
            {
                return NotFound();
            }

            // Check authorization before edit
            var existingBooking = await _context.Bookings.AsNoTracking().FirstOrDefaultAsync(b => b.BookingID == id);
            if (existingBooking != null)
            {
                var userRole = User.FindFirst("Role")?.Value;
                var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
                
                if (userRole != "Admin" && existingBooking.CustomerID != customerId)
                {
                    return Forbid();
                }
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(booking);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookingExists(booking.BookingID))
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

            ViewData["CustomerID"] = new SelectList(_context.Customers, "CustomerID", "Code", booking.CustomerID);
            ViewData["TripID"] = new SelectList(_context.Trips, "TripID", "Code", booking.TripID);
            return View(booking);
        }

        // GET: Booking/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var booking = await _context.Bookings
                .Include(b => b.Customer)
                .Include(b => b.Trip)
                .FirstOrDefaultAsync(m => m.BookingID == id);
            if (booking == null)
            {
                return NotFound();
            }

            // Check authorization: Admin can delete all, Customer only their own
            var userRole = User.FindFirst("Role")?.Value;
            var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
            
            if (userRole != "Admin" && booking.CustomerID != customerId)
            {
                return Forbid();
            }

            return View(booking);
        }

        // POST: Booking/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking != null)
            {
                // Check authorization before delete
                var userRole = User.FindFirst("Role")?.Value;
                var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
                
                if (userRole != "Admin" && booking.CustomerID != customerId)
                {
                    return Forbid();
                }
                
                _context.Bookings.Remove(booking);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Booking/UpdateStatus - Quick update status only
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, string status)
        {
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null)
            {
                return NotFound();
            }

            // Check authorization
            var userRole = User.FindFirst("Role")?.Value;
            var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
            
            if (userRole != "Admin" && booking.CustomerID != customerId)
            {
                return Forbid();
            }

            // Validate status
            if (status != "Pending" && status != "Confirmed" && status != "Cancelled")
            {
                return BadRequest("Invalid status");
            }

            booking.Status = status;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool BookingExists(int id)
        {
            return _context.Bookings.Any(e => e.BookingID == id);
        }

        // Helper Methods
        private int GetCustomerIdForBooking(Booking booking)
        {
            var userRole = User.FindFirst("Role")?.Value;
            
            // Admin có thể tạo cho bất kỳ customer nào
            if (userRole == "Admin" && booking.CustomerID > 0)
            {
                return booking.CustomerID;
            }
            
            // Customer chỉ tạo cho chính mình
            if (int.TryParse(User.FindFirst("CustomerId")?.Value, out int customerId))
            {
                return customerId;
            }
            
            return 0;
        }

        private async Task<bool> ValidateBookingAsync(Booking booking)
        {
            // Kiểm tra Trip có tồn tại và available
            var trip = await _context.Trips.FindAsync(booking.TripID);
            if (trip == null)
            {
                ModelState.AddModelError("TripID", "Trip không tồn tại.");
                return false;
            }
            
            if (trip.Status != "Available")
            {
                ModelState.AddModelError("TripID", "Trip này không còn khả dụng.");
                return false;
            }
            
            // Kiểm tra Customer có tồn tại
            var customer = await _context.Customers.FindAsync(booking.CustomerID);
            if (customer == null)
            {
                ModelState.AddModelError("CustomerID", "Customer không tồn tại.");
                return false;
            }
            
            // Kiểm tra duplicate booking
            var existingBooking = await _context.Bookings
                .AnyAsync(b => b.CustomerID == booking.CustomerID 
                            && b.TripID == booking.TripID 
                            && b.Status != "Cancelled");
            
            if (existingBooking)
            {
                ModelState.AddModelError("", "Bạn đã đặt trip này rồi.");
                return false;
            }
            
            return true;
        }

        private async Task PopulateDropdownsAsync(Booking? booking = null)
        {
            var userRole = User.FindFirst("Role")?.Value;
            
            // Load trips với thông tin đầy đủ hơn
            var trips = await _context.Trips
                .Where(t => t.Status == "Available")
                .Select(t => new { 
                    t.TripID, 
                    DisplayText = $"{t.Code} - {t.Destination} ({t.Price:C})" 
                })
                .ToListAsync();
            
            ViewData["TripID"] = new SelectList(trips, "TripID", "DisplayText", booking?.TripID);
            
            if (userRole == "Admin")
            {
                var customers = await _context.Customers
                    .Select(c => new { 
                        c.CustomerID, 
                        DisplayText = $"{c.Code} - {c.FullName}" 
                    })
                    .ToListAsync();
                
                ViewData["CustomerID"] = new SelectList(customers, "CustomerID", "DisplayText", booking?.CustomerID);
                ViewBag.IsAdmin = true;
            }
            else
            {
                var customerId = int.Parse(User.FindFirst("CustomerId")?.Value ?? "0");
                ViewBag.CustomerID = customerId;
                ViewBag.IsAdmin = false;
            }
        }
    }
}