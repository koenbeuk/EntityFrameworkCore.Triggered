using System;
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
        public void CascadeMode_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.CascadeMode(CascadeMode.None);

            Assert.Equal(CascadeMode.None, extensionAccessor().CascadeMode);
        }

        [Fact]
        public void MaxCascade_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.MaxCascade(10);

            Assert.Equal(10, extensionAccessor().MaxCascade);
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
