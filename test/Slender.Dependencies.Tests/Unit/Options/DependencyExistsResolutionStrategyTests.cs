using FluentAssertions;
using Slender.Dependencies.Options;
using Slender.Dependencies.Tests.Support;
using Xunit;

namespace Slender.Dependencies.Tests.Unit.Options
{

    public class DependencyExistsResolutionStrategiesTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly DependencyProvider m_DependencyProvider = dependencyType => new TestDependency(dependencyType);
        private readonly TestDependency m_ExistingDependency = new(typeof(IService)) { Implementations = new() { typeof(ServiceImplementation) } };

        #endregion Fields

        #region - - - - - - KeepExisting Tests - - - - - -

        [Fact]
        public void KeepExisting_MergingExistingAndIncomingDependency_ReturnsExistingDependencyUnchanged()
            => DependencyExistsResolutionStrategies
                .KeepExisting
                .Invoke(this.m_DependencyProvider, this.m_ExistingDependency, this.m_ExistingDependency.DependencyType)
                .Should()
                .Be(this.m_ExistingDependency)
                .And
                .BeEquivalentTo(new TestDependency(typeof(IService))
                {
                    Implementations = new() { typeof(ServiceImplementation) }
                });

        #endregion KeepExisting Tests

        #region - - - - - - ReplaceExisting Tests - - - - - -

        [Fact]
        public void ReplaceExisting_MergingExistingAndIncomingDependency_ReturnsExistingDependencyUnchanged()
            => DependencyExistsResolutionStrategies
                .ReplaceExisting
                .Invoke(this.m_DependencyProvider, this.m_ExistingDependency, this.m_ExistingDependency.DependencyType)
                .Should()
                .NotBe(this.m_ExistingDependency)
                .And
                .BeEquivalentTo(new TestDependency(typeof(IService)));

        #endregion ReplaceExisting Tests

    }

}
