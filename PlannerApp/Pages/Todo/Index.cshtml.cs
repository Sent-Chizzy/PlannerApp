using System.Collections.Generic;                   // For IList<T>
using System.Threading.Tasks;                        // For Task and async/await
using Microsoft.AspNetCore.Mvc;                      // For IActionResult, [BindProperty], RedirectToPage
using Microsoft.AspNetCore.Mvc.RazorPages;           // Base class for Razor PageModels
using Microsoft.EntityFrameworkCore;                 // For ToListAsync(), FindAsync()
using PlannerApp.Data;                               // Your ApplicationDbContext namespace
using PlannerApp.Models;                             // Your TodoItem model & Difficulty enum namespace

namespace PlannerApp.Pages.Todo
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // Property to hold all to-do items (including Progress, IsDone, etc.)
        public IList<TodoItem> TodoItems { get; set; }

        // OnGetAsync: load all items for display
        public async Task OnGetAsync()
        {
            TodoItems = await _context.TodoItems.ToListAsync();
        }

        // New handler: runs when user clicks the "Complete" button for a given Id
        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            // 1) Load the item from the database
            var item = await _context.TodoItems.FindAsync(id);

            if (item == null)
            {
                // If not found, return 404
                return NotFound();
            }

            // 2) Mark it as completed: set progress to 100 and IsDone = true
            item.Progress = 100;
            item.IsDone = true;

            // 3) Save changes
            await _context.SaveChangesAsync();

            // 4) Redirect back to the Index page to see updated status
            return RedirectToPage();
        }
    }
}
