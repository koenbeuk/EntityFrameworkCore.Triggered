using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Triggered.Internal
{
    public sealed class EntityBagStateManager
    {
        private readonly Dictionary<object, IDictionary<object, object>> _resolvedBags = new();

        public IDictionary<object, object> GetForEntity(object entity)
        {
            if (!_resolvedBags.TryGetValue(entity, out var bag))
            {
                bag = new Dictionary<object, object>();
                _resolvedBags.Add(entity, bag);
            }

            return bag;
        }
    }
}
