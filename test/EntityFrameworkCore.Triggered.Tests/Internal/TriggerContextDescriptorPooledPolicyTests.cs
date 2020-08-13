using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Internal;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerContextDescriptorPooledPolicyTests
    {
        [Fact]
        public void Create_ReturnsNewInstance()
        {
            var subject = new TriggerContextDescriptorPooledPolicy();
            var instance1 = subject.Create();
            var instance2 = subject.Create();

            Assert.NotNull(instance1);
            Assert.NotNull(instance2);

            Assert.NotEqual(instance1, instance2);
        }

        [Fact]
        public void Return_ResetsInstance()
        {
            var subject = new TriggerContextDescriptorPooledPolicy();
            var instance = new TriggerContextDescriptor();
            instance.Initialize(null, ChangeType.Modified);

            subject.Return(instance);

            Assert.Equal(default, instance.ChangeType);
        }
    }
}
