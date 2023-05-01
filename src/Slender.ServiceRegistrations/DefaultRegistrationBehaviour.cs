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
        /// <param name="allowBehaviourToChange">Determines if a service registration can have its behaviour changed. Default = true.</param>
        /// <param name="allowMultipleImplementationTypes">Determines if a service registration can have multiple implementation types registered against it. Default = false.</param>
        /// <returns>An instance of the default registration behaviour.</returns>
        public static DefaultRegistrationBehaviour Instance(bool allowBehaviourToChange = true, bool allowMultipleImplementationTypes = false)
            => new DefaultRegistrationBehaviour(allowBehaviourToChange, allowMultipleImplementationTypes);

        private bool CanRegisterImplementation(RegistrationContext context, bool isImplementationType)
            => context.ImplementationFactory == null &&
                context.ImplementationInstance == null &&
                (!context.ImplementationTypes.Any() || (this.m_AllowMultipleImplementationTypes && isImplementationType));

        void IRegistrationBehaviour.AddImplementationType(RegistrationContext context, Type type)
        {
            if (this.CanRegisterImplementation(context, true)) context.ImplementationTypes.Add(type);
        }

        void IRegistrationBehaviour.UpdateBehaviour(RegistrationContext context, IRegistrationBehaviour behaviour)
        {
            if (this.m_AllowBehaviourToChange) context.Behaviour = behaviour;
        }

        void IRegistrationBehaviour.UpdateImplementationFactory(RegistrationContext context, Func<ServiceFactory, object> implementationFactory)
        {
            if (this.CanRegisterImplementation(context, false)) context.ImplementationFactory = implementationFactory;
        }

        void IRegistrationBehaviour.UpdateImplementationInstance(RegistrationContext context, object implementationInstance)
        {
            if (this.CanRegisterImplementation(context, false)) context.ImplementationInstance = implementationInstance;
        }

        void IRegistrationBehaviour.UpdateLifetime(RegistrationContext context, RegistrationLifetime lifetime) { }

        #endregion Methods

    }

}
