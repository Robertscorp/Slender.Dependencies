using System;

namespace Slender.ServiceRegistrations
{

    public interface IRegistrationBehaviour
    {

        #region - - - - - - Methods - - - - - -

        void AddImplementationType(RegistrationContext context, Type type);

        void UpdateBehaviour(RegistrationContext context, IRegistrationBehaviour behaviour);

        void UpdateImplementationFactory(RegistrationContext context, Func<object> implementationFactory);

        void UpdateImplementationInstance(RegistrationContext context, object implementationInstance);

        void UpdateLifetime(RegistrationContext context, RegistrationLifetime lifetime);

        #endregion Methods

    }

}
