using System.Threading.Tasks;                  // For Task and async/await
using Microsoft.AspNetCore.Mvc;                // For IActionResult, [BindProperty], NotFound(), RedirectToPage
using Microsoft.AspNetCore.Mvc.RazorPages;     // Base class for Razor PageModels
using PlannerApp.Models;                       // Namespace for TodoItem
using PlannerApp.Data;                            // Namespace for ApplicationDbContext                      

namespace TodoApp.Pages.Todo
{
    // PageModel class to handle loading the item to confirm deletion and then deleting it
    public class DeleteModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: receives ApplicationDbContext via dependency injection
        public DeleteModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Property to hold the TodoItem being deleted
        [BindProperty]
        public TodoItem TodoItem { get; set; }

        // OnGetAsync runs on HTTP GET /Todo/Delete?id=123
        // Load the existing TodoItem to display its details for confirmation
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If no id was provided, return 404 Not Found
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the TodoItem by its Id
            TodoItem = await _context.TodoItems.FindAsync(id);

            // If not found, return 404
            if (TodoItem == null)
            {
                return NotFound();
            }

            // Render the page to show the delete confirmation
            return Page();
        }

        // OnPostAsync runs when the confirmation form is submitted (POST request)
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            // If no id was provided, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Find the item by its Id
            TodoItem = await _context.TodoItems.FindAsync(id);

            // If found, remove it
            if (TodoItem != null)
            {
                _context.TodoItems.Remove(TodoItem);
                await _context.SaveChangesAsync();
            }

            // Redirect back to the Index page after deletion
            return RedirectToPage("Index");
        }
    }
}
