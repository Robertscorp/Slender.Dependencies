using FluentAssertions;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace Slender.ServiceRegistrations.Tests.Unit
{

    public class DefaultRegistrationBehaviourTests
    {

        #region - - - - - - Fields - - - - - -

        private static readonly Func<ServiceFactory, object> s_ImplementationFactory = serviceFactory => new object();
        private static readonly object s_ImplementationInstance = new();

        private readonly IRegistrationBehaviour m_RegistrationBehaviour = new Mock<IRegistrationBehaviour>().Object;

        #endregion Fields

        #region - - - - - - Properties - - - - - -

        private static IRegistrationBehaviour RegistrationBehaviour_Default
            => DefaultRegistrationBehaviour.Instance();

        private static IRegistrationBehaviour RegistrationBehaviour_DisableChange
            => DefaultRegistrationBehaviour.Instance(allowBehaviourToChange: false);

        private static IRegistrationBehaviour RegistrationBehaviour_MultipleTypes
            => DefaultRegistrationBehaviour.Instance(allowMultipleImplementationTypes: true);

        private static RegistrationContext RegistrationContext_Empty
            => new();

        private static RegistrationContext RegistrationContextWithFactory
            => new() { ImplementationFactory = s_ImplementationFactory };

        private static RegistrationContext RegistrationContextWithInstance
            => new() { ImplementationInstance = s_ImplementationInstance };

        private static RegistrationContext RegistrationContextWithMultipleTypes
            => new() { ImplementationTypes = new List<Type>() { typeof(object), typeof(object) } };

        private static RegistrationContext RegistrationContextWithSingleType
            => new() { ImplementationTypes = new List<Type>() { typeof(object) } };

        #endregion Properties

        #region - - - - - - AddImplementationType Tests - - - - - -

        [Theory]
        [MemberData(nameof(AddImplementationType_VariousScenarios_ResultsInExpectedRegistrationContext_GetTestData))]
        public void AddImplementationType_VariousScenarios_ResultsInExpectedRegistrationContext(RegistrationContext startingContext, RegistrationContext expectedContext, IRegistrationBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.AddImplementationType(startingContext, typeof(object));

            // Assert
            _ = startingContext.Should().BeEquivalentTo(expectedContext);
        }

        public static IEnumerable<object[]> AddImplementationType_VariousScenarios_ResultsInExpectedRegistrationContext_GetTestData()
            => new[]
            {
                new object[] { RegistrationContext_Empty, RegistrationContextWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithSingleType, RegistrationContextWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithInstance, RegistrationContextWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithFactory, RegistrationContextWithFactory, RegistrationBehaviour_Default },
                new object[] { RegistrationContext_Empty, RegistrationContextWithSingleType, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithSingleType, RegistrationContextWithMultipleTypes, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithInstance, RegistrationContextWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithFactory, RegistrationContextWithFactory, RegistrationBehaviour_MultipleTypes },
            };

        #endregion AddImplementationType Tests

        #region - - - - - - UpdateBehaviour Tests - - - - - -

        [Fact]
        public void UpdateLifetime_CanUpdateBehaviour_BehaviourChanged()
        {
            // Arrange
            var _Context = RegistrationContext_Empty;

            // Act
            _Context.Behaviour.UpdateBehaviour(_Context, this.m_RegistrationBehaviour);

            // Assert
            _ = _Context.Behaviour.Should().Be(this.m_RegistrationBehaviour);
        }

        [Fact]
        public void UpdateLifetime_CannotUpdateBehaviour_BehaviourDoesNotChange()
        {
            // Arrange
            var _Context = new RegistrationContext { Behaviour = RegistrationBehaviour_DisableChange };
            var _Behaviour = _Context.Behaviour;

            // Act
            _Context.Behaviour.UpdateBehaviour(_Context, this.m_RegistrationBehaviour);

            // Assert
            _ = _Context.Behaviour.Should().Be(_Behaviour);
        }

        #endregion UpdateBehaviour Tests

        #region - - - - - - UpdateImplementationFactory Tests - - - - - -

        [Theory]
        [MemberData(nameof(UpdateImplementationFactory_VariousScenarios_ResultsInExpectedRegistrationContext_GetTestData))]
        public void UpdateImplementationFactory_VariousScenarios_ResultsInExpectedRegistrationContext(RegistrationContext startingContext, RegistrationContext expectedContext, IRegistrationBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.UpdateImplementationFactory(startingContext, s_ImplementationFactory);

            // Assert
            _ = startingContext.Should().BeEquivalentTo(expectedContext);
        }

        public static IEnumerable<object[]> UpdateImplementationFactory_VariousScenarios_ResultsInExpectedRegistrationContext_GetTestData()
            => new[]
            {
                new object[] { RegistrationContext_Empty, RegistrationContextWithFactory, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithSingleType, RegistrationContextWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithInstance, RegistrationContextWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithFactory, RegistrationContextWithFactory, RegistrationBehaviour_Default },
                new object[] { RegistrationContext_Empty, RegistrationContextWithFactory, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithSingleType, RegistrationContextWithSingleType, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithInstance, RegistrationContextWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithFactory, RegistrationContextWithFactory, RegistrationBehaviour_MultipleTypes },
            };

        #endregion UpdateImplementationFactory Tests

        #region - - - - - - UpdateImplementationInstance Tests - - - - - -

        [Theory]
        [MemberData(nameof(UpdateImplementationInstance_VariousScenarios_ResultsInExpectedRegistrationContext_GetTestData))]
        public void UpdateImplementationInstance_VariousScenarios_ResultsInExpectedRegistrationContext(RegistrationContext startingContext, RegistrationContext expectedContext, IRegistrationBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.UpdateImplementationInstance(startingContext, s_ImplementationInstance);

            // Assert
            _ = startingContext.Should().BeEquivalentTo(expectedContext);
        }

        public static IEnumerable<object[]> UpdateImplementationInstance_VariousScenarios_ResultsInExpectedRegistrationContext_GetTestData()
            => new[]
            {
                new object[] { RegistrationContext_Empty, RegistrationContextWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithSingleType, RegistrationContextWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithInstance, RegistrationContextWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationContextWithFactory, RegistrationContextWithFactory, RegistrationBehaviour_Default },
                new object[] { RegistrationContext_Empty, RegistrationContextWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithSingleType, RegistrationContextWithSingleType, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithInstance, RegistrationContextWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationContextWithFactory, RegistrationContextWithFactory, RegistrationBehaviour_MultipleTypes },
            };

        #endregion UpdateImplementationInstance Tests

        #region - - - - - - UpdateLifetime Tests - - - - - -

        [Fact]
        public void UpdateLifetime_TryChangeLifetime_LifetimeCannotBeChanged()
        {
            // Arrange
            var _Context = RegistrationContext_Empty;

            // Act
            _Context.Behaviour.UpdateLifetime(_Context, RegistrationLifetime.Singleton());

            // Assert
            _ = _Context.Should().BeEquivalentTo(RegistrationContext_Empty);
        }

        #endregion UpdateLifetime Tests

    }

}
