using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace StudentManager.Pages
{
    public class AuditsModel : PageModel
    {
        private readonly StudentManager.ApplicationDbContext _context;

        public AuditsModel(StudentManager.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Audit> Audit { get; set; }

        public async Task OnGetAsync()
        {
            Audit = await _context.Audits.ToListAsync();
        }
    }
}
