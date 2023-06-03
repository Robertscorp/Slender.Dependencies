using System;
using System.Linq;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// The default configuration behaviour.
    /// </summary>
    public class DefaultRegistrationBehaviour : IRegistrationBehaviour
    {

        #region - - - - - - Fields - - - - - -

        private readonly bool m_AllowBehaviourToChange;
        private readonly bool m_AllowMultipleImplementationTypes;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        private DefaultRegistrationBehaviour(bool allowBehaviourToChange, bool allowMultipleImplementationTypes)
        {
            this.m_AllowBehaviourToChange = allowBehaviourToChange;
            this.m_AllowMultipleImplementationTypes = allowMultipleImplementationTypes;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Gets a new instance of the default registration behaviour.
        /// </summary>
        /// <param name="allowBehaviourToChange">Determines if a registered service can have its behaviour changed. Default = true.</param>
        /// <param name="allowMultipleImplementationTypes">Determines if a registered service can have multiple implementation types registered against it. Default = false.</param>
        /// <returns>An instance of the default registration behaviour.</returns>
        public static DefaultRegistrationBehaviour Instance(bool allowBehaviourToChange = true, bool allowMultipleImplementationTypes = false)
            => new DefaultRegistrationBehaviour(allowBehaviourToChange, allowMultipleImplementationTypes);

        private bool CanRegisterImplementation(Registration registration, bool isImplementationType)
            => registration.ImplementationFactory == null &&
                registration.ImplementationInstance == null &&
                (!registration.ImplementationTypes.Any() || (this.m_AllowMultipleImplementationTypes && isImplementationType));

        void IRegistrationBehaviour.AddImplementationType(Registration registration, Type type)
        {
            if (this.CanRegisterImplementation(registration, true)) registration.ImplementationTypes.Add(type);
        }

        void IRegistrationBehaviour.AllowScannedImplementationTypes(Registration registration)
            => registration.AllowScannedImplementationTypes = true;

        void IRegistrationBehaviour.UpdateBehaviour(Registration registration, IRegistrationBehaviour behaviour)
        {
            if (this.m_AllowBehaviourToChange) registration.Behaviour = behaviour;
        }

        void IRegistrationBehaviour.UpdateImplementationFactory(Registration registration, Func<ServiceFactory, object> implementationFactory)
        {
            if (this.CanRegisterImplementation(registration, false)) registration.ImplementationFactory = implementationFactory;
        }

        void IRegistrationBehaviour.UpdateImplementationInstance(Registration registration, object implementationInstance)
        {
            if (this.CanRegisterImplementation(registration, false)) registration.ImplementationInstance = implementationInstance;
        }

        void IRegistrationBehaviour.UpdateLifetime(Registration registration, RegistrationLifetime lifetime) { }

        #endregion Methods

    }

}
