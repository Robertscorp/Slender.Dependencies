using Slender.Dependencies.Legacy;
using System;

namespace Slender.Dependencies
{

    public static partial class IDependencyCollectionExtensions
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Adds <typeparamref name="TImplementation"/> as a scoped dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <param name="dependencies">The <see cref="IDependencyCollection"/> to update. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="AddDependency{TDependencyCollection}(TDependencyCollection, Type, Action{IDependency})"/>.</remarks>
        public static IDependencyCollection AddScoped<TImplementation>(this IDependencyCollection dependencies)
            => AddDependency(dependencies, typeof(TImplementation), dependency => dependency.HasLifetime(DependencyLifetime.Scoped()).HasImplementationType<TImplementation>());

        /// <summary>
        /// Adds <typeparamref name="TDependency"/> as a scoped dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <param name="dependencies">The <see cref="IDependencyCollection"/> to update. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="AddDependency{TDependencyCollection}(TDependencyCollection, Type, Action{IDependency})"/>.</remarks>
        public static IDependencyCollection AddScoped<TDependency, TImplementation>(this IDependencyCollection dependencies) where TImplementation : TDependency
            => AddDependency(dependencies, typeof(TDependency), dependency => dependency.HasLifetime(DependencyLifetime.Scoped()).HasImplementationType<TImplementation>());

        /// <summary>
        /// Adds <typeparamref name="TDependency"/> as a scoped dependency with <paramref name="implementationFactory"/> as the mechanism of providing an implementation instance.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="dependencies">The <see cref="IDependencyCollection"/> to update. Cannot be null.</param>
        /// <param name="implementationFactory">A factory which produces an instance of the dependency. Cannot be null.</param>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="AddDependency{TDependencyCollection}(TDependencyCollection, Type, Action{IDependency})"/>.</remarks>
        public static IDependencyCollection AddScoped<TDependency>(this IDependencyCollection dependencies, Func<DependencyFactory, TDependency> implementationFactory) where TDependency : class
            => AddDependency(dependencies, typeof(TDependency), dependency => dependency.HasLifetime(DependencyLifetime.Scoped()).HasImplementationFactory(implementationFactory));

        /// <summary>
        /// Adds <paramref name="dependencyType"/> as a scoped dependency.
        /// </summary>
        /// <param name="dependencies">The <see cref="IDependencyCollection"/> to update. Cannot be null.</param>
        /// <param name="dependencyType">The type of dependency. Cannot be null.</param>
        /// <param name="configurationAction">An action to configure the dependency. Can be null.</param>
        /// <returns>The specified <paramref name="dependencies"/>.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="AddDependency{TDependencyCollection}(TDependencyCollection, Type, Action{IDependency})"/>.</remarks>
        public static IDependencyCollection AddScoped(this IDependencyCollection dependencies, Type dependencyType, Action<IDependency> configurationAction = null)
            => AddDependency(dependencies, dependencyType, dependency =>
            {
                dependency.SetLifetime(DependencyLifetime.Scoped());
                configurationAction?.Invoke(dependency);
            });

        #endregion Methods

    }

}
