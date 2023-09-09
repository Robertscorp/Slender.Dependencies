using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    internal class ServiceAndImplementationScanVisitor : AssemblyScanVisitor
    {

        #region - - - - - - Properties - - - - - -

        public Action<Type> OnServiceFound { get; set; }

        public Action<Type, IEnumerable<Type>> OnServiceAndImplementationsFound { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        protected override void VisitAbstract(Type abstractType)
            => this.OnServiceFound?.Invoke(abstractType);

        protected override void VisitAbstractAndImplementations(Type abstractType, IEnumerable<Type> implementationTypes)
            => this.OnServiceAndImplementationsFound?.Invoke(abstractType, implementationTypes);

        protected override void VisitInterface(Type interfaceType)
            => this.OnServiceFound?.Invoke(interfaceType);

        protected override void VisitInterfaceAndImplementations(Type interfaceType, IEnumerable<Type> implementationTypes)
            => this.OnServiceAndImplementationsFound?.Invoke(interfaceType, implementationTypes);

        #endregion Methods

    }

}
