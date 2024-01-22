using System;
using System.Collections.Generic;

namespace Slender.Dependencies.Internals
{

    internal class ReadOnlyDependency : IDependency
    {

        #region - - - - - - Fields - - - - - -

        private readonly bool m_IsLocked;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ReadOnlyDependency(IDependency dependency)
        {
            this.Dependency = dependency;
            this.DependencyType = dependency.GetDependencyType();
            dependency.AddToDependency(this);
            this.m_IsLocked = true;
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public List<Type> Decorators { get; } = new List<Type>();

        internal IDependency Dependency { get; }

        public Type DependencyType { get; }

        public List<object> Implementations { get; } = new List<object>();

        public DependencyLifetime Lifetime { get; private set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        void IDependency.AddDecorator(Type decoratorType)
        {
            if (!this.m_IsLocked)
                this.Decorators.Add(decoratorType);
        }

        void IDependency.AddImplementation(object implementation)
        {
            if (!this.m_IsLocked)
                this.Implementations.Add(implementation);
        }

        void IDependency.AddToDependency(IDependency dependency)
            => throw new NotImplementedException();

        Type IDependency.GetDependencyType()
            => this.DependencyType;

        void IDependency.SetLifetime(DependencyLifetime lifetime)
        {
            if (!this.m_IsLocked)
                this.Lifetime = lifetime;
        }

        #endregion Methods

    }

}
