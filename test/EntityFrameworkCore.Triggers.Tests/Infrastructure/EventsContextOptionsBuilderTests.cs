using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggers.Infrastructure;
using EntityFrameworkCore.Triggers.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace EntityFrameworkCore.Triggers.Tests.Infrastructure
{
    public class EventsContextOptionsBuilderTests
    {
        static (EventsContextOptionsBuilder subject, Func<EventsOptionExtension> extensionAccessor) CreateSubject()
        {
            var optionsBuilder = new DbContextOptionsBuilder();
            EventsContextOptionsBuilder subject = null;
            optionsBuilder.UseEvents(x => subject = x);

            return (subject, () => optionsBuilder.Options.FindExtension<EventsOptionExtension>());
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
        public void AddChangeEventHandler_Sticks()
        {
            var (subject, extensionAccessor) = CreateSubject();

            subject.AddChangeEventHandler<Stubs.ChangeEventHandlerStub<object>>();

            Assert.Single(extensionAccessor().ChangeEventHandlers);
        }

    }
}
