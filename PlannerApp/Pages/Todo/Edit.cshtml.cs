using System.Threading.Tasks;                  // For Task and async/await
using Microsoft.AspNetCore.Mvc;                // For IActionResult, [BindProperty], NotFound(), RedirectToPage
using Microsoft.AspNetCore.Mvc.RazorPages;     // Base class for Razor PageModels
using Microsoft.EntityFrameworkCore;           // For FindAsync, FirstOrDefaultAsync, and handling DbUpdateConcurrencyException
using PlannerApp.Models;                       // Namespace for TodoItem
using TodoApp.Data;                            // Namespace for ApplicationDbContext

namespace TodoApp.Pages.Todo
{
    // PageModel class to handle loading an existing TodoItem and saving edits
    public class EditModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: receives ApplicationDbContext via dependency injection
        public EditModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // The [BindProperty] attribute binds form values to this TodoItem property
        [BindProperty]
        public TodoItem TodoItem { get; set; }

        // OnGetAsync runs on HTTP GET /Todo/Edit?id=123
        // It loads the existing TodoItem from the database by its primary key (Id)
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // If no id was provided in the query string, return 404
            if (id == null)
            {
                return NotFound();
            }

            // Fetch the TodoItem whose Id matches the provided id
            TodoItem = await _context.TodoItems.FirstOrDefaultAsync(m => m.Id == id);

            // If no TodoItem found, return 404
            if (TodoItem == null)
            {
                return NotFound();
            }

            // Otherwise, render the page and pre-populate the form with loaded data
            return Page();
        }

        // OnPostAsync runs on HTTP POST when the edit form is submitted
        public async Task<IActionResult> OnPostAsync()
        {
            // If the submitted data fails validation, re-display the form with errors
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Attach the TodoItem to the context and mark it as modified
            _context.Attach(TodoItem).State = EntityState.Modified;

            try
            {
                // Save the changes back to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                // If the item was deleted by another process in between, check if it still exists
                bool exists = await _context.TodoItems.AnyAsync(e => e.Id == TodoItem.Id);
                if (!exists)
                {
                    // If it no longer exists, return 404
                    return NotFound();
                }
                else
                {
                    // Otherwise rethrow the exception to bubble up the error
                    throw;
                }
            }

            // After saving successfully, redirect back to the Index (list) page
            return RedirectToPage("Index");
        }
    }
}
