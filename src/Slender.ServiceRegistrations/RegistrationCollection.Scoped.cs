using System;

namespace Slender.ServiceRegistrations
{

    public partial class RegistrationCollection
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as a scoped service implementation.
        /// </summary>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddScoped<TImplementation>()
            => this.AddService(typeof(TImplementation), RegistrationLifetime.Scoped(), r => r.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as the scoped implementation type of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddScoped<TService, TImplementation>() where TImplementation : TService
            => this.AddService(typeof(TService), RegistrationLifetime.Scoped(), r => r.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <paramref name="implementationFactory"/> as the mechanism of providing a scoped instance of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="implementationFactory">A factory which produces an instance that can be assigned to a <typeparamref name="TService"/> reference.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddScoped<TService>(Func<ServiceFactory, TService> implementationFactory) where TService : class
            => this.AddService(typeof(TService), RegistrationLifetime.Scoped(), r => r.WithImplementationFactory(implementationFactory));

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a scoped service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddScoped(Type type, Action<RegistrationBuilder> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Scoped(), configurationAction);

        #endregion Methods

    }

}
