using System;
using System.Collections.Generic;
using System.Linq;

namespace Slender.Dependencies.Internals
{

    internal class DependencyCollectionScanningDecorator : IDependencyCollection
    {

        #region - - - - - - Fields - - - - - -

        private readonly IDependencyCollection m_Dependencies;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public DependencyCollectionScanningDecorator(IDependencyCollection dependencies)
        {
            this.m_Dependencies = dependencies;

            var _Dependencies = new ReadOnlyDependencyCollection(dependencies);
            this.DependenciesByType = _Dependencies.Dependencies.ToDictionary(d => d.DependencyType, d => d.Dependency);
            this.RegisteredTransitiveDependencies = new HashSet<Type>(_Dependencies.TransitiveDependencies.Select(d => d.GetTypeDefinition()));
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public Dictionary<Type, IDependency> DependenciesByType { get; }

        public HashSet<Type> RegisteredTransitiveDependencies { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        public void AddDependency(IDependency dependency)
            => throw new InvalidOperationException($"Cannot directly add dependencies during assembly scanning. Use {nameof(AddDependency)}({nameof(Type)}) instead.");

        public IDependency AddDependency(Type dependencyType)
        {
            var _Dependency = this.m_Dependencies.AddDependency(dependencyType);
            if (_Dependency != null)
                this.DependenciesByType.Add(_Dependency.GetDependencyType(), _Dependency);

            return _Dependency;
        }

        public void AddToDependencyCollection(IDependencyCollection dependencies)
            => this.m_Dependencies.AddToDependencyCollection(dependencies);

        public void AddTransitiveDependency(Type transitiveDependencyType)
            => throw new InvalidOperationException("Cannot add transitive dependencies during assembly scanning.");

        public IDependency GetDependency(Type dependencyType)
            => this.DependenciesByType.TryGetValue(dependencyType, out var _Dependency) ? _Dependency : null;

        #endregion Methods

    }

}
