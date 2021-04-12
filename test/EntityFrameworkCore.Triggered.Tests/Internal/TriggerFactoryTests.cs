using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using Xunit.Sdk;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerFactoryTests
    {
        class SampleTrigger : IBeforeSaveTrigger<object>
        {
            public Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        class SampleTrigger2 : IBeforeSaveTrigger<object>
        {
            public SampleTrigger2(TriggerFactory triggerFactory)
            {
                TriggerFactory = triggerFactory;
            }

            public TriggerFactory TriggerFactory { get; }

            public Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken) => throw new NotImplementedException();
        }

        [Fact]
        public void Resolve_FromExternalServiceProvider_FindsType()
        {
            var serviceProvider = new ServiceCollection().BuildServiceProvider();

            var applicationServiceProvider = new ServiceCollection()
                .AddTransient<IBeforeSaveTrigger<object>, SampleTrigger>()
                .BuildServiceProvider();

            var subject = new TriggerFactory(serviceProvider);

            var triggers = subject.Resolve(applicationServiceProvider, typeof(IBeforeSaveTrigger<object>));

            Assert.Single(triggers);
        }

        [Fact]
        public void Resolve_FromInternalServices_FindsType()
        {
            var serviceProvider = new ServiceCollection()
                .AddTransient(typeof(ITriggerInstanceFactory<IBeforeSaveTrigger<object>>), _ => new TriggerInstanceFactory<SampleTrigger>(null))
                .BuildServiceProvider();

            var applicationServiceProvider = new ServiceCollection().BuildServiceProvider();

            var subject = new TriggerFactory(serviceProvider);

            var triggers = subject.Resolve(applicationServiceProvider, typeof(IBeforeSaveTrigger<object>));

            Assert.Single(triggers);
        }

        [Fact]
        public void Resolve_FromInternalServices_GetsConstructedUsingExternalServiceProvider()
        {
            var serviceProvider = new ServiceCollection()
                .AddLogging()
                .AddTransient(typeof(ITriggerInstanceFactory<IBeforeSaveTrigger<object>>), _ => new TriggerInstanceFactory<SampleTrigger2>(null))
                .BuildServiceProvider();

            var subject = new TriggerFactory(serviceProvider);
            
            var applicationServiceProvider = new ServiceCollection()
                .AddSingleton(subject)
                .BuildServiceProvider();

            var trigger = subject.Resolve(applicationServiceProvider, typeof(IBeforeSaveTrigger<object>)).FirstOrDefault() as SampleTrigger2;

            Assert.NotNull(trigger);
            Assert.Equal(subject, trigger.TriggerFactory);
        }
    }
}
