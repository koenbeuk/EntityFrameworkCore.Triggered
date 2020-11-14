using System.Linq;
using EntityFrameworkCore.Triggered.Internal;
using Xunit;

namespace EntityFrameworkCore.Triggered.Tests.Internal
{
    public class TriggerTypeRegistryTests
    {
        public class BaseType { }
        public class DerivedType : BaseType { }
        public interface IInterfaceType { }
        public class BaseTypeWithInterface : IInterfaceType { }
        public class DerivedTypeWithInterface : BaseTypeWithInterface, IInterfaceType { }

        TriggerTypeRegistry CreateSubject<TType>()
            => new(typeof(TType), type => new BeforeSaveTriggerDescriptor(type));

        [Fact]
        public void GetTriggerTypeDescriptors_ForObject_Returns1Descriptor()
        {
            var subject = CreateSubject<object>();
            var result = subject.GetTriggerTypeDescriptors();

            Assert.Single(result);
            Assert.Equal(typeof(IBeforeSaveTrigger<object>), result.Skip(0).First().TriggerType);
        }

        [Fact]
        public void GetTriggerTypeDescriptors_ForBaseType_Returns2DescriptorsInOrder()
        {
            var subject = CreateSubject<BaseType>();
            var result = subject.GetTriggerTypeDescriptors();

            Assert.Equal(2, result.Length);
            Assert.Equal(typeof(IBeforeSaveTrigger<object>), result.Skip(0).First().TriggerType);
            Assert.Equal(typeof(IBeforeSaveTrigger<BaseType>), result.Skip(1).First().TriggerType);
        }

        [Fact]
        public void GetTriggerTypeDescriptors_ForInterfaceType_Returns1Descriptor()
        {
            var subject = CreateSubject<IInterfaceType>();
            var result = subject.GetTriggerTypeDescriptors();

            Assert.Single(result);
            Assert.Equal(typeof(IBeforeSaveTrigger<IInterfaceType>), result.Skip(0).First().TriggerType);
        }

        [Fact]
        public void GetTriggerTypeDescriptors_ForBaseTypeWithInterface_Returns3DescriptorsInOrder()
        {
            var subject = CreateSubject<BaseTypeWithInterface>();
            var result = subject.GetTriggerTypeDescriptors();

            Assert.Equal(3, result.Length);
            Assert.Equal(typeof(IBeforeSaveTrigger<object>), result.Skip(0).First().TriggerType);
            Assert.Equal(typeof(IBeforeSaveTrigger<IInterfaceType>), result.Skip(1).First().TriggerType);
            Assert.Equal(typeof(IBeforeSaveTrigger<BaseTypeWithInterface>), result.Skip(2).First().TriggerType);
        }

        [Fact]
        public void GetTriggerTypeDescriptors_ForDerivedTypeWithInterface_Returns4DescriptorsInOrder()
        {
            var subject = CreateSubject<DerivedTypeWithInterface>();
            var result = subject.GetTriggerTypeDescriptors();

            Assert.Equal(4, result.Length);
            Assert.Equal(typeof(IBeforeSaveTrigger<object>), result.Skip(0).First().TriggerType);
            Assert.Equal(typeof(IBeforeSaveTrigger<IInterfaceType>), result.Skip(1).First().TriggerType);
            Assert.Equal(typeof(IBeforeSaveTrigger<BaseTypeWithInterface>), result.Skip(2).First().TriggerType);
            Assert.Equal(typeof(IBeforeSaveTrigger<DerivedTypeWithInterface>), result.Skip(3).First().TriggerType);
        }
    }
}
