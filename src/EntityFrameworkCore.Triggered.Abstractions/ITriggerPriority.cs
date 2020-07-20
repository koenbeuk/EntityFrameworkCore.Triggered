using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered
{
    public interface ITriggerPriority
    {
        /// <summary>
        /// Get the priority number (lower means earlier) for when this trigger is supposed to be invoked
        /// </summary>
        public int Priority { get; }
    }
}
