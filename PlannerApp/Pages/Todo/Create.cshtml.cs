using System.Threading.Tasks;               // For Task and async/await
using Microsoft.AspNetCore.Mvc;             // For IActionResult, [BindProperty], RedirectToPage
using Microsoft.AspNetCore.Mvc.RazorPages;  // Base class for Razor PageModels
using PlannerApp.Models;                    // Namespace for TodoItem
using TodoApp.Data;                         // Namespace for ApplicationDbContext                       

namespace TodoApp.Pages.Todo
{
    // PageModel class to back the Create.cshtml Razor view
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;

        // Constructor: receives an instance of ApplicationDbContext via dependency injection
        public CreateModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // The [BindProperty] attribute tells Razor to bind form fields to this property
        [BindProperty]
        public TodoItem TodoItem { get; set; }

        // OnGet runs when a GET request is made to /Todo/Create
        public IActionResult OnGet()
        {
            // Simply render the page; no data to load yet
            return Page();
        }

        // OnPostAsync runs when the form is submitted (POST request)
        public async Task<IActionResult> OnPostAsync()
        {
            // If the submitted form data doesn’t pass validation, redisplay the form
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Add the new TodoItem to the DbContext’s tracking
            _context.TodoItems.Add(TodoItem);

            // Save the new item to the database asynchronously
            await _context.SaveChangesAsync();

            // After saving, redirect back to the Index page to see the full list
            return RedirectToPage("Index");
        }
    }
}
