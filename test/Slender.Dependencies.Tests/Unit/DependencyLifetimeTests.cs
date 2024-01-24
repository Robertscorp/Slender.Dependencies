using FluentAssertions;
using System.Collections.Generic;
using Xunit;

namespace Slender.Dependencies.Tests.Unit
{

    public class DependencyLifetimeTests
    {

        #region - - - - - - Fields - - - - - -

        private static readonly object s_ImplementationFactory = (DependencyFactory f) => new object();
        private static readonly object s_ImplementationInstance = new();
        private static readonly object s_ImplementationType = typeof(object);

        #endregion Fields

        #region - - - - - - Scoped Tests - - - - - -

        [Theory]
        [MemberData(nameof(SupportsImplementation_ScopedLifetimeWithVariousImplementations_ReturnsExpectedResult_GetTestData))]
        public void SupportsImplementation_ScopedLifetimeWithVariousImplementations_ReturnsExpectedResult(object implementation, bool expected) =>
            DependencyLifetime
                .Scoped()
                .SupportsImplementation(implementation)
                .Should()
                .Be(expected);

        public static IEnumerable<object[]> SupportsImplementation_ScopedLifetimeWithVariousImplementations_ReturnsExpectedResult_GetTestData()
            => new List<object[]>
            {
                new object[] { default!, false },
                new object[] { s_ImplementationFactory, true },
                new object[] { s_ImplementationInstance, false },
                new object[] { s_ImplementationType, true },
            };

        #endregion Scoped Tests

        #region - - - - - - Singleton Tests - - - - - -

        [Theory]
        [MemberData(nameof(SupportsImplementation_SingletonLifetimeWithVariousImplementations_ReturnsExpectedResult_GetTestData))]
        public void SupportsImplementation_SingletonLifetimeWithVariousImplementations_ReturnsExpectedResult(object implementation, bool expected) =>
            DependencyLifetime
                .Singleton()
                .SupportsImplementation(implementation)
                .Should()
                .Be(expected);

        public static IEnumerable<object[]> SupportsImplementation_SingletonLifetimeWithVariousImplementations_ReturnsExpectedResult_GetTestData()
            => new List<object[]>
            {
                new object[] { default!, false },
                new object[] { s_ImplementationFactory, true },
                new object[] { s_ImplementationInstance, true },
                new object[] { s_ImplementationType, true },
            };

        #endregion Singleton Tests

        #region - - - - - - Transient Tests - - - - - -

        [Theory]
        [MemberData(nameof(SupportsImplementation_TransientLifetimeWithVariousImplementations_ReturnsExpectedResult_GetTestData))]
        public void SupportsImplementation_TransientLifetimeWithVariousImplementations_ReturnsExpectedResult(object implementation, bool expected) =>
            DependencyLifetime
                .Transient()
                .SupportsImplementation(implementation)
                .Should()
                .Be(expected);

        public static IEnumerable<object[]> SupportsImplementation_TransientLifetimeWithVariousImplementations_ReturnsExpectedResult_GetTestData()
            => new List<object[]>
            {
                new object[] { default!, false },
                new object[] { s_ImplementationFactory, true },
                new object[] { s_ImplementationInstance, false },
                new object[] { s_ImplementationType, true },
            };

        #endregion Transient Tests

    }

}
