﻿using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace EntityFrameworkCore.Triggered.Internal.Descriptors
{
    public sealed class BeforeSaveTriggerDescriptor : ITriggerTypeDescriptor
    {
        readonly Action<object, object> _invocationDelegate;
        readonly Type _triggerType;

        public BeforeSaveTriggerDescriptor(Type entityType)
        {
            var triggerType = typeof(IBeforeSaveTrigger<>).MakeGenericType(entityType);
            var triggerMethod = triggerType.GetMethod(nameof(IBeforeSaveTrigger<object>.BeforeSave));

            _triggerType = triggerType;
            _invocationDelegate = TriggerTypeDescriptorHelpers.GetWeakDelegate(triggerType, entityType, triggerMethod!);
        }

        public Type TriggerType => _triggerType;

        public void Invoke(object trigger, object triggerContext, Exception? exception)
        {
            Debug.Assert(exception == null);

            _invocationDelegate(trigger, triggerContext);
        }
    }
}
