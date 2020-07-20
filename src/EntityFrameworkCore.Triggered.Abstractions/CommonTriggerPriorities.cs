using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public static class CommonTriggerPriority
    {
        public const int Earlier = -2;
        public const int Early = -1;
        public const int Normal = 0;
        public const int Late = 1;
        public const int Later = 2;
    }
}
