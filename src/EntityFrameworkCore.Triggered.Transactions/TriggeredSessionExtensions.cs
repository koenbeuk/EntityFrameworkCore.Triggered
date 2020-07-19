using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
                _beforeCommitTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("BeforeCommit", typeof(IBeforeCommitTrigger<>), trigger => new BeforeCommitTriggerAdapter(trigger));
            }


            return ((TriggerSession)triggerSession).RaiseTriggers(_beforeCommitTriggerContextDiscoveryStrategy, cancellationToken);
        }

        public static Task RaiseAfterCommitTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (_afterCommitTriggerContextDiscoveryStrategy == null)
            {
                _afterCommitTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterCommit", typeof(IAfterCommitTrigger<>), trigger => new AfterCommitTriggerAdapter(trigger));
            }


            return ((TriggerSession)triggerSession).RaiseTriggers(_afterCommitTriggerContextDiscoveryStrategy, cancellationToken);
        }
        public static Task RaiseBeforeRollbackTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (_beforeRollbackTriggerContextDiscoveryStrategy == null)
            {
                _beforeRollbackTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("BeforeRollback", typeof(IBeforeRollbackTrigger<>), trigger => new BeforeRollbackTriggerAdapter(trigger));
            }


            return ((TriggerSession)triggerSession).RaiseTriggers(_beforeRollbackTriggerContextDiscoveryStrategy, cancellationToken);
        }

        public static Task RaiseAfterRollbackTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (_afterRollbackTriggerContextDiscoveryStrategy == null)
            {
                _afterRollbackTriggerContextDiscoveryStrategy = new NonRecursiveTriggerContextDiscoveryStrategy("AfterRollback", typeof(IAfterRollbackTrigger<>), trigger => new AfterRollbackTriggerAdapter(trigger));
            }


            return ((TriggerSession)triggerSession).RaiseTriggers(_afterRollbackTriggerContextDiscoveryStrategy, cancellationToken);
        }

    }
}
