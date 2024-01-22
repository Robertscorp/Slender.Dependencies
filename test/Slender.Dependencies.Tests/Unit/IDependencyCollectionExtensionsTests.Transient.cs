using FluentAssertions;
using Slender.Dependencies.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public partial class IDependencyCollectionExtensionsTests
    {

        #region - - - - - - AddTransient<TImplementation> Tests - - - - - -

        [Fact]
        public void AddTransient_TImplementation_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddTransient<ServiceImplementation>(default!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddTransient_TImplementation_AddingImplementationAsDependency_RegistersDependencyWithImplementation()
            => this.m_DependencyCollection
                .AddTransient<ServiceImplementation>()
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(ServiceImplementation))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Transient()
                        }
                    }
                });

        #endregion AddTransient<TImplementation> Tests

        #region - - - - - - AddTransient<TDependency, TImplementation> Tests - - - - - -

        [Fact]
        public void AddTransient_TDependencyTImplementation_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddTransient<IService, ServiceImplementation>(default!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddTransient_TDependencyTImplementation_AddingDependencyAndImplementation_RegistersDependencyWithImplementation()
            => this.m_DependencyCollection
                .AddTransient<IService, ServiceImplementation>()
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Transient()
                        }
                    }
                });

        #endregion AddTransient<TDependency, TImplementation> Tests

        #region - - - - - - AddTransient(ImplementationFactory) Tests - - - - - -

        [Fact]
        public void AddTransient_ImplementationFactory_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddTransient(default!, this.m_ImplementationFactory))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddTransient_ImplementationFactory_NotSpecifyingFactory_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddTransient<IDependency>(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddTransient_ImplementationFactory_AddingDependencyWithImplementationFactory_RegistersDependencyWithImplementationFactory()
            => this.m_DependencyCollection
                .AddTransient(this.m_ImplementationFactory)
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { this.m_ImplementationFactory },
                            Lifetime = DependencyLifetime.Transient()
                        }
                    }
                });

        #endregion AddTransient(ImplementationFactory) Tests

        #region - - - - - - AddTransient(DependencyType, ConfigurationAction)  Tests - - - - - -

        [Fact]
        public void AddTransient_DependencyTypeConfigurationAction_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddTransient(default!, typeof(IService), d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddTransient_DependencyTypeConfigurationAction_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddTransient(null, d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddTransient_DependencyTypeConfigurationAction_AddingDependencyWithNoConfigurationAction_RegistersDependency()
            => this.m_DependencyCollection
                .AddTransient(typeof(IService))
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Lifetime = DependencyLifetime.Transient()
                        }
                    }
                });

        [Fact]
        public void AddTransient_DependencyTypeConfigurationAction_AddingDependencyWithConfigurationAction_RegistersDependencyAndInvokesConfigurationAction()
            => this.m_DependencyCollection
                .AddTransient(typeof(IService), d => d.HasImplementationType(typeof(ServiceImplementation)))
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Transient()
                        }
                    }
                });

        #endregion AddTransient(DependencyType, ConfigurationAction) Tests

    }

}
