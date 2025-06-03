using System.Linq;                       // For Where(...)
using System.Threading.Tasks;            // For Task and async/await
using Microsoft.AspNetCore.Mvc;          // For IActionResult, [BindProperty], NotFound(), RedirectToPage, ModelState
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;      // For FirstOrDefaultAsync, AnyAsync, DbUpdateConcurrencyException
using PlannerApp.Models;
using TodoApp.Data;


namespace TodoApp.Pages.Todo
{
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Binds form data (Id, Title, IsDone) into this property
        [BindProperty]
        public TodoItem TodoItem { get; set; }

        // Loads the existing item on GET /Todo/Edit?id=123
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            // Load the item from the database
            TodoItem = await _context.TodoItems.FirstOrDefaultAsync(m => m.Id == id);

            if (TodoItem == null)
            {
                return NotFound();
            }

            return Page();
        }

        // Saves changes when the edit form is submitted
        public async Task<IActionResult> OnPostAsync()
        {
            // 1) Validate required fields
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 2) Check if any other item (with a different Id) already has this Title
            bool titleInUse = await _context.TodoItems
                .Where(t => t.Id != TodoItem.Id)             // exclude current record
                .AnyAsync(t => t.Title.ToLower() == TodoItem.Title.ToLower());

            if (titleInUse)
            {
                // 2a) Add an error to the Title field
                ModelState.AddModelError(
                    "TodoItem.Title",
                    "Another to-do with this title already exists."
                );
                return Page(); // Redisplay form with error
            }

            // 3) Attach this entity to the context and mark it Modified
            _context.Attach(TodoItem).State = EntityState.Modified;

            try
            {
                // 4) Save changes
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the item no longer exists, return 404; otherwise rethrow
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

            // 5) Redirect back to the list
            return RedirectToPage("Index");
        }
    }
}
