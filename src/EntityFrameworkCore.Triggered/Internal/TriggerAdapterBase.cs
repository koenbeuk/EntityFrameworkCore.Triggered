using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;

namespace EntityFrameworkCore.Triggered.Internal
{
    public abstract class TriggerAdapterBase
    {
        public TriggerAdapterBase(object trigger)
        {
            if (trigger == null)
            {
                throw new ArgumentNullException(nameof(trigger));
            }

            Debug.Assert(TypeHelpers.FindGenericInterfaces(trigger.GetType(), typeof(IBeforeSaveTrigger<>)) != null);

            Trigger = trigger;
        }

        protected object Trigger { get; }

        protected Task Execute(string methodName, object triggerContext, CancellationToken cancellationToken)
        {
            var methodInfo = Trigger
                .GetType()
                .GetMethod(methodName);

            if (methodInfo == null)
            {
                throw new InvalidOperationException("No such method found");
            }

            var result = (Task)methodInfo.Invoke(Trigger, new object[] { triggerContext, cancellationToken });

            return result;
        }

        public abstract Task Execute(object triggerContext, CancellationToken cancellationToken);
    }
}
 