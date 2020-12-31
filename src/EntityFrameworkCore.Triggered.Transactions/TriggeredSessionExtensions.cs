using System;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using EntityFrameworkCore.Triggered.Transactions;
using EntityFrameworkCore.Triggered.Transactions.Abstractions.Lifecycles;
using EntityFrameworkCore.Triggered.Transactions.Internal;
using Microsoft.Extensions.DependencyInjection;

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

        public static async Task RaiseBeforeCommitStartingTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (triggerSession is not TriggerSession typedTriggerSession)
            {
                throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
            }

            var triggers = typedTriggerSession.DiscoveryService.ServiceProvider.GetServices<IBeforeCommitStartingTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.BeforeCommitStarting(cancellationToken);
            }
        }

        public static async Task RaiseBeforeCommitCompletedTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (triggerSession is not TriggerSession typedTriggerSession)
            {
                throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
            }

            var triggers = typedTriggerSession.DiscoveryService.ServiceProvider.GetServices<IBeforeCommitCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.BeforeCommitCompleted(cancellationToken);
            }
        }

        public static async Task RaiseAfterCommitStartingTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (triggerSession is not TriggerSession typedTriggerSession)
            {
                throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
            }

            var triggers = typedTriggerSession.DiscoveryService.ServiceProvider.GetServices<IAfterCommitStartingTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterCommitStarting(cancellationToken);
            }
        }

        public static async Task RaiseAfterCommitCompletedTriggers(this ITriggerSession triggerSession, CancellationToken cancellationToken = default)
        {
            if (triggerSession == null)
            {
                throw new ArgumentNullException(nameof(triggerSession));
            }

            if (triggerSession is not TriggerSession typedTriggerSession)
            {
                throw new InvalidOperationException("Method is implemented for concrete TriggerSessions only");
            }

            var triggers = typedTriggerSession.DiscoveryService.ServiceProvider.GetServices<IAfterCommitCompletedTrigger>();

            foreach (var trigger in triggers)
            {
                await trigger.AfterCommitCompleted(cancellationToken);
            }
        }

    }
}
