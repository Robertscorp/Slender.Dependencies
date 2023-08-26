using System;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// Used to determine the configuration behaviours of a registered service.
    /// </summary>
    public interface IRegistrationBehaviour
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds an implementation type to the registered service.
        /// </summary>
        /// <param name="registration">The registration to update.</param>
        /// <param name="type">A <see cref="Type"/> which implements or inherits the registered service.</param>
        void AddImplementationType(Registration registration, Type type);

        /// <summary>
        /// Changes the registration to allow scanned implementation types to be added.
        /// </summary>
        /// <param name="registration">The registration to update.</param>
        void AllowScannedImplementationTypes(Registration registration);

        /// <summary>
        /// Merges the specified <paramref name="registration"/> into the registered service.
        /// </summary>
        /// <param name="builder">The builder for the registered service to update.</param>
        /// <param name="registration">The registered service being merged in.</param>
        void MergeRegistration(RegistrationBuilder builder, Registration registration);

        /// <summary>
        /// Changes the registration behaviour of the registered service.
        /// </summary>
        /// <param name="registration">The registration to update.</param>
        /// <param name="behaviour">The new behaviour for the registered service.</param>
        void UpdateBehaviour(Registration registration, IRegistrationBehaviour behaviour);

        /// <summary>
        /// Sets the implementation factory of the registered service.
        /// </summary>
        /// <param name="registration">The registration to update.</param>
        /// <param name="implementationFactory">A factory which produces an instance that can be assigned to a reference of the registered service.</param>
        void UpdateImplementationFactory(Registration registration, Func<ServiceFactory, object> implementationFactory);

        /// <summary>
        /// Sets the implementation instance of the registered service.
        /// </summary>
        /// <param name="registration">The registration to update.</param>
        /// <param name="implementationInstance">An instance that can be assigned to a reference of the registered service.</param>
        void UpdateImplementationInstance(Registration registration, object implementationInstance);

        /// <summary>
        /// Changes the service lifetime of the registered service.
        /// </summary>
        /// <param name="registration">The registration to update.</param>
        /// <param name="lifetime">The new service lifetime for the registered service.</param>
        void UpdateLifetime(Registration registration, RegistrationLifetime lifetime);

        #endregion Methods

    }

}
