using FluentAssertions;
using Slender.Dependencies.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public partial class IDependencyCollectionExtensionsTests
    {

        #region - - - - - - AddSingleton<TImplementation> Tests - - - - - -

        [Fact]
        public void AddSingleton_TImplementation_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddSingleton<ServiceImplementation>(default!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_TImplementation_AddingImplementationAsDependency_RegistersDependencyWithImplementation()
            => this.m_DependencyCollection
                .AddSingleton<ServiceImplementation>()
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(ServiceImplementation))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Singleton()
                        }
                    }
                });

        #endregion AddSingleton<TImplementation> Tests

        #region - - - - - - AddSingleton<TImplementation>(ImplementationInstance) Tests - - - - - -

        [Fact]
        public void AddSingleton_TImplementationImplementationInstance_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddSingleton(default!, this.m_ImplementationInstance))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_TImplementationImplementationInstance_NotSpecifyingImplementationInstance_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddSingleton<ServiceImplementation>(implementationInstance: default!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_TImplementationImplementationInstance_AddingImplementationInstanceAsDependency_RegistersDependencyWithImplementationInstance()
            => this.m_DependencyCollection
                .AddSingleton(this.m_ImplementationInstance)
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(ServiceImplementation))
                        {
                            Implementations = new List<object> { this.m_ImplementationInstance },
                            Lifetime = DependencyLifetime.Singleton()
                        }
                    }
                });

        #endregion AddSingleton<TImplementation> Tests

        #region - - - - - - AddSingleton<TDependency, TImplementation> Tests - - - - - -

        [Fact]
        public void AddSingleton_TDependencyTImplementation_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddSingleton<IService, ServiceImplementation>(default!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_TDependencyTImplementation_AddingDependencyAndImplementation_RegistersDependencyWithImplementation()
            => this.m_DependencyCollection
                .AddSingleton<IService, ServiceImplementation>()
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Singleton()
                        }
                    }
                });

        #endregion AddSingleton<TDependency, TImplementation> Tests

        #region - - - - - - AddSingleton(ImplementationFactory) Tests - - - - - -

        [Fact]
        public void AddSingleton_ImplementationFactory_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddSingleton(default!, this.m_ImplementationFactory))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_ImplementationFactory_NotSpecifyingFactory_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddSingleton<IDependency>(implementationFactory: null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_ImplementationFactory_AddingDependencyWithImplementationFactory_RegistersDependencyWithImplementationFactory()
            => this.m_DependencyCollection
                .AddSingleton(this.m_ImplementationFactory)
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { this.m_ImplementationFactory },
                            Lifetime = DependencyLifetime.Singleton()
                        }
                    }
                });

        #endregion AddSingleton(ImplementationFactory) Tests

        #region - - - - - - AddSingleton<TDependency>(ImplementationInstance) Tests - - - - - -

        [Fact]
        public void AddSingleton_TDependencyImplementationInstance_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddSingleton<IService>(default!, (object)this.m_ImplementationInstance))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_TDependencyImplementationInstance_NotSpecifyingImplementationInstance_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddSingleton<IService>(default(object)!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_TDependencyImplementationInstance_AddingImplementationInstanceAsDependency_RegistersDependencyWithImplementationInstance()
            => this.m_DependencyCollection
                .AddSingleton<IService>((object)this.m_ImplementationInstance)
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { this.m_ImplementationInstance },
                            Lifetime = DependencyLifetime.Singleton()
                        }
                    }
                });

        #endregion AddSingleton<TImplementation> Tests

        #region - - - - - - AddSingleton(DependencyType, ConfigurationAction)  Tests - - - - - -

        [Fact]
        public void AddSingleton_DependencyTypeConfigurationAction_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddSingleton(default!, typeof(IService), d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_DependencyTypeConfigurationAction_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddSingleton(null, d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddSingleton_DependencyTypeConfigurationAction_AddingDependencyWithNoConfigurationAction_RegistersDependency()
            => this.m_DependencyCollection
                .AddSingleton(typeof(IService))
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Lifetime = DependencyLifetime.Singleton()
                        }
                    }
                });

        [Fact]
        public void AddSingleton_DependencyTypeConfigurationAction_AddingDependencyWithConfigurationAction_RegistersDependencyAndInvokesConfigurationAction()
            => this.m_DependencyCollection
                .AddSingleton(typeof(IService), d => d.HasImplementationType(typeof(ServiceImplementation)))
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Singleton()
                        }
                    }
                });

        #endregion AddSingleton(DependencyType, ConfigurationAction) Tests

    }

}
