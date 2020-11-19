using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManager;

namespace StudentManager.Pages.Courses
{
    public class IndexModel : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context;

        public IndexModel(StudentManager.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Course> Course { get;set; }

        public async Task OnGetAsync()
        {
            Course = await _context.Courses.IgnoreQueryFilters().ToListAsync();
        }
    }
}
