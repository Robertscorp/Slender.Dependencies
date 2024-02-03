using Slender.Dependencies.Internals;
using Slender.Dependencies.Options;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Slender.Dependencies
{

    /// <summary>
    /// A collection of dependencies that can be used to configure a dependency injection container.
    /// </summary>
    public class DependencyCollection : IDependencyCollection
    {

        #region - - - - - - Fields - - - - - -

        private readonly Dictionary<Type, IDependency> m_DependenciesByType = new Dictionary<Type, IDependency>();
        private readonly DependencyCollectionOptions m_Options = new DependencyCollectionOptions();
        private readonly List<Type> m_TransitiveDependencies = new List<Type>();

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="DependencyCollection"/> class.
        /// </summary>
        /// <param name="configureAction">An action to configure options for the dependency collection. Can be null.</param>
        public DependencyCollection(Action<DependencyCollectionOptions> configureAction = null)
            => configureAction?.Invoke(this.m_Options);

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <inheritdoc/>
        public void AddDependency(IDependency dependency)
        {
            if (dependency is null) throw new ArgumentNullException(nameof(dependency));

            var _DependencyType = dependency.GetDependencyType();

            this.m_DependenciesByType[_DependencyType]
                = this.m_DependenciesByType.TryGetValue(_DependencyType, out var _ExistingDependency)
                    ? this.m_Options.MergeResolutionStrategy(_ExistingDependency, dependency)
                    : dependency;
        }

        /// <inheritdoc/>
        public IDependency AddDependency(Type dependencyType)
        {
            if (dependencyType is null) throw new ArgumentNullException(nameof(dependencyType));

            if (this.m_DependenciesByType.TryGetValue(dependencyType, out var _Dependency))
                _Dependency = this.m_Options.DependencyExistsResolutionStrategy(this.m_Options.DependencyProvider, _Dependency, dependencyType);

            else
                _Dependency = this.m_Options.DependencyProvider(dependencyType);

            this.m_DependenciesByType[dependencyType] = _Dependency;
            return _Dependency;
        }

        /// <inheritdoc/>
        public void AddToDependencyCollection(IDependencyCollection dependencies)
        {
            if (dependencies is null) throw new ArgumentNullException(nameof(dependencies));

            this.m_DependenciesByType.Values.ToList().ForEach(d => dependencies.AddDependency(d));
            this.m_TransitiveDependencies.ForEach(d => dependencies.AddTransitiveDependency(d));
        }

        /// <inheritdoc/>
        public void AddTransitiveDependency(Type transitiveDependencyType)
        {
            if (transitiveDependencyType is null) throw new ArgumentNullException(nameof(transitiveDependencyType));

            var _Type = transitiveDependencyType.GetTypeDefinition();

            if (!this.m_TransitiveDependencies.Contains(_Type))
                this.m_TransitiveDependencies.Add(_Type);
        }

        /// <inheritdoc/>
        public IDependency GetDependency(Type dependencyType)
            => dependencyType is null
                ? throw new ArgumentNullException(nameof(dependencyType))
                : this.m_DependenciesByType.TryGetValue(dependencyType, out var _Dependency) ? _Dependency : null;

        #endregion Methods

    }

}
