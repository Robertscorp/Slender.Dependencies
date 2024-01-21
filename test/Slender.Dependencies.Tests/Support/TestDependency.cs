using System;
using System.Collections.Generic;

namespace Slender.Dependencies.Tests.Support
{

    public class TestDependency : IDependency
    {

        #region - - - - - - Constructors - - - - - -

        public TestDependency(Type dependencyType)
            => this.DependencyType = dependencyType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public List<Type> Decorators { get; set; } = new();

        public Type DependencyType { get; set; }

        public List<object> Implementations { get; set; } = new();

        public DependencyLifetime? Lifetime { get; set; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        void IDependency.AddDecorator(Type decoratorType)
            => this.Decorators.Add(decoratorType);

        void IDependency.AddImplementation(object implementation)
            => this.Implementations.Add(implementation);

        void IDependency.AddToDependency(IDependency dependency)
        {
            dependency.SetLifetime(this.Lifetime);

            this.Decorators.ForEach(d => dependency.AddDecorator(d));
            this.Implementations.ForEach(d => dependency.AddImplementation(d));
        }

        Type IDependency.GetDependencyType()
            => this.DependencyType;

        void IDependency.SetLifetime(DependencyLifetime lifetime)
            => this.Lifetime = lifetime;

        #endregion Methods

    }

}
