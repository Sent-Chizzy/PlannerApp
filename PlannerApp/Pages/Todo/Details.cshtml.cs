using System.Threading.Tasks;                  // For Task and async/await
using Microsoft.AspNetCore.Mvc;                // For IActionResult, NotFound()
using Microsoft.AspNetCore.Mvc.RazorPages;     // Base class for Razor PageModels
using Microsoft.EntityFrameworkCore;           // For FirstOrDefaultAsync
using PlannerApp.Models;                       // Namespace for TodoItem
using TodoApp.Data;                            // Namespace for ApplicationDbContext                       

namespace TodoApp.Pages.Todo
{
    // PageModel class to handle loading a single TodoItem for display
    public class DetailsModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: receives ApplicationDbContext via dependency injection
        public DetailsModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Property to hold the TodoItem we want to display
        public TodoItem TodoItem { get; set; }

        // OnGetAsync runs on HTTP GET /Todo/Details?id=123
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If no id was provided, return 404 Not Found
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the TodoItem whose Id matches the provided id
            TodoItem = await _context.TodoItems.FirstOrDefaultAsync(m => m.Id == id);

            // If not found, return 404
            if (TodoItem == null)
            {
                return NotFound();
            }

            // Otherwise, render the page to display the item
            return Page();
        }
    }
}
