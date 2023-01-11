using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// The context that contains information for a service registration.
    /// </summary>
    public class RegistrationContext
    {

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// The configuration behaviour of the service registration.
        /// </summary>
        public IRegistrationBehaviour Behaviour { get; set; } = DefaultRegistrationBehaviour.Instance();

        /// <summary>
        /// A factory that produces an instance that can be assigned to a reference of the registered service.
        /// </summary>
        public Func<object> ImplementationFactory { get; set; }

        /// <summary>
        /// An instance that can be assigned to a reference of the registered service.
        /// </summary>
        public object ImplementationInstance { get; set; }

        /// <summary>
        /// A list of types that implement or inherit the registered service.
        /// </summary>
        public List<Type> ImplementationTypes { get; set; } = new List<Type>();

        /// <summary>
        /// The service lifetime of the service registration.
        /// </summary>
        public RegistrationLifetime Lifetime { get; set; }

        #endregion Properties

    }

}
