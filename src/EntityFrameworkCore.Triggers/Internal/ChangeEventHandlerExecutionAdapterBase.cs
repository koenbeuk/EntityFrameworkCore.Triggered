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
    public abstract class ChangeEventHandlerExecutionAdapterBase
    {
        public ChangeEventHandlerExecutionAdapterBase(object changeHandler)
        {
            if (changeHandler == null)
            {
                throw new ArgumentNullException(nameof(changeHandler));
            }

            Debug.Assert(TypeHelpers.FindGenericInterface(changeHandler.GetType(), typeof(IBeforeSaveChangeEventHandler<>)) != null);

            ChangeHandler = changeHandler;
        }

        protected object ChangeHandler { get; }

        protected Task Execute(string methodName, object @event, CancellationToken cancellationToken)
        {
            var methodInfo = ChangeHandler
                .GetType()
                .GetMethod(methodName);

            if (methodInfo == null)
            {
                throw new InvalidOperationException("No such method found");
            }

            var result = (Task)methodInfo.Invoke(ChangeHandler, new object[] { @event, cancellationToken });

            return result;
        }

        public abstract Task Execute(object @event, CancellationToken cancellationToken);
    }
}
 