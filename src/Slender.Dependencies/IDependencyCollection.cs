using System;

namespace Slender.Dependencies
{

    /// <summary>
    /// Contains dependencies that can be used to configure a dependency injection container.
    /// </summary>
    public interface IDependencyCollection
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds the specified <paramref name="dependency"/> to the dependency collection.
        /// </summary>
        /// <param name="dependency">The <see cref="IDependency"/> to add to the dependency collection.</param>
        void AddDependency(IDependency dependency);

        /// <summary>
        /// Adds a new dependency with the specified <paramref name="dependencyType"/> to the dependency collection.
        /// </summary>
        /// <param name="dependencyType">The <see cref="Type"/> of dependency.</param>
        /// <returns>The added <see cref="IDependency"/>.</returns>
        IDependency AddDependency(Type dependencyType);

        /// <summary>
        /// Adds this dependency collection to the specified <paramref name="dependencies"/>.
        /// </summary>
        /// <param name="dependencies">The <see cref="IDependencyCollection"/> to add this collection to.</param>
        void AddToDependencyCollection(IDependencyCollection dependencies);

        /// <summary>
        /// Adds a transitive dependency to the dependency collection.
        /// </summary>
        /// <param name="transitiveDependencyType">The type of transitive dependency.</param>
        /// <remarks>
        /// Generally, this should be used when a dependency is implemented in an external library that doesn't provide an <see cref="IDependencyCollection"/>.<br/>
        /// These transitive dependencies will likely be directly registered with the dependency injection container.<br/>
        /// <br/>
        /// Additionally, a generic transitive dependency will be treated as an open generic type during scanning and validation, regardless of how it's registered.
        /// </remarks>
        void AddTransitiveDependency(Type transitiveDependencyType);

        /// <summary>
        /// Gets the dependency from the dependency collection.
        /// </summary>
        /// <param name="dependencyType">The <see cref="Type"/> of dependency.</param>
        /// <returns>The <see cref="IDependency"/>.</returns>
        IDependency GetDependency(Type dependencyType);

        #endregion Methods

    }

}
