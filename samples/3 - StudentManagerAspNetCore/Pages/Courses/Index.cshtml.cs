using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Pages.Courses
{
    public class IndexModel(StudentManager.ApplicationDbContext context) : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context = context;

        public IList<Course> Course { get; set; }

        public async Task OnGetAsync() => Course = await _context.Courses.IgnoreQueryFilters().ToListAsync();
    }
}
