using System;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// A factory used to produce an instance of the specified service <see cref="Type"/>.
    /// </summary>
    /// <param name="serviceType">The type of service to produce.</param>
    /// <returns>An instance of the specified service, or null.</returns>
    public delegate object ServiceFactory(Type serviceType);

    /// <summary>
    /// Contains <see cref="ServiceFactory"/> extension methods.
    /// </summary>
    public static class ServiceFactoryExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Produces an instance of the <typeparamref name="TService"/> service.
        /// </summary>
        /// <typeparam name="TService">The type of service to produce.</typeparam>
        /// <param name="serviceFactory">The service factory used to produce the service.</param>
        /// <returns>An instance of the <typeparamref name="TService"/> service, or null.</returns>
        public static TService GetService<TService>(this ServiceFactory serviceFactory)
            => (TService)serviceFactory(typeof(TService));

        #endregion Methods

    }

}
