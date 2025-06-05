using System.Linq;                        // For AnyAsync, Where
using System.Threading.Tasks;             // For Task and async/await
using Microsoft.AspNetCore.Mvc;           // For IActionResult, [BindProperty], NotFound(), RedirectToPage
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;       // For FirstOrDefaultAsync, AnyAsync, DbUpdateConcurrencyException
using PlannerApp.Data;                     // Your ApplicationDbContext namespace
using PlannerApp.Models;                   // Your TodoItem & Difficulty enum namespace

namespace PlannerApp.Pages.Todo
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: ASP.NET Core will inject ApplicationDbContext
        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Binds all form fields—including Progress and IsDone—into this property.
        /// </summary>
        [BindProperty]
        public TodoItem TodoItem { get; set; }

        /// <summary>
        /// OnGetAsync runs on HTTP GET /Todo/Edit?id=123.
        /// It loads the existing TodoItem from the database by its Id.
        /// </summary>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the TodoItem by Id
            TodoItem = await _context.TodoItems.FirstOrDefaultAsync(m => m.Id == id);

            if (TodoItem == null)
            {
                return NotFound();
            }

            return Page();
        }

        /// <summary>
        /// OnPostAsync runs when the form is submitted (POST) from /Todo/Edit?id=123.
        /// Validates, prevents duplicate titles, clamps progress, and saves changes.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // 1) If validation fails (e.g. Title required), redisplay form
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 2) Prevent duplicate titles (case-insensitive) for items other than this one
            bool titleInUse = await _context.TodoItems
                .Where(t => t.Id != TodoItem.Id)
                .AnyAsync(t => t.Title.ToLower() == TodoItem.Title.ToLower());

            if (titleInUse)
            {
                ModelState.AddModelError(
                    "TodoItem.Title",
                    "Another to-do with this title already exists."
                );
                return Page();
            }

            // 3) If slider moved to 100 or checkbox checked, mark as completed and clamp progress
            if (TodoItem.Progress >= 100 || TodoItem.IsDone)
            {
                TodoItem.Progress = 100;
                TodoItem.IsDone = true;
            }
            else
            {
                // If progress < 100 but user checked the box, force progress to 100
                if (TodoItem.IsDone)
                {
                    TodoItem.Progress = 100;
                }
                // Otherwise, if user slides below 100, ensure IsDone = false
                else
                {
                    TodoItem.IsDone = false;
                }
            }

            // 4) Attach the modified TodoItem to the context and save
            _context.Attach(TodoItem).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If task was deleted by another process, return 404
                bool exists = await _context.TodoItems.AnyAsync(e => e.Id == TodoItem.Id);
                if (!exists)
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            // 5) Redirect back to the Index (list) page
            return RedirectToPage("Index");
        }
    }
}
