using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Infrastructure.Internal;

namespace EntityFrameworkCore.Triggers.Internal
{
    public abstract class TriggerAdapterBase
    {
        public TriggerAdapterBase(object changeHandler)
        {
            if (changeHandler == null)
            {
                throw new ArgumentNullException(nameof(changeHandler));
            }

            Debug.Assert(TypeHelpers.FindGenericInterfaces(changeHandler.GetType(), typeof(IBeforeSaveTrigger<>)) != null);

            ChangeHandler = changeHandler;
        }

        protected object ChangeHandler { get; }

        protected Task Execute(string methodName, object trigger, CancellationToken cancellationToken)
        {
            var methodInfo = ChangeHandler
                .GetType()
                .GetMethod(methodName);

            if (methodInfo == null)
            {
                throw new InvalidOperationException("No such method found");
            }

            var result = (Task)methodInfo.Invoke(ChangeHandler, new object[] { trigger, cancellationToken });

            return result;
        }

        public abstract Task Execute(object triggerContext, CancellationToken cancellationToken);
    }
}
 