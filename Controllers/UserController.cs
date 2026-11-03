using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GroupProject325.Data;
using GroupProject325.Models;

namespace GroupProject325.Controllers
{
    // Every action requires a signed-in (OAuth) user.
    [Authorize]
    public class UsersController : Controller
    {
        private readonly AppDbContext _context;

        public UsersController(AppDbContext context)
        {
            _context = context;
        }

        // GET: Users
        // Lists the members of the signed-in user's party, with an optional name search.
        public async Task<IActionResult> Index(string searchString)
        {
            var current = await GetCurrentUserAsync();
            if (current == null)
            {
                // If they are signed in with OAuth but no User row yet, send them to register.
                return RedirectToAction(nameof(Create));
            }

            // A user who has not joined a party has no one to list.
            if (current.PartyId == null)
            {
                return View(new List<User>());
            }

            var users = from u in _context.Users
                        where u.PartyId == current.PartyId
                        select u;

            if (!string.IsNullOrEmpty(searchString))
            {
                users = users.Where(u => u.UserName.ToUpper().Contains(searchString.ToUpper()));
            }

            return View(await users.OrderBy(u => u.UserName).ToListAsync());
        }

        // GET: Users/Details/Id
        // A user can view themselves and anyone in their own party.
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var current = await GetCurrentUserAsync();
            if (current == null || (current.Id != user.Id &&
                (current.PartyId == null || current.PartyId != user.PartyId)))
            {
                return Forbid();
            }

            return View(user);
        }

        // GET: Users/Create
        // Shows the registration form. The email comes from OAuth, so it is not typed in.
        public IActionResult Create()
        {
            return View();
        }

        // POST: Users/Create
        // Only UserName is bound from the form. Email comes from the OAuth claim, and
        // Role and PartyId are never taken from the form, to protect from overposting.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserName")] User user)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return Unauthorized();
            }

            // If they are already registered, go to their existing account instead of making a 
            // duplicate.
            var existing = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
            if (existing != null)
            {
                return RedirectToAction(nameof(Details), new { id = existing.Id });
            }

            user.Email = email;
            ModelState.Remove("Email"); // Email was not on the form, it was set above.

            if (ModelState.IsValid)
            {
                user.UserName = user.UserName.Trim();
                user.Role = PartyRole.Member;          // GM is assigned when a party is created.
                user.Inventory = new UserInventory();  // Every user has exactly one inventory.

                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = user.Id });
            }
            return View(user);
        }

        // GET: Users/Edit/Id
        // A user can only edit their own profile.
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var current = await GetCurrentUserAsync();
            if (current == null || current.Id != user.Id)
            {
                return Forbid();
            }
            return View(user);
        }

        // POST: Users/Edit/Id
        // Only Id and UserName are bound, so Email, Role, and PartyId cannot be changed here.
        // The existing row is loaded and updated instead of calling Update(user), because
        // Update would overwrite the unbound columns with empty values.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,UserName")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            var current = await GetCurrentUserAsync();
            if (current == null || current.Id != id)
            {
                return Forbid();
            }

            ModelState.Remove("Email"); // Email is not editable, so it is not on the form.

            if (ModelState.IsValid)
            {
                try
                {
                    current.UserName = user.UserName.Trim();
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
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
            return View(user);
        }

        // POST: Users/ChangeRole/Id
        // Only the GM of a party can change the role of a member of that same party.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeRole(Guid id, PartyRole role)
        {
            var current = await GetCurrentUserAsync();
            if (current == null || current.Role != PartyRole.GM || current.PartyId == null)
            {
                return Forbid();
            }

            var target = await _context.Users.FindAsync(id);
            if (target == null)
            {
                return NotFound();
            }
            if (target.PartyId != current.PartyId)
            {
                return Forbid();
            }

            target.Role = role;
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = target.Id });
        }

        // GET: Users/Delete/Id
        // A user can only delete their own account.
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .FirstOrDefaultAsync(m => m.Id == id);
            if (user == null)
            {
                return NotFound();
            }

            var current = await GetCurrentUserAsync();
            if (current == null || current.Id != user.Id)
            {
                return Forbid();
            }

            return View(user);
        }

        // POST: Users/Delete/Id
        // The user's inventory is deleted with them.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var current = await GetCurrentUserAsync();
            if (current == null || current.Id != id)
            {
                return Forbid();
            }

            _context.Users.Remove(current);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }

        // Finds the User row that matches the signed-in OAuth identity.
        private async Task<User?> GetCurrentUserAsync()
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            if (string.IsNullOrWhiteSpace(email))
            {
                return null;
            }
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}