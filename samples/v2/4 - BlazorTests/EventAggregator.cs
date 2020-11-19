using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlazorTests
{
    /// <summary>
    /// This is NOT a production ready EventAggregator and only used for demo purposes
    /// </summary>
    public class EventAggregator
    {
        public event Action<Count> CountAdded;

        public void PublishCountAdded(Count count)
        {
            CountAdded?.Invoke(count);
        }
    }
}
