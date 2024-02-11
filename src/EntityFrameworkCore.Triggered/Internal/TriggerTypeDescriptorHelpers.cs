using System.Reflection;

namespace EntityFrameworkCore.Triggered.Internal;

// Credits: https://codeblog.jonskeet.uk/2008/08/09/making-reflection-fly-and-exploring-delegates/
public static class TriggerTypeDescriptorHelpers
{
    public static Action<object, object> GetWeakDelegate(Type triggerType, Type entityType, MethodInfo method)
    {
        var triggerContextType = typeof(ITriggerContext<>).MakeGenericType(entityType);

        var genericHelper = typeof(TriggerTypeDescriptorHelpers).GetMethod(nameof(TriggerTypeDescriptorHelpers.GetWeakDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic);
        var constructedHelper = genericHelper!.MakeGenericMethod(triggerType, triggerContextType);

        return (Action<object, object>)constructedHelper.Invoke(null, [method])!;
    }

    public static Action<object, object, Exception?> GetWeakDelegateWithException(Type triggerType, Type entityType, MethodInfo method)
    {
        var triggerContextType = typeof(ITriggerContext<>).MakeGenericType(entityType);

        var genericHelper = typeof(TriggerTypeDescriptorHelpers).GetMethod(nameof(TriggerTypeDescriptorHelpers.GetWeakDelegateHelperWithException), BindingFlags.Static | BindingFlags.NonPublic);
        var constructedHelper = genericHelper!.MakeGenericMethod(triggerType, triggerContextType);

        return (Action<object, object, Exception?>)constructedHelper.Invoke(null, [method])!;
    }

    static Action<object, object> GetWeakDelegateHelper<TTriggerType, TTriggerContext>(MethodInfo method)
        where TTriggerType : class
    {
        var invocationDelegate = (Action<TTriggerType, TTriggerContext>)Delegate.CreateDelegate(typeof(Action<TTriggerType, TTriggerContext>), method);

        Action<object, object> result = (object trigger, object triggerContext) => invocationDelegate((TTriggerType)trigger, (TTriggerContext)triggerContext);
        return result;
    }

    static Action<object, object, Exception> GetWeakDelegateHelperWithException<TTriggerType, TTriggerContext>(MethodInfo method)
        where TTriggerType : class
    {
        var invocationDelegate = (Action<TTriggerType, TTriggerContext, Exception?>)Delegate.CreateDelegate(typeof(Action<TTriggerType, TTriggerContext, Exception>), method);

        Action<object, object, Exception?> result = (object trigger, object triggerContext, Exception? exception) => invocationDelegate((TTriggerType)trigger, (TTriggerContext)triggerContext, exception);
        return result;
    }

    public static Func<object, object, CancellationToken, Task> GetAsyncWeakDelegate(Type triggerType, Type entityType, MethodInfo method)
    {
        var triggerContextType = typeof(ITriggerContext<>).MakeGenericType(entityType);

        var genericHelper = typeof(TriggerTypeDescriptorHelpers).GetMethod(nameof(TriggerTypeDescriptorHelpers.GetAsyncWeakDelegateHelper), BindingFlags.Static | BindingFlags.NonPublic);
        var constructedHelper = genericHelper!.MakeGenericMethod(triggerType, triggerContextType);

        return (Func<object, object, CancellationToken, Task>)constructedHelper.Invoke(null, [method])!;
    }

    public static Func<object, object, Exception?, CancellationToken, Task> GetAsyncWeakDelegateWithException(Type triggerType, Type entityType, MethodInfo method)
    {
        var triggerContextType = typeof(ITriggerContext<>).MakeGenericType(entityType);

        var genericHelper = typeof(TriggerTypeDescriptorHelpers).GetMethod(nameof(TriggerTypeDescriptorHelpers.GetAsyncWeakDelegateHelperWithException), BindingFlags.Static | BindingFlags.NonPublic);
        var constructedHelper = genericHelper!.MakeGenericMethod(triggerType, triggerContextType);

        return (Func<object, object, Exception?, CancellationToken, Task>)constructedHelper.Invoke(null, [method])!;
    }

    static Func<object, object, CancellationToken, Task> GetAsyncWeakDelegateHelper<TTriggerType, TTriggerContext>(MethodInfo method)
        where TTriggerType : class
    {
        var invocationDelegate = (Func<TTriggerType, TTriggerContext, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<TTriggerType, TTriggerContext, CancellationToken, Task>), method);

        Func<object, object, CancellationToken, Task> result = (object trigger, object triggerContext, CancellationToken cancellationToken) => invocationDelegate((TTriggerType)trigger, (TTriggerContext)triggerContext, cancellationToken);
        return result;
    }

    static Func<object, object, Exception?, CancellationToken, Task> GetAsyncWeakDelegateHelperWithException<TTriggerType, TTriggerContext>(MethodInfo method)
        where TTriggerType : class
    {
        var invocationDelegate = (Func<TTriggerType, TTriggerContext, Exception?, CancellationToken, Task>)Delegate.CreateDelegate(typeof(Func<TTriggerType, TTriggerContext, Exception?, CancellationToken, Task>), method);

        Func<object, object, Exception?, CancellationToken, Task> result = (object trigger, object triggerContext, Exception? exception, CancellationToken cancellationToken) => invocationDelegate((TTriggerType)trigger, (TTriggerContext)triggerContext, exception, cancellationToken);
        return result;
    }
}
