using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public class TriggerOptions
    {
        public int MaxRecursion { get; set; } = 100;
    }
}
