using System;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// Used to determine the configuration behaviours of a service registration.
    /// </summary>
    public interface IRegistrationBehaviour
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds an implementation type to the the registration context.
        /// </summary>
        /// <param name="context">The registration context to update.</param>
        /// <param name="type">The type that implements or inherits the registered service.</param>
        void AddImplementationType(RegistrationContext context, Type type);

        /// <summary>
        /// Changes the registration behaviour of the registration context.
        /// </summary>
        /// <param name="context">The registration context to update.</param>
        /// <param name="behaviour">The new behaviour for the registration context.</param>
        void UpdateBehaviour(RegistrationContext context, IRegistrationBehaviour behaviour);

        /// <summary>
        /// Sets the implementation factory of the registration context.
        /// </summary>
        /// <param name="context">The registration context to update.</param>
        /// <param name="implementationFactory">A factory which produces an instance that can be assigned to a reference of the registered service.</param>
        void UpdateImplementationFactory(RegistrationContext context, Func<ServiceFactory, object> implementationFactory);

        /// <summary>
        /// Sets the implementation instance of the registration context.
        /// </summary>
        /// <param name="context">The registration context to update.</param>
        /// <param name="implementationInstance">An instance that can be assigned to a reference of the registered service.</param>
        void UpdateImplementationInstance(RegistrationContext context, object implementationInstance);

        /// <summary>
        /// Changes the service lifetime of the service registration.
        /// </summary>
        /// <param name="context">The registration context to update.</param>
        /// <param name="lifetime">The new service lifetime for the service registration.</param>
        void UpdateLifetime(RegistrationContext context, RegistrationLifetime lifetime);

        #endregion Methods

    }

}
