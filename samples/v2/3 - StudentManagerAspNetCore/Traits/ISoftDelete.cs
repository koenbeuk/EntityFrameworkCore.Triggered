using System;

namespace StudentManager.Traits
{
    public interface ISoftDelete
    {
        DateTimeOffset? DeletedOn { get; set; }
    }
}
