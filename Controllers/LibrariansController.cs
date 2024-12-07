using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;

namespace LibraryManagementSystem.Controllers
{
    public class LibrariansController : Controller
    {
        private readonly LibraryManagementContext _context;

        public LibrariansController(LibraryManagementContext context)
        {
            _context = context;
        }

        // GET: Librarians
        public async Task<IActionResult> Index()
        {
            return View(await _context.Librarians.ToListAsync());
        }

        // GET: Librarians/Details/5
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.LibrarianId == id);
            if (librarian == null)
            {
                return NotFound();
            }

            return View(librarian);
        }

        [Authorize(Roles = "Librarian")]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Register(Librarian lUser)
        {
            if(ModelState.IsValid)
            {
               await _context.Librarians.AddAsync(lUser);
                await _context.SaveChangesAsync();
                TempData["succerss"] = "Registerd Successfully";
                return RedirectToAction("Dashboard");
            }
            return View();
        }

        [Authorize(Roles = "Librarian")]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("UserSession") != null)
            {
               return RedirectToAction("Dashboard");
            }
            return View();
        }
        [HttpPost]
        [Authorize(Roles = "Librarian")]
        public IActionResult Login(Librarian lUser)
        {
            var lib = _context.Librarians
                .Where(x => x.Email == lUser.Email && x.Password == lUser.Password).FirstOrDefault();
            if (lib != null)
            {
                HttpContext.Session.SetString("UserSession", lib.Email);
                HttpContext.Session.SetString("UserRole", "Librarian");
                return RedirectToAction("Dashboard");
            }
            else
            {
                ViewBag.Message = "Login failed";
            }
            return View();

        }
        [Authorize(Roles = "Librarian")]
        public IActionResult Dashboard()
        {
           if (HttpContext.Session.GetString("UserSession") != null)
            {
                ViewBag.Librariansession = HttpContext.Session.GetString("UserSession").ToString();
            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();
        }
        [Authorize(Roles = "Librarian")]
        public IActionResult Logout()
        {
            if(HttpContext.Session.GetString("UserSession") != null)
            {
                HttpContext.Session.Remove("UserSession");
                return RedirectToAction("Index");
            }
            return View();
        }



        // GET: Librarians/Create
        [Authorize(Roles = "Librarian")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Librarians/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Create([Bind("LibrarianId,Name,Email,Role,DateJoined,Password")] Librarian librarian)
        {
            if (ModelState.IsValid)
            {
                _context.Add(librarian);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(librarian);
        }

        // GET: Librarians/Edit/5
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian == null)
            {
                return NotFound();
            }
            return View(librarian);
        }

        // POST: Librarians/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Edit(int id, [Bind("LibrarianId,Name,Email,Role,DateJoined,Password")] Librarian librarian)
        {
            if (id != librarian.LibrarianId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(librarian);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LibrarianExists(librarian.LibrarianId))
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
            return View(librarian);
        }

        // GET: Librarians/Delete/5
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var librarian = await _context.Librarians
                .FirstOrDefaultAsync(m => m.LibrarianId == id);
            if (librarian == null)
            {
                return NotFound();
            }

            return View(librarian);
        }

        // POST: Librarians/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Librarian")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var librarian = await _context.Librarians.FindAsync(id);
            if (librarian != null)
            {
                _context.Librarians.Remove(librarian);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool LibrarianExists(int id)
        {
            return _context.Librarians.Any(e => e.LibrarianId == id);
        }
    }
}
