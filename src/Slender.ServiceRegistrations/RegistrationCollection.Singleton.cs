using System;

namespace Slender.ServiceRegistrations
{

    public partial class RegistrationCollection
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as a singleton service implementation.
        /// </summary>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{Registration})"/>.</remarks>
        public RegistrationCollection AddSingleton<TImplementation>()
            => this.AddService(typeof(TImplementation), RegistrationLifetime.Singleton(), r => r.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers the specified <paramref name="implementationInstance"/> as a singleton instance.
        /// </summary>
        /// <typeparam name="TImplementation">The type of singleton instance.</typeparam>
        /// <param name="implementationInstance">The instance to be used when resolving a <typeparamref name="TImplementation"/> reference.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{Registration})"/>.</remarks>
        public RegistrationCollection AddSingleton<TImplementation>(TImplementation implementationInstance)
            => this.AddService(typeof(TImplementation), RegistrationLifetime.Singleton(), r => r.WithImplementationInstance(implementationInstance));

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as the singleton implementation type of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{Registration})"/>.</remarks>
        public RegistrationCollection AddSingleton<TService, TImplementation>() where TImplementation : TService
            => this.AddService(typeof(TService), RegistrationLifetime.Singleton(), r => r.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <paramref name="implementationFactory"/> as the mechanism of providing a singleton instance of <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="implementationFactory">A factory which produces an instance that can be assigned to a <typeparamref name="TService"/> reference.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{Registration})"/>.</remarks>
        public RegistrationCollection AddSingleton<TService>(Func<ServiceFactory, TService> implementationFactory) where TService : class
            => this.AddService(typeof(TService), RegistrationLifetime.Singleton(), r => r.WithImplementationFactory(implementationFactory));

        /// <summary>
        /// Registers the specified <paramref name="implementationInstance"/> as the singleton instance for <typeparamref name="TService"/>.
        /// </summary>
        /// <typeparam name="TService">The type of service.</typeparam>
        /// <param name="implementationInstance">The instance to be used when resolving a <typeparamref name="TService"/> reference.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{Registration})"/>.</remarks>
        public RegistrationCollection AddSingleton<TService>(object implementationInstance)
            => this.AddService(typeof(TService), RegistrationLifetime.Singleton(), r => r.WithImplementationInstance(implementationInstance));

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a singleton service.
        /// </summary>
        /// <param name="type">The type of service.</param>
        /// <param name="configurationAction">An action to configure the registered service.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding services, see <see cref="RegistrationCollection.AddService(Type, RegistrationLifetime, Action{Registration})"/>.</remarks>
        public RegistrationCollection AddSingleton(Type type, Action<Registration> configurationAction)
            => this.AddService(type, RegistrationLifetime.Singleton(), configurationAction);

        #endregion Methods

    }

}
