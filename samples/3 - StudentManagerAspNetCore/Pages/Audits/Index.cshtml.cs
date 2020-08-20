using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using StudentManager;

namespace StudentManager.Pages
{
    public class AuditsModel : PageModel
    {
        private readonly StudentManager.ApplicationContext _context;

        public AuditsModel(StudentManager.ApplicationContext context)
        {
            _context = context;
        }

        public IList<Audit> Audit { get;set; }

        public async Task OnGetAsync()
        {
            Audit = await _context.Audits.ToListAsync();
        }
    }
}
