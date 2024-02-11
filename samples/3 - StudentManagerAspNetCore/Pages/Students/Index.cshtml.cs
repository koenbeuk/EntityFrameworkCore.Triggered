using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Pages.Students
{
    public class IndexModel(StudentManager.ApplicationDbContext context) : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context = context;

        public IList<Student> Student { get; set; }

        public async Task OnGetAsync() => Student = await _context.Students.Include(x => x.Courses).ThenInclude(x => x.Course).ToListAsync();
    }
}
