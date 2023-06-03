using System;

namespace Slender.ServiceRegistrations
{

    public partial class RegistrationCollection
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as a transient service implementation.
        /// </summary>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddTransient<TImplementation>()
            => this.AddService(typeof(TImplementation), RegistrationLifetime.Transient(), r => r.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as the transient implementation type of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddTransient<TService, TImplementation>() where TImplementation : TService
            => this.AddService(typeof(TService), RegistrationLifetime.Transient(), r => r.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <paramref name="implementationFactory"/> as the mechanism of providing a transient instance of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="implementationFactory">A factory which produces an instance that can be assigned to a <typeparamref name="TService"/> reference.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddTransient<TService>(Func<ServiceFactory, TService> implementationFactory) where TService : class
            => this.AddService(typeof(TService), RegistrationLifetime.Transient(), r => r.WithImplementationFactory(implementationFactory));

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a transient service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{RegistrationBuilder})"/>.</remarks>
        public RegistrationCollection AddTransient(Type type, Action<RegistrationBuilder> configurationAction = null)
            => this.AddService(type, RegistrationLifetime.Transient(), configurationAction);

        #endregion Methods

    }

}
