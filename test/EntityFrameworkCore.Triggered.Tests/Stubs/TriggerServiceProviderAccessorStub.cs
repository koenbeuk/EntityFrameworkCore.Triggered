﻿using System;

namespace EntityFrameworkCore.Triggered.Tests.Stubs
{
    public class TriggerServiceProviderAccessorStub : ITriggerServiceProviderAccessor
    {
        readonly IServiceProvider _serviceProvider;

        public TriggerServiceProviderAccessorStub(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IServiceProvider GetTriggerServiceProvider() => _serviceProvider;
    }
}
