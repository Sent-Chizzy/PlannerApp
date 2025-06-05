using System.Linq;                        // For AnyAsync
using System.Threading.Tasks;             // For Task and async/await
using Microsoft.AspNetCore.Mvc;           // For IActionResult, [BindProperty], RedirectToPage, ModelState
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;       // For AnyAsync
using PlannerApp.Data;                     // Your ApplicationDbContext namespace
using PlannerApp.Models;                   // Your TodoItem & Difficulty enum namespace

namespace PlannerApp.Pages.Todo
{
    /// <summary>
    /// PageModel class to back the Create.cshtml Razor view.
    /// Handles GET (show empty form) and POST (validate + save new to-do).
    /// </summary>
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: ASP.NET Core injects an instance of ApplicationDbContext
        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Binds form fields (Title, Description, DueDate, DifficultyLevel, IsDone) to this property.
        /// </summary>
        [BindProperty]
        public TodoItem TodoItem { get; set; }

        /// <summary>
        /// OnGet handler: simply renders the empty form.
        /// </summary>
        public IActionResult OnGet()
        {
            return Page();
        }

        /// <summary>
        /// OnPostAsync handler: runs when the form submits (POST to /Todo/Create).
        /// Validates, checks for duplicates, then saves if valid.
        /// </summary>
        public async Task<IActionResult> OnPostAsync()
        {
            // 1) Ensure required fields pass validation (Title, DifficultyLevel)
            if (!ModelState.IsValid)
            {
                return Page(); // Redisplay with validation messages
            }

            // 2) Prevent duplicate Titles (case-insensitive)
            bool titleExists = await _context.TodoItems
                .AnyAsync(t => t.Title.ToLower() == TodoItem.Title.ToLower());

            if (titleExists)
            {
                // Attach a validation error specifically to the Title field
                ModelState.AddModelError(
                    "TodoItem.Title",
                    "A to-do with this title already exists."
                );
                return Page(); // Redisplay form with the error shown under Title
            }

            // 3) No duplicate found ? add the new TodoItem to the context
            _context.TodoItems.Add(TodoItem);

            // 4) Save changes asynchronously to the database
            await _context.SaveChangesAsync();

            // 5) Redirect back to the Index (list) page to see the new to-do
            return RedirectToPage("Index");
        }
    }
}
