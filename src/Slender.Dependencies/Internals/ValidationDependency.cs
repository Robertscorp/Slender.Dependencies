using System;
using System.Collections.Generic;
using System.Linq;

namespace Slender.Dependencies.Internals
{

    internal class ValidationDependency : IDependency
    {

        #region - - - - - - Fields - - - - - -

        private readonly List<Type> m_Decorators = new List<Type>();
        private readonly Type m_DependencyType;
        private readonly List<object> m_Implementations = new List<object>();

        private DependencyLifetime m_Lifetime;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ValidationDependency(IDependency dependency)
        {
            this.m_DependencyType = dependency.GetDependencyType();
            dependency.AddToDependency(this);
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        public void AddDecorator(Type decoratorType)
            => this.m_Decorators.Add(decoratorType);

        public void AddImplementation(object implementation)
            => this.m_Implementations.Add(implementation);

        public Type GetDependencyType()
            => this.m_DependencyType;

        public bool HasInvalidImplementations()
            => this.m_Lifetime != null && this.m_Implementations.Any(i => !this.m_Lifetime.SupportsImplementation(i));

        public bool HasNoImplementations()
            => !this.m_Implementations.Any();

        public bool HasNoLifetime()
            => this.m_Lifetime == null;

        void IDependency.AddToDependency(IDependency dependency)
            => throw new NotImplementedException();

        public void SetLifetime(DependencyLifetime lifetime)
            => this.m_Lifetime = lifetime;

        #endregion Methods

    }

}
