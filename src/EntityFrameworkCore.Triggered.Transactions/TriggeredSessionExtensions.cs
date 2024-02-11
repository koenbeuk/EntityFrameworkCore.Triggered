using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Transactions;
using EntityFrameworkCore.Triggered.Transactions.Internal;
using EntityFrameworkCore.Triggered.Transactions.Lifecycles;

namespace EntityFrameworkCore.Triggered;

public static class TriggeredSessionExtensions
{
    static ITriggerContextDiscoveryStrategy? _beforeCommitTriggerContextDiscoveryStrategy;
    static ITriggerContextDiscoveryStrategy? _afterCommitTriggerContextDiscoveryStrategy;

    static ITriggerContextDiscoveryStrategy? _beforeRollbackTriggerContextDiscoveryStrategy;
    static ITriggerContextDiscoveryStrategy? _afterRollbackTriggerContextDiscoveryStrategy;

    public static void RaiseBeforeCommitTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _beforeCommitTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("BeforeCommit");

        ((TriggerSession)triggerSession).RaiseTriggers(typeof(IBeforeCommitTrigger<>), null, _beforeCommitTriggerContextDiscoveryStrategy, entityType => new BeforeCommitTriggerDescriptor(entityType));
    }

    public static Task RaiseBeforeCommitAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _beforeCommitTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("BeforeCommit");

        return ((TriggerSession)triggerSession).RaiseAsyncTriggers(typeof(IBeforeCommitAsyncTrigger<>), null, _beforeCommitTriggerContextDiscoveryStrategy, entityType => new BeforeCommitAsyncTriggerDescriptor(entityType), cancellationToken);
    }

    public static void RaiseAfterCommitTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _afterCommitTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterCommit");

        ((TriggerSession)triggerSession).RaiseTriggers(typeof(IAfterCommitTrigger<>), null, _afterCommitTriggerContextDiscoveryStrategy, entityType => new AfterCommitTriggerDescriptor(entityType));
    }

    public static Task RaiseAfterCommitAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _afterCommitTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterCommit");

        return ((TriggerSession)triggerSession).RaiseAsyncTriggers(typeof(IAfterCommitAsyncTrigger<>), null, _afterCommitTriggerContextDiscoveryStrategy, entityType => new AfterCommitAsyncTriggerDescriptor(entityType), cancellationToken);
    }

    public static void RaiseBeforeRollbackTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _beforeRollbackTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("BeforeRollback");

        ((TriggerSession)triggerSession).RaiseTriggers(typeof(IBeforeRollbackTrigger<>), null, _beforeRollbackTriggerContextDiscoveryStrategy, entityType => new BeforeRollbackTriggerDescriptor(entityType));
    }

    public static Task RaiseBeforeRollbackAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _beforeRollbackTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("BeforeRollback");

        return ((TriggerSession)triggerSession).RaiseAsyncTriggers(typeof(IBeforeRollbackAsyncTrigger<>), null, _beforeRollbackTriggerContextDiscoveryStrategy, entityType => new BeforeRollbackAsyncTriggerDescriptor(entityType), cancellationToken);
    }

    public static void RaiseAfterRollbackTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _afterRollbackTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterRollback");

        ((TriggerSession)triggerSession).RaiseTriggers(typeof(IAfterRollbackTrigger<>), null, _afterRollbackTriggerContextDiscoveryStrategy, entityType => new AfterRollbackTriggerDescriptor(entityType));
    }

    public static Task RaiseAfterRollbackAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        _afterRollbackTriggerContextDiscoveryStrategy ??= new NonCascadingTriggerContextDiscoveryStrategy("AfterRollback");

        return ((TriggerSession)triggerSession).RaiseAsyncTriggers(typeof(IAfterRollbackAsyncTrigger<>), null, _afterRollbackTriggerContextDiscoveryStrategy, entityType => new AfterRollbackAsyncTriggerDescriptor(entityType), cancellationToken);
    }

    public static void RaiseBeforeCommitStartingTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IBeforeCommitStartingTrigger>();

        foreach (var trigger in triggers)
        {
            trigger.BeforeCommitStarting();
        }
    }


    public async static Task RaiseBeforeCommitStartingAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IBeforeCommitStartingAsyncTrigger>();

        foreach (var trigger in triggers)
        {
            await trigger.BeforeCommitStartingAsync(cancellationToken);
        }
    }

    public static void RaiseBeforeCommitCompletedTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IBeforeCommitCompletedTrigger>();

        foreach (var trigger in triggers)
        {
            trigger.BeforeCommitCompleted();
        }
    }

    public async static Task RaiseBeforeCommitCompletedAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IBeforeCommitCompletedAsyncTrigger>();

        foreach (var trigger in triggers)
        {
            await trigger.BeforeCommitCompletedAsync(cancellationToken);
        }
    }

    public static void RaiseAfterCommitStartingTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IAfterCommitStartingTrigger>();

        foreach (var trigger in triggers)
        {
            trigger.AfterCommitStarting();
        }
    }

    public async static Task RaiseAfterCommitStartingAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IAfterCommitStartingAsyncTrigger>();

        foreach (var trigger in triggers)
        {
            await trigger.AfterCommitStartingAsync(cancellationToken);
        }
    }

    public static void RaiseAfterCommitCompletedTriggers(this ITriggerSession triggerSession)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IAfterCommitCompletedTrigger>();

        foreach (var trigger in triggers)
        {
            trigger.AfterCommitCompleted();
        }
    }

    public async static Task RaiseAfterCommitCompletedAsyncTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(triggerSession);

        if (triggerSession is not TriggerSession typedTriggerSession)
        {
            throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
        }

        var triggers = typedTriggerSession.DiscoveryService.DiscoverTriggers<IAfterCommitCompletedAsyncTrigger>();

        foreach (var trigger in triggers)
        {
            await trigger.AfterCommitCompletedAsync(cancellationToken);
        }
    }
}
