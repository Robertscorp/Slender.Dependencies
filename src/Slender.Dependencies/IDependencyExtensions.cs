using Slender.Dependencies.Legacy;
using System;

namespace Slender.Dependencies
{

    /// <summary>
    /// Contains <see cref="IDependency"/> extension methods.
    /// </summary>
    public static class IDependencyExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Attempts to add <paramref name="implementationFactory"/> to the dependency.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="dependency">The dependency to update. Cannot be null.</param>
        /// <param name="implementationFactory">A factory which produces an instance of the dependency. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependency"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependency"/> or <paramref name="implementationFactory"/> is null.</exception>
        /// <remarks>
        /// The dependency may be implemented to disallow the change.
        /// </remarks>
        public static TDependency HasImplementationFactory<TDependency>(this TDependency dependency, Func<DependencyFactory, object> implementationFactory) where TDependency : IDependency
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            if (implementationFactory == null) throw new ArgumentNullException(nameof(implementationFactory));

            dependency.AddImplementation(implementationFactory);

            return dependency;
        }

        /// <summary>
        /// Attempts to add <paramref name="implementationInstance"/> to the dependency.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="dependency">The dependency to update. Cannot be null.</param>
        /// <param name="implementationInstance">An instance of the dependency. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependency"/>.</returns>
        /// <exception cref="ArgumentException">Throw when <paramref name="implementationInstance"/> does not implement or inherit from the <see cref="IDependency.GetDependencyType()"/> of the specified <paramref name="dependency"/>.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependency"/> or <paramref name="implementationInstance"/> is null.</exception>
        /// <remarks>
        /// The dependency may be implemented to disallow the change.
        /// </remarks>
        public static TDependency HasImplementationInstance<TDependency>(this TDependency dependency, object implementationInstance) where TDependency : IDependency
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            if (implementationInstance == null) throw new ArgumentNullException(nameof(implementationInstance));

            var _DependencyType = dependency.GetDependencyType();

            if (!_DependencyType.IsAssignableFrom(implementationInstance.GetType()))
                throw new ArgumentException($"{nameof(implementationInstance)} does not implement or inherit from '{_DependencyType.Name}'.");

            dependency.AddImplementation(implementationInstance);

            return dependency;
        }

        /// <summary>
        /// Attempts to add <typeparamref name="TImplementation"/> as an implementation type to the dependency.
        /// </summary>
        /// <typeparam name="TImplementation">A type that implements or inherits from the dependency.</typeparam>
        /// <param name="dependency">The dependency to update. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependency"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependency"/> is null.</exception>
        /// <remarks>
        /// The dependency may be implemented to disallow the change.
        /// </remarks>
        public static IDependency HasImplementationType<TImplementation>(this IDependency dependency)
            => dependency.HasImplementationType(typeof(TImplementation));

        /// <summary>
        /// Attempts to add <paramref name="implementationType"/> as an implementation type to the dependency.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="dependency">The dependency to update. Cannot be null.</param>
        /// <param name="implementationType">A type that implements or inherits from the dependency. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependency"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependency"/> or <paramref name="implementationType"/> is null.</exception>
        /// <remarks>
        /// The dependency may be implemented to disallow the change.
        /// </remarks>
        public static TDependency HasImplementationType<TDependency>(this TDependency dependency, Type implementationType) where TDependency : IDependency
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            if (implementationType is null) throw new ArgumentNullException(nameof(implementationType));

            var _DependencyType = dependency.GetDependencyType();

            if (!_DependencyType.IsAssignableFrom(implementationType))
                throw new ArgumentException($"{nameof(implementationType)} does not implement or inherit from '{_DependencyType.Name}'.");

            dependency.AddImplementation(implementationType);

            return dependency;
        }

        /// <summary>
        /// Attempts to set the lifetime of the dependency.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="dependency">The dependency to update. Cannot be null.</param>
        /// <param name="lifetime">The lifetime for the dependency. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependency"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependency"/> or <paramref name="lifetime"/> is null.</exception>
        /// <remarks>The behaviour of the dependency may be implemented to disallow the change.</remarks>
        public static TDependency HasLifetime<TDependency>(this TDependency dependency, DependencyLifetime lifetime) where TDependency : IDependency
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            dependency.SetLifetime(lifetime);

            return dependency;
        }

        #endregion Methods

    }

}
