using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal
{
    public static class TriggerTypeDescriptorHelpers
    {
        public static Func<object, object, CancellationToken, Task> GetWeakDelegate(Type triggerType, Type entityType, MethodInfo method)
        {
            // Credits: https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/

            var triggerContextType = typeof(ITriggerContext<>).MakeGenericType(entityType);

            var genericHelper = typeof(TriggerTypeDescriptorHelpers).GetMethod(nameof(TriggerTypeDescriptorHelpers.GetWeakDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic);
            var constructedHelper = genericHelper!.MakeGenericMethod(triggerType, triggerContextType);

            return (Func<object, object, CancellationToken, Task>)constructedHelper.Invoke(null, new object[] { method })!;
        }

        public static Func<object, object, Exception?, CancellationToken, Task> GetWeakDelegateWithException(Type triggerType, Type entityType, MethodInfo method)
        {
            var triggerContextType = typeof(ITriggerContext<>).MakeGenericType(entityType);

            var genericHelper = typeof(TriggerTypeDescriptorHelpers).GetMethod(nameof(TriggerTypeDescriptorHelpers.GetWeakDelegateHelperWithException), BindingFlags.Static | BindingFlags.NonPublic);
            var constructedHelper = genericHelper!.MakeGenericMethod(triggerType, triggerContextType);

            return (Func<object, object, Exception?, CancellationToken, Task>)constructedHelper.Invoke(null, new object[] { method })!;
        }

        static Func<object, object, CancellationToken, Task> GetWeakDelegateHelper<TTriggerType, TTriggerContext>(MethodInfo method)
            where TTriggerType : class
        {
            var invocationDelegate = (Func<TTriggerType, TTriggerContext, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<TTriggerType, TTriggerContext, CancellationToken, Task>), method);

            Func<object, object, CancellationToken, Task> result = (object trigger, object triggerContext, CancellationToken cancellationToken) => invocationDelegate((TTriggerType)trigger, (TTriggerContext)triggerContext, cancellationToken);
            return result;
        }

        static Func<object, object, Exception?, CancellationToken, Task> GetWeakDelegateHelperWithException<TTriggerType, TTriggerContext>(MethodInfo method)
            where TTriggerType : class
        {
            var invocationDelegate = (Func<TTriggerType, TTriggerContext, Exception?, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<TTriggerType, TTriggerContext, Exception?, CancellationToken, Task>), method);

            Func<object, object, Exception?, CancellationToken, Task> result = (object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken) => invocationDelegate((TTriggerType)trigger, (TTriggerContext)triggerContext, exception, cancellationToken);
            return result;
        }
    }
}
