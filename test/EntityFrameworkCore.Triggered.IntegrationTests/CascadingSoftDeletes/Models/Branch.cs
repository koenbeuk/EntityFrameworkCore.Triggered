using System;
using System.Collections.Generic;

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
