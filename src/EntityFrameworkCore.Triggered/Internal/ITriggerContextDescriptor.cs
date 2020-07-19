using System;
using System.Collections.Generic;
using System.Text;

namespace EntityFrameworkCore.Triggered.Internal
{
    public interface ITriggerContextDescriptor
    {
        ChangeType ChangeType { get; }

        object Entity { get; }

        Type EntityType { get; }

        object GetTriggerContext();
    }
}
