using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Triggers.Internal
{
    public interface IChangeEventDescriptor
    {
        ChangeType Type { get; }

        object Entity { get; }

        Type EntityType { get; }

        object GetChangeEvent();
    }
}
