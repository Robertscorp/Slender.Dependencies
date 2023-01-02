using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    public class Registration
    {

        #region - - - - - - Fields - - - - - -

        private readonly RegistrationContext m_Context = new RegistrationContext();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public Registration(Type serviceType, RegistrationLifetime lifetime)
        {
            this.m_Context.Lifetime = lifetime ?? throw new ArgumentNullException(nameof(lifetime));
            this.ServiceType = serviceType ?? throw new ArgumentNullException(nameof(serviceType));
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public Func<object> ImplementationFactory => this.m_Context.ImplementationFactory;

        public object ImplementationInstance => this.m_Context.ImplementationInstance;

        public IEnumerable<Type> ImplementationTypes => this.m_Context.ImplementationTypes.AsReadOnly();

        public Type ServiceType { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public Registration AddImplementationType<TImplementation>()
            => this.AddImplementationType(typeof(TImplementation));

        public Registration AddImplementationType(Type implementationType)
        {
            if (implementationType is null) throw new ArgumentNullException(nameof(implementationType));

            this.m_Context.Behaviour.AddImplementationType(this.m_Context, implementationType);

            return this;
        }

        public Registration WithImplementationFactory(Func<object> implementationFactory)
        {
            if (implementationFactory is null) throw new ArgumentNullException(nameof(implementationFactory));

            this.m_Context.Behaviour.UpdateImplementationFactory(this.m_Context, implementationFactory);

            return this;
        }

        public Registration WithImplementationInstance(object implementationInstance)
        {
            if (implementationInstance is null)
                throw new ArgumentNullException(nameof(implementationInstance));

            if (!this.m_Context.Lifetime.AllowImplementationInstances)
                throw new Exception($"{this.m_Context.Lifetime} Lifetime does not allow implementation instances.");

            this.m_Context.Behaviour.UpdateImplementationInstance(this.m_Context, implementationInstance);

            return this;
        }

        public Registration WithLifetime(RegistrationLifetime lifetime)
        {
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            this.m_Context.Behaviour.UpdateLifetime(this.m_Context, lifetime);

            return this;
        }

        public Registration WithRegistrationBehaviour(IRegistrationBehaviour behaviour)
        {
            if (behaviour is null) throw new ArgumentNullException(nameof(behaviour));

            this.m_Context.Behaviour.UpdateBehaviour(this.m_Context, behaviour);

            return this;
        }

        #endregion Methods

    }

}
