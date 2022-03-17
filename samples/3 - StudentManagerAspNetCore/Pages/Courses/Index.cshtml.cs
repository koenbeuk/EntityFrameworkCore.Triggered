using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context;

        public IndexModel(StudentManager.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Course> Course { get; set; }

        public async Task OnGetAsync()
        {
            Course = await _context.Courses.IgnoreQueryFilters().ToListAsync();
        }
    }
}
