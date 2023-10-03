using FluentAssertions;
using Moq;
using Slender.ServiceRegistrations.Tests.Support;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public class DefaultDependencyBehaviourTests
    {

        #region - - - - - - Fields - - - - - -

        private static readonly Func<DependencyFactory, object> s_ImplementationFactory = factory => new object();
        private static readonly object s_ImplementationInstance = new();

        private readonly IDependencyBuilderBehaviour m_DependencyBehaviour = new Mock<IDependencyBuilderBehaviour>().Object;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        private static IDependencyBuilderBehaviour BuilderBehaviour_Default
            => DefaultDependencyBehaviour.Instance();

        private static IDependencyBuilderBehaviour BuilderBehaviour_DisableChange
            => DefaultDependencyBehaviour.Instance(allowBehaviourToChange: false);

        private static IDependencyBuilderBehaviour BuilderBehaviour_MultipleTypes
            => DefaultDependencyBehaviour.Instance(allowMultipleImplementationTypes: true);

        private static Dependency Dependency_Empty
            => new(typeof(object));

        private static Dependency DependencyWithFactory
            => new(typeof(object)) { ImplementationFactory = s_ImplementationFactory };

        private static Dependency DependencyWithInstance
            => new(typeof(object)) { ImplementationInstance = s_ImplementationInstance };

        private static Dependency DependencyWithMultipleTypes
            => new(typeof(object)) { ImplementationTypes = new List<Type>() { typeof(object), typeof(object) } };

        private static Dependency DependencyWithSingleType
            => new(typeof(object)) { ImplementationTypes = new List<Type>() { typeof(object) } };

        #endregion Properties

        #region - - - - - - AddImplementationType Tests - - - - - -

        [Theory]
        [MemberData(nameof(AddImplementationType_VariousScenarios_ResultsInExpectedDependency_GetTestData))]
        public void AddImplementationType_VariousScenarios_ResultsInExpectedDependency(Dependency startingDependency, Dependency expectedDependency, IDependencyBuilderBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.AddImplementationType(startingDependency, typeof(object));

            // Assert
            _ = startingDependency.Should().BeEquivalentTo(expectedDependency);
        }

        public static IEnumerable<object[]> AddImplementationType_VariousScenarios_ResultsInExpectedDependency_GetTestData()
            => new[]
            {
                new object[] { Dependency_Empty, DependencyWithSingleType, BuilderBehaviour_Default },
                new object[] { DependencyWithSingleType, DependencyWithSingleType, BuilderBehaviour_Default },
                new object[] { DependencyWithInstance, DependencyWithInstance, BuilderBehaviour_Default },
                new object[] { DependencyWithFactory, DependencyWithFactory, BuilderBehaviour_Default },
                new object[] { Dependency_Empty, DependencyWithSingleType, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithSingleType, DependencyWithMultipleTypes, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithInstance, DependencyWithInstance, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithFactory, DependencyWithFactory, BuilderBehaviour_MultipleTypes },
            };

        #endregion AddImplementationType Tests

        #region - - - - - - AllowScannedImplementationTypes Tests - - - - - -

        [Fact]
        public void AllowScannedImplementationTypes_AnyRequest_DependencyAllowsScannedImplementationTypes()
        {
            // Arrange
            var _Actual = new Dependency(typeof(object));
            var _Expected = new Dependency(typeof(object)) { AllowScannedImplementationTypes = true };

            // Act
            BuilderBehaviour_Default.AllowScannedImplementationTypes(_Actual);

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_Expected);
        }

        #endregion AllowScannedImplementationTypes Tests

        #region - - - - - - MergeDependency Tests - - - - - -

        [Fact]
        public void MergeDependency_AnyDependency_TakesOnIncomingDependencyAndAttemptsToOverrideWithExisting()
        {
            // Arrange
            var _MockBuilderBehaviour = new Mock<IDependencyBuilderBehaviour>();

            var _Builder = new DependencyBuilder(typeof(object));
            _Builder.Dependency.AllowScannedImplementationTypes = true;
            _Builder.Dependency.Behaviour = BuilderBehaviour_MultipleTypes;
            _Builder.Dependency.ImplementationFactory = factory => string.Empty;
            _Builder.Dependency.ImplementationInstance = string.Empty;
            _Builder.Dependency.ImplementationTypes = new List<Type> { typeof(object), typeof(string) };
            _Builder.Dependency.Lifetime = TestDependencyLifetime.Instance(true);

            var _InitialDependency = _Builder.Dependency;

            var _Dependency = new Dependency(typeof(object))
            {
                Behaviour = _MockBuilderBehaviour.Object,
                ImplementationTypes = new List<Type> { typeof(object), typeof(int) },
                Lifetime = TestDependencyLifetime.Instance(true)
            };

            // Act
            _Builder.Dependency.Behaviour.MergeDependencies(_Builder, _Dependency);

            // Assert
            _ = _Builder.Dependency.Should().BeEquivalentTo(_Dependency);

            _MockBuilderBehaviour.Verify(mock => mock.AddImplementationType(_Builder.Dependency, typeof(string)));
            _MockBuilderBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(_Builder.Dependency));
            _MockBuilderBehaviour.Verify(mock => mock.UpdateBehaviour(_Builder.Dependency, _InitialDependency.Behaviour));
            _MockBuilderBehaviour.Verify(mock => mock.UpdateImplementationFactory(_Builder.Dependency, _InitialDependency.ImplementationFactory));
            _MockBuilderBehaviour.Verify(mock => mock.UpdateImplementationInstance(_Builder.Dependency, _InitialDependency.ImplementationInstance));
            _MockBuilderBehaviour.Verify(mock => mock.UpdateLifetime(_Builder.Dependency, _InitialDependency.Lifetime));
            _MockBuilderBehaviour.VerifyNoOtherCalls();
        }

        #endregion MergeDependency Tests

        #region - - - - - - UpdateBehaviour Tests - - - - - -

        [Fact]
        public void UpdateBehaviour_CanUpdateBehaviour_BehaviourChanged()
        {
            // Arrange
            var _Dependency = Dependency_Empty;

            // Act
            _Dependency.Behaviour.UpdateBehaviour(_Dependency, this.m_DependencyBehaviour);

            // Assert
            _ = _Dependency.Behaviour.Should().Be(this.m_DependencyBehaviour);
        }

        [Fact]
        public void UpdateBehaviour_CannotUpdateBehaviour_BehaviourDoesNotChange()
        {
            // Arrange
            var _Dependency = new Dependency(typeof(object)) { Behaviour = BuilderBehaviour_DisableChange };
            var _Behaviour = _Dependency.Behaviour;

            // Act
            _Dependency.Behaviour.UpdateBehaviour(_Dependency, this.m_DependencyBehaviour);

            // Assert
            _ = _Dependency.Behaviour.Should().Be(_Behaviour);
        }

        #endregion UpdateBehaviour Tests

        #region - - - - - - UpdateImplementationFactory Tests - - - - - -

        [Theory]
        [MemberData(nameof(UpdateImplementationFactory_VariousScenarios_ResultsInExpectedDependency_GetTestData))]
        public void UpdateImplementationFactory_VariousScenarios_ResultsInExpectedDependency(Dependency startingDependency, Dependency expectedDependency, IDependencyBuilderBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.UpdateImplementationFactory(startingDependency, s_ImplementationFactory);

            // Assert
            _ = startingDependency.Should().BeEquivalentTo(expectedDependency);
        }

        public static IEnumerable<object[]> UpdateImplementationFactory_VariousScenarios_ResultsInExpectedDependency_GetTestData()
            => new[]
            {
                new object[] { Dependency_Empty, DependencyWithFactory, BuilderBehaviour_Default },
                new object[] { DependencyWithSingleType, DependencyWithSingleType, BuilderBehaviour_Default },
                new object[] { DependencyWithInstance, DependencyWithInstance, BuilderBehaviour_Default },
                new object[] { DependencyWithFactory, DependencyWithFactory, BuilderBehaviour_Default },
                new object[] { Dependency_Empty, DependencyWithFactory, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithSingleType, DependencyWithSingleType, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithInstance, DependencyWithInstance, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithFactory, DependencyWithFactory, BuilderBehaviour_MultipleTypes },
            };

        #endregion UpdateImplementationFactory Tests

        #region - - - - - - UpdateImplementationInstance Tests - - - - - -

        [Theory]
        [MemberData(nameof(UpdateImplementationInstance_VariousScenarios_ResultsInExpectedDependency_GetTestData))]
        public void UpdateImplementationInstance_VariousScenarios_ResultsInExpectedDependency(Dependency startingDependency, Dependency expectedDependency, IDependencyBuilderBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.UpdateImplementationInstance(startingDependency, s_ImplementationInstance);

            // Assert
            _ = startingDependency.Should().BeEquivalentTo(expectedDependency);
        }

        public static IEnumerable<object[]> UpdateImplementationInstance_VariousScenarios_ResultsInExpectedDependency_GetTestData()
            => new[]
            {
                new object[] { Dependency_Empty, DependencyWithInstance, BuilderBehaviour_Default },
                new object[] { DependencyWithSingleType, DependencyWithSingleType, BuilderBehaviour_Default },
                new object[] { DependencyWithInstance, DependencyWithInstance, BuilderBehaviour_Default },
                new object[] { DependencyWithFactory, DependencyWithFactory, BuilderBehaviour_Default },
                new object[] { Dependency_Empty, DependencyWithInstance, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithSingleType, DependencyWithSingleType, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithInstance, DependencyWithInstance, BuilderBehaviour_MultipleTypes },
                new object[] { DependencyWithFactory, DependencyWithFactory, BuilderBehaviour_MultipleTypes },
            };

        #endregion UpdateImplementationInstance Tests

        #region - - - - - - UpdateLifetime Tests - - - - - -

        [Fact]
        public void UpdateLifetime_TryChangeLifetime_LifetimeCannotBeChanged()
        {
            // Arrange
            var _Dependency = Dependency_Empty;

            // Act
            _Dependency.Behaviour.UpdateLifetime(_Dependency, TestDependencyLifetime.Instance(true));

            // Assert
            _ = _Dependency.Should().BeEquivalentTo(Dependency_Empty);
        }

        #endregion UpdateLifetime Tests

    }

}
