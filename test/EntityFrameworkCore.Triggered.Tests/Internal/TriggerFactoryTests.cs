using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerFactoryTests
    {
        class SampleTrigger : IBeforeSaveTrigger<object>
        {
            public void BeforeSave(ITriggerContext<object> context) => throw new NotImplementedException();
        }

        class SampleTrigger2 : IBeforeSaveTrigger<object>
        {
            public SampleTrigger2(TriggerFactory triggerFactory)
            {
                TriggerFactory = triggerFactory;
            }

            public TriggerFactory TriggerFactory { get; }

            public void BeforeSave(ITriggerContext<object> context) => throw new NotImplementedException();
        }

        class SampleTrigger3<TDbContext> : IBeforeSaveTrigger<object>
            where TDbContext : DbContext
        {
            public SampleTrigger3(TDbContext dbContext)
            {
                DbContext = dbContext;
            }

            public TDbContext DbContext { get; }

            public void BeforeSave(ITriggerContext<object> context) => throw new NotImplementedException();
        }

        public class SampleDbContext3 : DbContext
        {
            protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            {
                optionsBuilder
                    .UseInMemoryDatabase(nameof(SampleDbContext3))
                    .UseTriggers(triggerOptions =>
                        triggerOptions
                            .AddTrigger<SampleTrigger3<SampleDbContext3>>()
                            .AddTrigger<SampleTrigger3<DbContext>>()
                    );
                optionsBuilder.ConfigureWarnings(warningOptions => {
                    warningOptions.Ignore(CoreEventId.ManyServiceProvidersCreatedWarning);
                });
            }
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

        [Fact]
        public void Resolve_FromHybridServices_GetsPasedTheConcreteDbContext()
        {
            using var dbContext = new SampleDbContext3();
            var factory = dbContext.GetService<TriggerFactory>();
            var serviceProvider = new HybridServiceProvider(dbContext.GetInfrastructure(), dbContext);

            var trigger = factory.Resolve(serviceProvider, typeof(IBeforeSaveTrigger<object>)).FirstOrDefault() as SampleTrigger3<SampleDbContext3>;

            Assert.NotNull(trigger);
            Assert.Equal(dbContext, trigger.DbContext);
        }

        [Fact]
        public void Resolve_FromHybridServices_GetsPasedTheAbstractDbContext()
        {
            using var dbContext = new SampleDbContext3();
            var factory = dbContext.GetService<TriggerFactory>();
            var serviceProvider = new HybridServiceProvider(dbContext.GetInfrastructure(), dbContext);

            var trigger = factory.Resolve(serviceProvider, typeof(IBeforeSaveTrigger<object>)).LastOrDefault() as SampleTrigger3<DbContext>;

            Assert.NotNull(trigger);
            Assert.Equal(dbContext, trigger.DbContext);
        }
    }
}
