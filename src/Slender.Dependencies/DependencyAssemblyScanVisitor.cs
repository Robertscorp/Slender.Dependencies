using Slender.AssemblyScanner;
using System;
using System.Collections.Generic;

namespace Slender.Dependencies
{

    internal class DependencyAssemblyScanVisitor : AssemblyScanVisitor
    {

        #region - - - - - - Properties - - - - - -

        public Action<Type> OnDependencyFound { get; set; }

        public Action<Type, IEnumerable<Type>> OnDependencyImplementationsFound { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        protected override void VisitAbstract(Type abstractType)
            => this.OnDependencyFound?.Invoke(abstractType);

        protected override void VisitAbstractAndImplementations(Type abstractType, IEnumerable<Type> implementationTypes)
            => this.OnDependencyImplementationsFound?.Invoke(abstractType, implementationTypes);

        protected override void VisitInterface(Type interfaceType)
            => this.OnDependencyFound?.Invoke(interfaceType);

        protected override void VisitInterfaceAndImplementations(Type interfaceType, IEnumerable<Type> implementationTypes)
            => this.OnDependencyImplementationsFound?.Invoke(interfaceType, implementationTypes);

        #endregion Methods

    }

}
