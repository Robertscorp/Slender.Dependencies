using System;
using System.Collections.Generic;
using System.Linq;

namespace Slender.Dependencies.Tests.Support
{

    public class TestDependencyCollection : IDependencyCollection
    {

        #region - - - - - - Properties - - - - - -

        public List<IDependency> Dependencies { get; set; } = new();

        public List<Type> TransitiveDependencies { get; set; } = new();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void AddDependency(IDependency dependency)
            => this.Dependencies.Add(dependency);

        public IDependency AddDependency(Type dependencyType)
        {
            var _Dependency = new TestDependency(dependencyType);
            this.Dependencies.Add(_Dependency);
            return _Dependency;
        }

        public void AddToDependencyCollection(IDependencyCollection dependencies)
        {
            this.Dependencies.ForEach(d => dependencies.AddDependency(d));
            this.TransitiveDependencies.ForEach(td => dependencies.AddTransitiveDependency(td));
        }

        public void AddTransitiveDependency(Type transitiveDependencyType)
            => this.TransitiveDependencies.Add(transitiveDependencyType);

        public IDependency? GetDependency(Type dependencyType)
            => this.Dependencies.FirstOrDefault(d => d.GetDependencyType() == dependencyType);

        #endregion Methods

    }

}
