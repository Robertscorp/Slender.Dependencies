using System;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// Used to determine the configuration behaviours when updating a dependency.
    /// </summary>
    public interface IDependencyBuilderBehaviour
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds an implementation type to the specified dependency.
        /// </summary>
        /// <param name="dependency">The dependency to update.</param>
        /// <param name="type">A type which implements or inherits from the dependency.</param>
        void AddImplementationType(Dependency dependency, Type type);

        /// <summary>
        /// Changes the specified dependency to allow scanned implementation types to be automatically added.
        /// </summary>
        /// <param name="dependency">The dependency to update.</param>
        void AllowScannedImplementationTypes(Dependency dependency);

        /// <summary>
        /// Merges two dependencies together using the specified builder.
        /// </summary>
        /// <param name="builder">A builder to update the dependency.</param>
        /// <param name="dependency">The dependency being merged in.</param>
        /// <remarks>
        /// The dependencies being merged will be of the same type, but configured from two different sources.
        /// </remarks>
        void MergeDependencies(DependencyBuilder builder, Dependency dependency);

        /// <summary>
        /// Changes the behaviour of the specified dependency.
        /// </summary>
        /// <param name="dependency">The dependency to update.</param>
        /// <param name="behaviour">The new behaviour for the dependency.</param>
        void UpdateBehaviour(Dependency dependency, IDependencyBuilderBehaviour behaviour);

        /// <summary>
        /// Sets the implementation factory of the dependency.
        /// </summary>
        /// <param name="dependency">The dependency to update.</param>
        /// <param name="implementationFactory">A factory which produces an instance of the dependency.</param>
        void UpdateImplementationFactory(Dependency dependency, Func<DependencyFactory, object> implementationFactory);

        /// <summary>
        /// Sets the implementation instance of the dependency.
        /// </summary>
        /// <param name="dependency">The dependency to update.</param>
        /// <param name="implementationInstance">An instance of the dependency.</param>
        void UpdateImplementationInstance(Dependency dependency, object implementationInstance);

        /// <summary>
        /// Changes the lifetime of the dependency.
        /// </summary>
        /// <param name="dependency">The dependency to update.</param>
        /// <param name="lifetime">The new lifetime for the dependency.</param>
        void UpdateLifetime(Dependency dependency, DependencyLifetime lifetime);

        #endregion Methods

    }

}
