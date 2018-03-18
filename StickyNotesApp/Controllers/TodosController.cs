using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using StickyNotesApp.Data;
using StickyNotesApp.Models;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace StickyNotesApp.Controllers
{
    public class TodosController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TodosController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Todos
        public IActionResult Index()
        {
            return View();
        }

        // GET: Todos
        [Authorize]
        public async Task<IActionResult> IndexAuth()
        {
            return View(await _context.Todos.ToListAsync());
        }

        // GET: Todos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos
                .SingleOrDefaultAsync(m => m.ID == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // GET: Todos/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Todos/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Title,Description")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString(todo.Title, todo.Description);
                return RedirectToAction(nameof(Index));
            }
            return View(todo);
        }

        // GET: Todos/CreateAuth
        [Authorize]
        public IActionResult CreateAuth()
        {
            return View();
        }

        // POST: Todos/CreateAuth
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateAuth([Bind("ID,OwnerID,Title,Description,ExpireDate")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexAuth));
            }
            return View(todo);
        }

        // GET: Todos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos.SingleOrDefaultAsync(m => m.ID == id);
            if (todo == null)
            {
                return NotFound();
            }
            return View(todo);
        }

        // POST: Todos/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Title,Description,IsDone,ExpireDate")] Todo todo)
        {
            if (id != todo.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todo);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoExists(todo.ID))
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
            return View(todo);
        }

        public IActionResult Delete(string id)
        {
            HttpContext.Session.Remove(id);
            return RedirectToAction(nameof(Index));
        }

        public IActionResult DeleteAll()
        {
            return View();
        }

        [HttpPost, ActionName("DeleteAll")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteAllConfirmed()
        {
            HttpContext.Session.Clear();
            return RedirectToAction(nameof(Index));
        }

        // GET: Todos/DeleteAuth/5
        [Authorize]
        public async Task<IActionResult> DeleteAuth(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todo = await _context.Todos
                .SingleOrDefaultAsync(m => m.ID == id);
            if (todo == null)
            {
                return NotFound();
            }

            return View(todo);
        }

        // POST: Todos/DeleteAuth/5
        [Authorize]
        [HttpPost, ActionName("DeleteAuth")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmedAuth(int id)
        {
            var todo = await _context.Todos.SingleOrDefaultAsync(m => m.ID == id);
            _context.Todos.Remove(todo);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.ID == id);
        }
    }
}
