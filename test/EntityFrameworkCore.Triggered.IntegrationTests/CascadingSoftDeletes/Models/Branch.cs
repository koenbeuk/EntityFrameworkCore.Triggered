using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;

namespace EntityFrameworkCore.Triggered.IntegrationTests.CascadingSoftDeletes.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public DateTime? DeletedOn { get; set; }

        public Branch Parent { get; set; }
        public ICollection<Branch> Children { get; set; }
        
    }
}
