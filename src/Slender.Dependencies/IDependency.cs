using System;

namespace Slender.Dependencies
{

    /// <summary>
    /// Contains all of the information required to register a dependency with a dependency injection container.
    /// </summary>
    public interface IDependency
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds a decorator to the dependency.
        /// </summary>
        /// <param name="decoratorType">The <see cref="Type"/> of decorator to add to the dependency.</param>
        void AddDecorator(Type decoratorType);

        /// <summary>
        /// Adds the implementation to the dependency.
        /// </summary>
        /// <param name="implementation">The implementation to add to the dependency.</param>
        void AddImplementation(object implementation);

        /// <summary>
        /// Gets the type of the dependency.
        /// </summary>
        /// <returns>The type of the dependency.</returns>
        Type GetDependencyType();

        /// <summary>
        /// Sets the lifetime of the dependency.
        /// </summary>
        /// <param name="lifetime">The <see cref="DependencyLifetime"/> to assign to the dependency.</param>
        void SetLifetime(DependencyLifetime lifetime);

        #endregion Methods

    }

}
