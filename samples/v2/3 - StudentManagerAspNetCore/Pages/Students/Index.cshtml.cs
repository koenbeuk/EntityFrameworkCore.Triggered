using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManager;

namespace StudentManager.Pages.Students
{
    public class IndexModel : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context;

        public IndexModel(StudentManager.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Student> Student { get;set; }

        public async Task OnGetAsync()
        {
            Student = await _context.Students.Include(x => x.Courses).ThenInclude(x => x.Course).ToListAsync();
        }
    }
}
