using System;

namespace Slender.Dependencies
{

    public partial class DependencyCollection
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as a scoped dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddScoped<TImplementation>()
            => this.AddDependency(typeof(TImplementation), DependencyLifetime.Scoped(), d => d.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TDependency"/> as a scoped dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddScoped<TDependency, TImplementation>() where TImplementation : TDependency
            => this.AddDependency(typeof(TDependency), DependencyLifetime.Scoped(), d => d.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TDependency"/> as a scoped dependency with <paramref name="implementationFactory"/> as the mechanism of providing an implementation instance.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="implementationFactory">A factory which produces an instance of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddScoped<TDependency>(Func<DependencyFactory, TDependency> implementationFactory) where TDependency : class
            => this.AddDependency(typeof(TDependency), DependencyLifetime.Scoped(), d => d.WithImplementationFactory(implementationFactory));

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a scoped dependency.
        /// </summary>
        /// <param name="type">The type of dependency.</param>
        /// <param name="configurationAction">An action to configure the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddScoped(Type type, Action<DependencyBuilder> configurationAction = null)
            => this.AddDependency(type, DependencyLifetime.Scoped(), configurationAction);

        #endregion Methods

    }

}
