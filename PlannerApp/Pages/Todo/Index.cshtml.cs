using System.Collections.Generic;                 // For List<T>
using System.Threading.Tasks;                      // For Task and async/await
using Microsoft.AspNetCore.Mvc.RazorPages;         // Base class for Razor PageModels
using Microsoft.EntityFrameworkCore;
using PlannerApp.Models;                             // Namespace for TodoItem
using TodoApp.Data;                                // Namespace for ApplicationDbContext                         

namespace TodoApp.Pages.Todo
{
    // The IndexModel class handles data and actions for the Index Razor Page
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        // Constructor: ASP.NET Core will inject an instance of ApplicationDbContext
        public IndexModel(ApplicationDbContext context)
        {
            _context = context;
        }

        // This property will hold all to-do items to display on the page
        public IList<TodoItem> TodoItems { get; set; }

        // OnGetAsync runs when the Index page is accessed via HTTP GET
        public async Task OnGetAsync()
        {
            // Fetch all TodoItem records from the database, asynchronously
            TodoItems = await _context.TodoItems.ToListAsync();
        }
    }
}
