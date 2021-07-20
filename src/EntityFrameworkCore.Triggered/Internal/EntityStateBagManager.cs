using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class EntityBagStateManager
    {
        private readonly Dictionary<object, IDictionary<string, object>> _resolvedBags = new();

        public IDictionary<string, object> GetForEntity(object entity)
        {
            if (!_resolvedBags.TryGetValue(entity, out var bag))
            {
                bag = new Dictionary<string, object>();
                _resolvedBags.Add(entity, bag);
            }

            return bag;
        }
    }
}
