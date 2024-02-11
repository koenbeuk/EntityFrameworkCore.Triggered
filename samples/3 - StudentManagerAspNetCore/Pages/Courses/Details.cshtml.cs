using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Pages.Courses
{
    public class DetailsModel(StudentManager.ApplicationDbContext context) : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context = context;

        public Course Course { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Course = await _context.Courses.FirstOrDefaultAsync(m => m.Id == id);

            if (Course == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
