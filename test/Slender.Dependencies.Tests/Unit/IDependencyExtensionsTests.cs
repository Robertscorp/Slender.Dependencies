using FluentAssertions;
using Slender.Dependencies.Tests.Support;
using System;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public class IDependencyExtensionsTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly TestDependency m_Dependency = new(typeof(IService));
        private readonly Func<DependencyFactory, object> m_ImplementationFactory = factory => new();
        private readonly object m_ImplementationInstance = new ServiceImplementation();
        private readonly DependencyLifetime m_Lifetime = TestDependencyLifetime.Instance(true);

        #endregion Fields

        #region - - - - - - HasImplementationFactory Tests - - - - - -

        [Fact]
        public void HasImplementationFactory_NullDependency_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyExtensions.HasImplementationFactory<IDependency>(null!, this.m_ImplementationFactory))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasImplementationFactory_NullFactory_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.HasImplementationFactory(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasImplementationFactory_DependencyAndFactorySpecified_AddsFactoryAndReturnsDependency()
            => this.m_Dependency
                .HasImplementationFactory(this.m_ImplementationFactory)
                .Should()
                .BeEquivalentTo(new TestDependency(typeof(IService)) { Implementations = new() { this.m_ImplementationFactory } });

        #endregion HasImplementationFactory Tests

        #region - - - - - - HasImplementationInstance Tests - - - - - -

        [Fact]
        public void HasImplementationInstance_TDependency_NullDependency_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyExtensions.HasImplementationInstance<IDependency>(null!, this.m_ImplementationInstance))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasImplementationInstance_TDependency_NullInstance_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.HasImplementationInstance(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasImplementationInstance_TDependency_InstanceIsNotDerivedFromDependencyType_ThrowsArgumentException()
            => Record
                .Exception(() => this.m_Dependency.HasImplementationInstance(new object()))
                .Should()
                .BeOfType<ArgumentException>();

        [Fact]
        public void HasImplementationInstance_TDependency_DependencyAndValidInstanceSpecified_AddsInstanceAndReturnsDependency()
            => this.m_Dependency
                .HasImplementationInstance(this.m_ImplementationInstance)
                .Should()
                .BeEquivalentTo(new TestDependency(typeof(IService)) { Implementations = new() { this.m_ImplementationInstance } });

        #endregion HasImplementationInstance Tests

        #region - - - - - - HasImplementationType<TDependency> Tests - - - - - -

        [Fact]
        public void HasImplementationType_TDependency_NullDependency_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyExtensions.HasImplementationType<IDependency>(null!, typeof(ServiceImplementation)))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasImplementationType_TDependency_NullType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.HasImplementationType(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasImplementationType_TDependency_TypeIsNotDerivedFromDependencyType_ThrowsArgumentException()
            => Record
                .Exception(() => this.m_Dependency.HasImplementationType(typeof(string)))
                .Should()
                .BeOfType<ArgumentException>();

        [Fact]
        public void HasImplementationType_TDependency_DependencyAndValidTypeSpecified_AddsTypeAndReturnsDependency()
            => this.m_Dependency
                .HasImplementationType(typeof(ServiceImplementation))
                .Should()
                .BeEquivalentTo(new TestDependency(typeof(IService)) { Implementations = new() { typeof(ServiceImplementation) } });

        #endregion HasImplementationType<TDependency> Tests

        #region - - - - - - HasImplementationType<TImplementation> Tests - - - - - -

        [Fact]
        public void HasImplementationType_TImplementation_NullDependency_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyExtensions.HasImplementationType<IService>(null!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasImplementationType_TImplementation_TypeIsNotDerivedFromDependencyType_ThrowsArgumentException()
            => Record
                .Exception(() => this.m_Dependency.HasImplementationType<object>())
                .Should()
                .BeOfType<ArgumentException>();

        [Fact]
        public void HasImplementationType_TImplementation_DependencyAndValidTypeSpecified_AddsTypeAndReturnsDependency()
            => this.m_Dependency
                .HasImplementationType<ServiceImplementation>()
                .Should()
                .BeEquivalentTo(new TestDependency(typeof(IService)) { Implementations = new() { typeof(ServiceImplementation) } });

        #endregion HasImplementationType<TImplementation> Tests

        #region - - - - - - HasLifetime Tests - - - - - -

        [Fact]
        public void HasLifetime_NullDependency_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyExtensions.HasLifetime<IDependency>(null!, this.m_Lifetime))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasLifetime_NullLifetime_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_Dependency.HasLifetime(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void HasLifetime_DependencyAndLifetimeSpecified_SetsLifetimeAndReturnsDependency()
            => this.m_Dependency
                .HasLifetime(this.m_Lifetime)
                .Should()
                .BeEquivalentTo(new TestDependency(typeof(IService)) { Lifetime = this.m_Lifetime });

        #endregion HasLifetime Tests

    }

}
