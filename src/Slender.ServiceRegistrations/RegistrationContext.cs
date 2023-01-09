using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    public class RegistrationContext
    {

        #region - - - - - - Properties - - - - - -

        public IRegistrationBehaviour Behaviour { get; set; } = DefaultRegistrationBehaviour.Instance();

        public Func<object> ImplementationFactory { get; set; }

        public object ImplementationInstance { get; set; }

        public List<Type> ImplementationTypes { get; set; } = new List<Type>();

        public RegistrationLifetime Lifetime { get; set; }

        #endregion Properties

    }

}
