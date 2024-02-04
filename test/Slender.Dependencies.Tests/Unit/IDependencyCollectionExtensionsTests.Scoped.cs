using FluentAssertions;
using Slender.Dependencies.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public partial class IDependencyCollectionExtensionsTests
    {

        #region - - - - - - AddScoped<TImplementation> Tests - - - - - -

        [Fact]
        public void AddScoped_TImplementation_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddScoped<ServiceImplementation>(default!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddScoped_TImplementation_AddingImplementationAsDependency_RegistersDependencyWithImplementation()
            => this.m_DependencyCollection
                .AddScoped<ServiceImplementation>()
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(ServiceImplementation))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Scoped()
                        }
                    }
                });

        #endregion AddScoped<TImplementation> Tests

        #region - - - - - - AddScoped<TDependency, TImplementation> Tests - - - - - -

        [Fact]
        public void AddScoped_TDependencyTImplementation_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddScoped<IService, ServiceImplementation>(default!))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddScoped_TDependencyTImplementation_AddingDependencyAndImplementation_RegistersDependencyWithImplementation()
            => this.m_DependencyCollection
                .AddScoped<IService, ServiceImplementation>()
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Scoped()
                        }
                    }
                });

        #endregion AddScoped<TDependency, TImplementation> Tests

        #region - - - - - - AddScoped(ImplementationFactory) Tests - - - - - -

        [Fact]
        public void AddScoped_ImplementationFactory_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddScoped(default!, this.m_ImplementationFactory))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddScoped_ImplementationFactory_NotSpecifyingFactory_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddScoped<IDependency>(null))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddScoped_ImplementationFactory_AddingDependencyWithImplementationFactory_RegistersDependencyWithImplementationFactory()
            => this.m_DependencyCollection
                .AddScoped(this.m_ImplementationFactory)
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { this.m_ImplementationFactory },
                            Lifetime = DependencyLifetime.Scoped()
                        }
                    }
                });

        #endregion AddScoped(ImplementationFactory) Tests

        #region - - - - - - AddScoped(DependencyType, ConfigurationAction)  Tests - - - - - -

        [Fact]
        public void AddScoped_DependencyTypeConfigurationAction_NotSpecifyingDependencies_ThrowsArgumentNullException()
            => Record
                .Exception(() => IDependencyCollectionExtensions.AddScoped(default!, typeof(IService), d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddScoped_DependencyTypeConfigurationAction_NotSpecifyingDependencyType_ThrowsArgumentNullException()
            => Record
                .Exception(() => this.m_DependencyCollection.AddScoped(null, d => { }))
                .Should()
                .BeOfType<ArgumentNullException>();

        [Fact]
        public void AddScoped_DependencyTypeConfigurationAction_AddingDependencyWithNoConfigurationAction_RegistersDependency()
            => this.m_DependencyCollection
                .AddScoped(typeof(IService))
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Lifetime = DependencyLifetime.Scoped()
                        }
                    }
                });

        [Fact]
        public void AddScoped_DependencyTypeConfigurationAction_AddingDependencyWithConfigurationAction_RegistersDependencyAndInvokesConfigurationAction()
            => this.m_DependencyCollection
                .AddScoped(typeof(IService), d => d.HasImplementationType(typeof(ServiceImplementation)))
                .Should()
                .BeEquivalentTo(new TestDependencyCollection()
                {
                    Dependencies = new List<IDependency>()
                    {
                        new TestDependency(typeof(IService))
                        {
                            Implementations = new List<object> { typeof(ServiceImplementation) },
                            Lifetime = DependencyLifetime.Scoped()
                        }
                    }
                });

        #endregion AddScoped(DependencyType, ConfigurationAction) Tests

    }

}
