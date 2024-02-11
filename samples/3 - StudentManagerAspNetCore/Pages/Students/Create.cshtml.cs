using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace StudentManager.Pages.Students;

public class CreateModel(StudentManager.ApplicationDbContext context) : PageModel
{
    private readonly StudentManager.ApplicationDbContext _context = context;

    public IActionResult OnGet() => Page();

    [BindProperty]
    public Student Student { get; set; }

    // To protect from overposting attacks, enable the specific properties you want to bind to, for
    // more details, see https://aka.ms/RazorPagesCRUD.
    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        _context.Students.Add(Student);
        await _context.SaveChangesAsync();

        return RedirectToPage("./Index");
    }
}
