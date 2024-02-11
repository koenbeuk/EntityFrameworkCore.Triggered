using EntityFrameworkCore.Triggered.Infrastructure;
using EntityFrameworkCore.Triggered.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Infrastructure
{
    public class TriggersContextOptionsBuilderTests
    {
        static (TriggersContextOptionsBuilder subject, Func<TriggersOptionExtension> extensionAccessor) CreateSubject()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            TriggersContextOptionsBuilder subject = null;
            optionsBuilder.UseTriggers(x => subject = x);

            return (subject, () => optionsBuilder.Options.FindExtension<TriggersOptionExtension>());
        }

        [Fact]
        public void CascadingMode_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.CascadeBehavior(CascadeBehavior.None);

            Assert.Equal(CascadeBehavior.None, extensionAccessor().CascadeBehavior);
        }

        [Fact]
        public void MaxCascadingCycles_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.MaxCascadeCycles(10);

            Assert.Equal(10, extensionAccessor().MaxCascadeCycles);
        }

        [Fact]
        public void AddTigger_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.AddTrigger<Stubs.TriggerStub<object>>();

            Assert.Single(extensionAccessor().Triggers);
        }
    }
}
