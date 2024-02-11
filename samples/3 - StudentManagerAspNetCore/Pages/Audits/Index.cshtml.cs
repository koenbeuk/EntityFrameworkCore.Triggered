using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Pages
{
    public class AuditsModel(StudentManager.ApplicationDbContext context) : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context = context;

        public IList<Audit> Audit { get; set; }

        public async Task OnGetAsync() => Audit = await _context.Audits.ToListAsync();
    }
}
