using System.Linq;                       // For AnyAsync
using System.Threading.Tasks;            // For Task and async/await
using Microsoft.AspNetCore.Mvc;          // For IActionResult, [BindProperty], RedirectToPage, ModelState
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;      // For AnyAsync
using PlannerApp.Models;
using TodoApp.Data;


namespace TodoApp.Pages.Todo
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Binds form data (Title, IsDone) into this property
        [BindProperty]
        public TodoItem TodoItem { get; set; }

        // Runs on GET /Todo/Create
        public IActionResult OnGet()
        {
            return Page();
        }

        // Runs on POST when the form is submitted
        public async Task<IActionResult> OnPostAsync()
        {
            // 1) Validate required fields (Title, etc.)
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 2) Check if another item already has the same Title (case-insensitive)
            bool titleExists = await _context.TodoItems
                .AnyAsync(t => t.Title.ToLower() == TodoItem.Title.ToLower());

            if (titleExists)
            {
                // 2a) Add a field-specific error so the user sees the message under Title
                ModelState.AddModelError(
                    "TodoItem.Title",
                    "A to-do with this title already exists."
                );
                return Page(); // Redisplay the form with the validation error
            }

            // 3) No duplicate found ? add and save
            _context.TodoItems.Add(TodoItem);
            await _context.SaveChangesAsync();

            // 4) Redirect back to the list view
            return RedirectToPage("Index");
        }
    }
}
