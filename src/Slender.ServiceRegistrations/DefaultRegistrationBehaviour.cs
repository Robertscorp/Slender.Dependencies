using System;
using System.Linq;

namespace Slender.ServiceRegistrations
{

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

        void IRegistrationBehaviour.UpdateImplementationFactory(RegistrationContext context, Func<object> implementationFactory)
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
