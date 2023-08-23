using System;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// A builder that is used to configure a <see cref="ServiceRegistrations.Registration"/>.
    /// </summary>
    public class RegistrationBuilder
    {

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Creates a new instance of a registered service builder.
        /// </summary>
        /// <param name="serviceType">The type of service being registered.</param>
        /// <param name="lifetime">The lifetime of the registered service.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceType"/> or <paramref name="lifetime"/> is null.</exception>
        public RegistrationBuilder(Type serviceType, RegistrationLifetime lifetime)
            => this.Registration = new Registration(serviceType ?? throw new ArgumentNullException(nameof(serviceType)))
            {
                Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime)),
            };

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Action OnScanForImplementations { get; set; }

        internal Registration Registration { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds <typeparamref name="TImplementation"/> as an implementation type to the registered service.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type that implements or inherits the registered service.</typeparam>
        /// <returns>Itself.</returns>
        public RegistrationBuilder AddImplementationType<TImplementation>()
            => this.AddImplementationType(typeof(TImplementation));

        /// <summary>
        /// Adds an implementation type to the registered service.
        /// </summary>
        /// <param name="implementationType">The type that implements or inherits the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationType"/> is null.</exception>
        public RegistrationBuilder AddImplementationType(Type implementationType)
        {
            if (implementationType is null) throw new ArgumentNullException(nameof(implementationType));

            if (!this.Registration.ImplementationTypes.Contains(implementationType))
                this.Registration.Behaviour.AddImplementationType(this.Registration, implementationType);

            return this;
        }

        /// <summary>
        /// Allows scanned implementations of this service to be added to the registered service.
        /// </summary>
        /// <remarks>
        /// For more information on scanned types, see <see cref="RegistrationCollection.AddAssemblyScan(AssemblyScanner.IAssemblyScan)"/>.
        /// </remarks>
        public RegistrationBuilder ScanForImplementations()
        {
            this.Registration.Behaviour.AllowScannedImplementationTypes(this.Registration);
            this.OnScanForImplementations?.Invoke();
            return this;
        }

        /// <summary>
        /// Sets the implementation factory of the registered service.
        /// </summary>
        /// <param name="implementationFactory">A factory which produces an instance that can be assigned to a reference of the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationFactory"/> is null.</exception>
        public RegistrationBuilder WithImplementationFactory(Func<ServiceFactory, object> implementationFactory)
        {
            if (implementationFactory is null) throw new ArgumentNullException(nameof(implementationFactory));

            this.Registration.Behaviour.UpdateImplementationFactory(this.Registration, implementationFactory);

            return this;
        }

        /// <summary>
        /// Sets the implementation instance of the registered service.
        /// </summary>
        /// <param name="implementationInstance">An instance that can be assigned to a reference of the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationInstance"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the registered service lifetime doesn't allow implementation instances.</exception>
        public RegistrationBuilder WithImplementationInstance(object implementationInstance)
        {
            if (implementationInstance is null)
                throw new ArgumentNullException(nameof(implementationInstance));

            if (!this.Registration.Lifetime.AllowImplementationInstances)
                throw new Exception($"{this.Registration.Lifetime} Lifetime does not allow implementation instances.");

            this.Registration.Behaviour.UpdateImplementationInstance(this.Registration, implementationInstance);

            return this;
        }

        /// <summary>
        /// Sets the service lifetime of the registered service.
        /// </summary>
        /// <param name="lifetime">The new service lifetime for the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lifetime"/> is null.</exception>
        public RegistrationBuilder WithLifetime(RegistrationLifetime lifetime)
        {
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            this.Registration.Behaviour.UpdateLifetime(this.Registration, lifetime);

            return this;
        }

        /// <summary>
        /// Sets the details of the registered service.
        /// </summary>
        /// <param name="newRegistration">The new details for the registered service.</param>
        /// <param name="oldRegistration">The old details of the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newRegistration"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newRegistration"/> has a different <see cref="Registration.ServiceType"/>.</exception>
        public RegistrationBuilder WithRegistration(Registration newRegistration, out Registration oldRegistration)
        {
            if (newRegistration is null) throw new ArgumentNullException(nameof(newRegistration));
            if (newRegistration.ServiceType != this.Registration.ServiceType) throw new ArgumentException("Cannot change the type of the registered service.");

            oldRegistration = this.Registration;

            this.Registration = new Registration(this.Registration.ServiceType)
            {
                AllowScannedImplementationTypes = newRegistration.AllowScannedImplementationTypes,
                Behaviour = newRegistration.Behaviour,
                ImplementationFactory = newRegistration.ImplementationFactory,
                ImplementationInstance = newRegistration.ImplementationInstance,
                ImplementationTypes = newRegistration.ImplementationTypes,
                Lifetime = newRegistration.Lifetime
            };

            return this;
        }

        /// <summary>
        /// Sets the registration behaviour of the registered service.
        /// </summary>
        /// <param name="behaviour">The new behaviour for the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="behaviour"/> is null.</exception>
        public RegistrationBuilder WithRegistrationBehaviour(IRegistrationBehaviour behaviour)
        {
            if (behaviour is null) throw new ArgumentNullException(nameof(behaviour));

            this.Registration.Behaviour.UpdateBehaviour(this.Registration, behaviour);

            return this;
        }

        #endregion Methods

    }

}
