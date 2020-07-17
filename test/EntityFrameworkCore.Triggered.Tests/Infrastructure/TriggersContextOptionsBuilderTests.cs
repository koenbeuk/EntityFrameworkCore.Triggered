using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public void RecursionMode_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.RecursionMode(RecursionMode.None);

            Assert.Equal(RecursionMode.None, extensionAccessor().RecursionMode);
        }

        [Fact]
        public void MaxRecursion_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.MaxRecusion(10);

            Assert.Equal(10, extensionAccessor().MaxRecursion);
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
