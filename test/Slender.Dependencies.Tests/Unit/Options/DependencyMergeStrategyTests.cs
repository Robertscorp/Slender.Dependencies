using FluentAssertions;
using Moq;
using Slender.Dependencies.Options;
using Slender.Dependencies.Tests.Support;
using System;
using Xunit;

namespace Slender.Dependencies.Tests.Unit.Options
{

    public class DependencyMergeStrategyTests
    {

        #region - - - - - - Fields - - - - - -

        private readonly TestDependency m_ExistingDependency = new(typeof(IService)) { Implementations = new() { typeof(ServiceImplementation) } };
        private readonly TestDependency m_IncomingDependency = new(typeof(IService)) { Implementations = new() { typeof(ServiceImplementation2) } };

        #endregion Fields

        #region - - - - - - Custom Tests - - - - - -

        [Fact]
        public void Custom_AnyMergeFunction_AssignsSpecifiedMergeFunction()
        {
            // Arrange
            var _MergeFunction = new Mock<Func<IDependency, IDependency, IDependency>>().Object;

            // Act
            var _Strategy = DependencyMergeStrategy.Custom(_MergeFunction);

            // Assert
            _ = _Strategy.MergeFunction.Should().Be(_MergeFunction);
        }

        #endregion Custom Tests

        #region - - - - - - IgnoreIncoming Tests - - - - - -

        [Fact]
        public void IgnoreIncoming_MergingExistingAndIncomingDependency_ReturnsExistingDependencyUnchanged()
            => DependencyMergeStrategy
                .IgnoreIncoming()
                .MergeFunction
                .Invoke(this.m_ExistingDependency, this.m_IncomingDependency)
                .Should()
                .Be(this.m_ExistingDependency)
                .And
                .BeEquivalentTo(new TestDependency(typeof(IService))
                {
                    Implementations = new() { typeof(ServiceImplementation) }
                });

        #endregion IgnoreIncoming Tests

        #region - - - - - - MergeExistingIntoIncoming Tests - - - - - -

        [Fact]
        public void MergeExistingIntoIncoming_MergingExistingAndIncomingDependency_ReturnsIncomingDependencyWithExistingImplementationAdded()
            => DependencyMergeStrategy
                .MergeExistingIntoIncoming()
                .MergeFunction
                .Invoke(this.m_ExistingDependency, this.m_IncomingDependency)
                .Should()
                .Be(this.m_IncomingDependency)
                .And
                .BeEquivalentTo(new TestDependency(typeof(IService))
                {
                    Implementations = new() { typeof(ServiceImplementation2), typeof(ServiceImplementation) }
                }, opts => opts.WithStrictOrdering());

        #endregion MergeExistingIntoIncoming Tests

        #region - - - - - - MergeIncomingIntoExisting Tests - - - - - -

        [Fact]
        public void MergeIncomingIntoExisting_MergingExistingAndIncomingDependency_ReturnsExistingDependencyWithIncomingImplementationAdded()
            => DependencyMergeStrategy
                .MergeIncomingIntoExisting()
                .MergeFunction
                .Invoke(this.m_ExistingDependency, this.m_IncomingDependency)
                .Should()
                .Be(this.m_ExistingDependency)
                .And
                .BeEquivalentTo(new TestDependency(typeof(IService))
                {
                    Implementations = new() { typeof(ServiceImplementation), typeof(ServiceImplementation2) }
                }, opts => opts.WithStrictOrdering());

        #endregion MergeIncomingIntoExisting Tests

        #region - - - - - - ReplaceExisting Tests - - - - - -

        [Fact]
        public void ReplaceExisting_MergingExistingAndIncomingDependency_ReturnsIncomingDependencyUnchanged()
            => DependencyMergeStrategy
                .ReplaceExisting()
                .MergeFunction
                .Invoke(this.m_ExistingDependency, this.m_IncomingDependency)
                .Should()
                .Be(this.m_IncomingDependency)
                .And
                .BeEquivalentTo(new TestDependency(typeof(IService))
                {
                    Implementations = new() { typeof(ServiceImplementation2) }
                });

        #endregion ReplaceExisting Tests

    }

}
