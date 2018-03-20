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
            // Load the notes for the currently logged in user only.
            return View(await _context.Todos.Where(note => note.OwnerID == User.Identity.Name).ToListAsync());
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
            bool check = true;
            if (ModelState.IsValid)
            {
                // Iterate through keys to check for duplicate titles.
                foreach (var key in HttpContext.Session.Keys)
                {
                    if (key == todo.Title)
                    {
                        check = false;
                    }
                }
                // If we do not have duplicate titles, perform create.
                if (check)
                {
                    HttpContext.Session.SetString(todo.Title, todo.Description);
                    return RedirectToAction(nameof(Index));
                }
                else
                {
                    // Otherwise set DupError and return view.
                    ViewData["DupError"] = "No duplicate note titles allowed.";
                    return View(todo);
                }
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
        public async Task<IActionResult> CreateAuth([Bind("OwnerID,Title,Description,ExpireDate")] Todo todo)
        {
            if (ModelState.IsValid)
            {
                _context.Add(todo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(IndexAuth));
            }
            return View(todo);
        }

        // GET: Todos/Edit?key=
        public IActionResult Edit(string key)
        {
            ViewData["Key"] = key;
            return View();
        }

        // POST: Todos/Edit?key=
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(string key, [Bind("Title, Description")]Todo todo)
        {
            if (ModelState.IsValid)
            {
                HttpContext.Session.SetString(todo.Title, todo.Description);
                return RedirectToAction(nameof(Index));
            }
            //return View(todo);
            return Content($"Hello {key}, {todo.Description}");
        }

        // GET: Todos/EditAuth/5
        [Authorize]
        public async Task<IActionResult> EditAuth(int? id)
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

        // POST: Todos/EditAuth/5
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditAuth(int id, [Bind("ID,OwnerID,Title,Description,IsDone,ExpireDate")] Todo todo)
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
                return RedirectToAction(nameof(IndexAuth));
            }
            return View(todo);
        }

        // GET: Todos/Delete?key=
        public IActionResult Delete(string key)
        {
            ViewData["Key"] = key;
            return View();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(string key)
        {
            // Remove a note by looking up the session state[key]
            HttpContext.Session.Remove(key);
            return RedirectToAction(nameof(Index));
            //return Content($"Hello {key}");
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

        [Authorize]
        public IActionResult DeleteAllDone()
        {
            return View();
        }

        [Authorize]
        [HttpPost, ActionName("DeleteAllDone")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAllDoneConfirmed()
        {
            // Find all 'done' todos of the logged in user.
            var todos = from todo in _context.Todos
                        where todo.OwnerID == User.Identity.Name && todo.IsDone == true
                        select todo;

            // Execute query.
            foreach (var todo in todos)
            {
                _context.Todos.Remove(todo);
            }
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(IndexAuth));
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
            return RedirectToAction(nameof(IndexAuth));
        }

        private bool TodoExists(int id)
        {
            return _context.Todos.Any(e => e.ID == id);
        }
    }
}
