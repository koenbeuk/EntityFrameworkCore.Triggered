using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Pages.Students;

public class DetailsModel(StudentManager.ApplicationDbContext context) : PageModel
{
    private readonly StudentManager.ApplicationDbContext _context = context;

    public Student Student { get; set; }

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        Student = await _context.Students.FirstOrDefaultAsync(m => m.Id == id);

        if (Student == null)
        {
            return NotFound();
        }
        return Page();
    }
}
