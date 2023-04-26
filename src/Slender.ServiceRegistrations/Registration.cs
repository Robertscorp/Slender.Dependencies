using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// Contains information about how a service should be registered with a dependency injection container.
    /// </summary>
    public class Registration
    {

        #region - - - - - - Fields - - - - - -

        private readonly RegistrationContext m_Context = new RegistrationContext();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Creates a new instance of a service registration.
        /// </summary>
        /// <param name="serviceType">The type of service being registered.</param>
        /// <param name="lifetime">The lifetime of the service.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="serviceType"/> or <paramref name="lifetime"/> is null.</exception>
        public Registration(Type serviceType, RegistrationLifetime lifetime)
        {
            this.m_Context.Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            this.ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// A factory which produces an instance that can be assigned to a reference of the registered service.
        /// </summary>
        public Func<object> ImplementationFactory => this.m_Context.ImplementationFactory;

        /// <summary>
        /// An instance that can be assigned to a reference of the registered service.
        /// </summary>
        public object ImplementationInstance => this.m_Context.ImplementationInstance;

        /// <summary>
        /// A list of types that implement or inherit the registered service.
        /// </summary>
        public IEnumerable<Type> ImplementationTypes => this.m_Context.ImplementationTypes.AsReadOnly();

        /// <summary>
        /// The type of the registered service.
        /// </summary>
        public Type ServiceType { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds <typeparamref name="TImplementation"/> as an implementation type to the the service registration.
        /// </summary>
        /// <typeparam name="TImplementation">The implementation type that implements or inherits the registered service.</typeparam>
        /// <returns>Itself.</returns>
        public Registration AddImplementationType<TImplementation>()
            => this.AddImplementationType(typeof(TImplementation));

        /// <summary>
        /// Adds an implementation type to the the service registration.
        /// </summary>
        /// <param name="implementationType">The type that implements or inherits the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationType"/> is null.</exception>
        public Registration AddImplementationType(Type implementationType)
        {
            if (implementationType is null) throw new ArgumentNullException(nameof(implementationType));

            this.m_Context.Behaviour.AddImplementationType(this.m_Context, implementationType);

            return this;
        }

        /// <summary>
        /// Sets the implementation factory of the service registration.
        /// </summary>
        /// <param name="implementationFactory">A factory which produces an instance that can be assigned to a reference of the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationFactory"/> is null.</exception>
        public Registration WithImplementationFactory(Func<object> implementationFactory)
        {
            if (implementationFactory is null) throw new ArgumentNullException(nameof(implementationFactory));

            this.m_Context.Behaviour.UpdateImplementationFactory(this.m_Context, implementationFactory);

            return this;
        }

        /// <summary>
        /// Sets the implementation instance of the service registration.
        /// </summary>
        /// <param name="implementationInstance">An instance that can be assigned to a reference of the registered service.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationInstance"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the service registration lifetime doesn't allow implementation instances.</exception>
        public Registration WithImplementationInstance(object implementationInstance)
        {
            if (implementationInstance is null)
                throw new ArgumentNullException(nameof(implementationInstance));

            if (!this.m_Context.Lifetime.AllowImplementationInstances)
                throw new Exception($"{this.m_Context.Lifetime} Lifetime does not allow implementation instances.");

            this.m_Context.Behaviour.UpdateImplementationInstance(this.m_Context, implementationInstance);

            return this;
        }

        /// <summary>
        /// Sets the service lifetime of the service registration.
        /// </summary>
        /// <param name="lifetime">The new service lifetime for the service registration.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lifetime"/> is null.</exception>
        public Registration WithLifetime(RegistrationLifetime lifetime)
        {
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            this.m_Context.Behaviour.UpdateLifetime(this.m_Context, lifetime);

            return this;
        }

        /// <summary>
        /// Sets the registration behaviour of the service registration.
        /// </summary>
        /// <param name="behaviour">The new behaviour for the service registration.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="behaviour"/> is null.</exception>
        public Registration WithRegistrationBehaviour(IRegistrationBehaviour behaviour)
        {
            if (behaviour is null) throw new ArgumentNullException(nameof(behaviour));

            this.m_Context.Behaviour.UpdateBehaviour(this.m_Context, behaviour);

            return this;
        }

        #endregion Methods

    }

}
