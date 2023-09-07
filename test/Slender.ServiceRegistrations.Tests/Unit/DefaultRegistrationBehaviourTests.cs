using FluentAssertions;
using Moq;
using Slender.ServiceRegistrations.Tests.Support;
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

        private static Registration Registration_Empty
            => new(typeof(object));

        private static Registration RegistrationWithFactory
            => new(typeof(object)) { ImplementationFactory = s_ImplementationFactory };

        private static Registration RegistrationWithInstance
            => new(typeof(object)) { ImplementationInstance = s_ImplementationInstance };

        private static Registration RegistrationWithMultipleTypes
            => new(typeof(object)) { ImplementationTypes = new List<Type>() { typeof(object), typeof(object) } };

        private static Registration RegistrationWithSingleType
            => new(typeof(object)) { ImplementationTypes = new List<Type>() { typeof(object) } };

        #endregion Properties

        #region - - - - - - AddImplementationType Tests - - - - - -

        [Theory]
        [MemberData(nameof(AddImplementationType_VariousScenarios_ResultsInExpectedRegistration_GetTestData))]
        public void AddImplementationType_VariousScenarios_ResultsInExpectedRegistration(Registration startingRegistration, Registration expectedRegistration, IRegistrationBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.AddImplementationType(startingRegistration, typeof(object));

            // Assert
            _ = startingRegistration.Should().BeEquivalentTo(expectedRegistration);
        }

        public static IEnumerable<object[]> AddImplementationType_VariousScenarios_ResultsInExpectedRegistration_GetTestData()
            => new[]
            {
                new object[] { Registration_Empty, RegistrationWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationWithSingleType, RegistrationWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationWithInstance, RegistrationWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationWithFactory, RegistrationWithFactory, RegistrationBehaviour_Default },
                new object[] { Registration_Empty, RegistrationWithSingleType, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithSingleType, RegistrationWithMultipleTypes, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithInstance, RegistrationWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithFactory, RegistrationWithFactory, RegistrationBehaviour_MultipleTypes },
            };

        #endregion AddImplementationType Tests

        #region - - - - - - AllowScannedImplementationTypes Tests - - - - - -

        [Fact]
        public void AllowScannedImplementationTypes_AnyRequest_RegistrationAllowsScannedImplementationTypes()
        {
            // Arrange
            var _Actual = new Registration(typeof(object));
            var _Expected = new Registration(typeof(object)) { AllowScannedImplementationTypes = true };

            // Act
            RegistrationBehaviour_Default.AllowScannedImplementationTypes(_Actual);

            // Assert
            _ = _Actual.Should().BeEquivalentTo(_Expected);
        }

        #endregion AllowScannedImplementationTypes Tests

        #region - - - - - - MergeRegistration Tests - - - - - -

        [Fact]
        public void MergeRegistration_AnyRegistration_TakesOnIncomingRegistrationAndAttemptsToOverrideWithExisting()
        {
            // Arrange
            var _MockRegistrationBehaviour = new Mock<IRegistrationBehaviour>();

            var _Builder = new RegistrationBuilder(typeof(object));
            _Builder.Registration.AllowScannedImplementationTypes = true;
            _Builder.Registration.Behaviour = RegistrationBehaviour_MultipleTypes;
            _Builder.Registration.ImplementationFactory = factory => string.Empty;
            _Builder.Registration.ImplementationInstance = string.Empty;
            _Builder.Registration.ImplementationTypes = new List<Type> { typeof(object), typeof(string) };
            _Builder.Registration.Lifetime = TestRegistrationLifetime.Instance(true);

            var _InitialRegistration = _Builder.Registration;

            var _Registration = new Registration(typeof(object))
            {
                Behaviour = _MockRegistrationBehaviour.Object,
                ImplementationTypes = new List<Type> { typeof(object), typeof(int) },
                Lifetime = TestRegistrationLifetime.Instance(true)
            };

            // Act
            _Builder.Registration.Behaviour.MergeRegistration(_Builder, _Registration);

            // Assert
            _ = _Builder.Registration.Should().BeEquivalentTo(_Registration);

            _MockRegistrationBehaviour.Verify(mock => mock.AddImplementationType(_Builder.Registration, typeof(string)));
            _MockRegistrationBehaviour.Verify(mock => mock.AllowScannedImplementationTypes(_Builder.Registration));
            _MockRegistrationBehaviour.Verify(mock => mock.UpdateBehaviour(_Builder.Registration, _InitialRegistration.Behaviour));
            _MockRegistrationBehaviour.Verify(mock => mock.UpdateImplementationFactory(_Builder.Registration, _InitialRegistration.ImplementationFactory));
            _MockRegistrationBehaviour.Verify(mock => mock.UpdateImplementationInstance(_Builder.Registration, _InitialRegistration.ImplementationInstance));
            _MockRegistrationBehaviour.Verify(mock => mock.UpdateLifetime(_Builder.Registration, _InitialRegistration.Lifetime));
            _MockRegistrationBehaviour.VerifyNoOtherCalls();
        }

        #endregion MergeRegistration Tests

        #region - - - - - - UpdateBehaviour Tests - - - - - -

        [Fact]
        public void UpdateLifetime_CanUpdateBehaviour_BehaviourChanged()
        {
            // Arrange
            var _Registration = Registration_Empty;

            // Act
            _Registration.Behaviour.UpdateBehaviour(_Registration, this.m_RegistrationBehaviour);

            // Assert
            _ = _Registration.Behaviour.Should().Be(this.m_RegistrationBehaviour);
        }

        [Fact]
        public void UpdateLifetime_CannotUpdateBehaviour_BehaviourDoesNotChange()
        {
            // Arrange
            var _Registration = new Registration(typeof(object)) { Behaviour = RegistrationBehaviour_DisableChange };
            var _Behaviour = _Registration.Behaviour;

            // Act
            _Registration.Behaviour.UpdateBehaviour(_Registration, this.m_RegistrationBehaviour);

            // Assert
            _ = _Registration.Behaviour.Should().Be(_Behaviour);
        }

        #endregion UpdateBehaviour Tests

        #region - - - - - - UpdateImplementationFactory Tests - - - - - -

        [Theory]
        [MemberData(nameof(UpdateImplementationFactory_VariousScenarios_ResultsInExpectedRegistration_GetTestData))]
        public void UpdateImplementationFactory_VariousScenarios_ResultsInExpectedRegistration(Registration startingRegistration, Registration expectedRegistration, IRegistrationBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.UpdateImplementationFactory(startingRegistration, s_ImplementationFactory);

            // Assert
            _ = startingRegistration.Should().BeEquivalentTo(expectedRegistration);
        }

        public static IEnumerable<object[]> UpdateImplementationFactory_VariousScenarios_ResultsInExpectedRegistration_GetTestData()
            => new[]
            {
                new object[] { Registration_Empty, RegistrationWithFactory, RegistrationBehaviour_Default },
                new object[] { RegistrationWithSingleType, RegistrationWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationWithInstance, RegistrationWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationWithFactory, RegistrationWithFactory, RegistrationBehaviour_Default },
                new object[] { Registration_Empty, RegistrationWithFactory, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithSingleType, RegistrationWithSingleType, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithInstance, RegistrationWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithFactory, RegistrationWithFactory, RegistrationBehaviour_MultipleTypes },
            };

        #endregion UpdateImplementationFactory Tests

        #region - - - - - - UpdateImplementationInstance Tests - - - - - -

        [Theory]
        [MemberData(nameof(UpdateImplementationInstance_VariousScenarios_ResultsInExpectedRegistration_GetTestData))]
        public void UpdateImplementationInstance_VariousScenarios_ResultsInExpectedRegistration(Registration startingRegistration, Registration expectedRegistration, IRegistrationBehaviour behaviour)
        {
            // Arrange

            // Act
            behaviour.UpdateImplementationInstance(startingRegistration, s_ImplementationInstance);

            // Assert
            _ = startingRegistration.Should().BeEquivalentTo(expectedRegistration);
        }

        public static IEnumerable<object[]> UpdateImplementationInstance_VariousScenarios_ResultsInExpectedRegistration_GetTestData()
            => new[]
            {
                new object[] { Registration_Empty, RegistrationWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationWithSingleType, RegistrationWithSingleType, RegistrationBehaviour_Default },
                new object[] { RegistrationWithInstance, RegistrationWithInstance, RegistrationBehaviour_Default },
                new object[] { RegistrationWithFactory, RegistrationWithFactory, RegistrationBehaviour_Default },
                new object[] { Registration_Empty, RegistrationWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithSingleType, RegistrationWithSingleType, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithInstance, RegistrationWithInstance, RegistrationBehaviour_MultipleTypes },
                new object[] { RegistrationWithFactory, RegistrationWithFactory, RegistrationBehaviour_MultipleTypes },
            };

        #endregion UpdateImplementationInstance Tests

        #region - - - - - - UpdateLifetime Tests - - - - - -

        [Fact]
        public void UpdateLifetime_TryChangeLifetime_LifetimeCannotBeChanged()
        {
            // Arrange
            var _Registration = Registration_Empty;

            // Act
            _Registration.Behaviour.UpdateLifetime(_Registration, TestRegistrationLifetime.Instance(true));

            // Assert
            _ = _Registration.Should().BeEquivalentTo(Registration_Empty);
        }

        #endregion UpdateLifetime Tests

    }

}
