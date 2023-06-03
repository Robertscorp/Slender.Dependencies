using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// A service that has been registered with a <see cref="RegistrationCollection"/>.
    /// </summary>
    public class Registration
    {

        #region - - - - - - Constructors - - - - - -

        internal Registration(Type serviceType)
            => this.ServiceType = serviceType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Determines if scanned implementation types are allowed to be added to the registered service.
        /// </summary>
        public bool AllowScannedImplementationTypes { get; set; }

        /// <summary>
        /// The configuration behaviour of the registered service.
        /// </summary>
        public IRegistrationBehaviour Behaviour { get; set; } = DefaultRegistrationBehaviour.Instance();

        /// <summary>
        /// A factory which produces an instance that can be assigned to a reference of the registered service.
        /// </summary>
        public Func<ServiceFactory, object> ImplementationFactory { get; set; }

        /// <summary>
        /// An instance that can be assigned to a reference of the registered service.
        /// </summary>
        public object ImplementationInstance { get; set; }

        /// <summary>
        /// A list of types that implement or inherit the registered service.
        /// </summary>
        public List<Type> ImplementationTypes { get; set; } = new List<Type>();

        /// <summary>
        /// The lifetime of the registered service.
        /// </summary>
        public RegistrationLifetime Lifetime { get; set; }

        /// <summary>
        /// The type of the registered service.
        /// </summary>
        public Type ServiceType { get; }

        #endregion Properties

    }

}
