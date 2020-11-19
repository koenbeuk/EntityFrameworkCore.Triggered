using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTests
{
    public class Count
    {
        public Guid Id { get; set; }
        public DateTimeOffset CreatedOn { get; set; }
    }

    public class ApplicationDbContext : DbContext
    {
        
    }
}
