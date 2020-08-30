using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Transactions;
using EntityFrameworkCore.Triggered.Transactions.Internal;

namespace EntityFrameworkCore.Triggered
{
    public static class TriggeredSessionExtensions
    {
        static ITriggerContextDiscoveryStrategy? _beforeCommitTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _afterCommitTriggerContextDiscoveryStrategy;

        static ITriggerContextDiscoveryStrategy? _beforeRollbackTriggerContextDiscoveryStrategy;
        static ITriggerContextDiscoveryStrategy? _afterRollbackTriggerContextDiscoveryStrategy;

        public static Task RaiseBeforeCommitTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (_beforeCommitTriggerContextDiscoveryStrategy == null)
            {
                _beforeCommitTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("BeforeCommit");
            }


            return ((TriggerSession)triggerSession).RaiseTriggers(typeof(IBeforeCommitTrigger<>), null, _beforeCommitTriggerContextDiscoveryStrategy, entityType => new BeforeCommitTriggerDescriptor(entityType), cancellationToken);
        }

        public static Task RaiseAfterCommitTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (_afterCommitTriggerContextDiscoveryStrategy == null)
            {
                _afterCommitTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterCommit");
            }

            return ((TriggerSession)triggerSession).RaiseTriggers(typeof(IAfterCommitTrigger<>), null, _afterCommitTriggerContextDiscoveryStrategy, entityType => new AfterCommitTriggerDescriptor(entityType), cancellationToken);
        }
        public static Task RaiseBeforeRollbackTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (_beforeRollbackTriggerContextDiscoveryStrategy == null)
            {
                _beforeRollbackTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("BeforeRollback");
            }

            return ((TriggerSession)triggerSession).RaiseTriggers(typeof(IBeforeRollbackTrigger<>), null, _beforeRollbackTriggerContextDiscoveryStrategy, entityType => new BeforeRollbackTriggerDescriptor(entityType), cancellationToken);

        }

        public static Task RaiseAfterRollbackTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (_afterRollbackTriggerContextDiscoveryStrategy == null)
            {
                _afterRollbackTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterRollback");
            }


            return ((TriggerSession)triggerSession).RaiseTriggers(typeof(IAfterRollbackTrigger<>), null, _afterRollbackTriggerContextDiscoveryStrategy, entityType => new AfterRollbackTriggerDescriptor(entityType), cancellationToken);
        }

    }
}
