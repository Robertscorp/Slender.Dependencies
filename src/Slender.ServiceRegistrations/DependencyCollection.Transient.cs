using System;

namespace Slender.ServiceRegistrations
{

    public partial class DependencyCollection
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as a transient dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddTransient<TImplementation>()
            => this.AddDependency(typeof(TImplementation), DependencyLifetime.Transient(), d => d.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TDependency"/> as a transient dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddTransient<TDependency, TImplementation>() where TImplementation : TDependency
            => this.AddDependency(typeof(TDependency), DependencyLifetime.Transient(), d => d.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TDependency"/> as a transient dependency with <paramref name="implementationFactory"/> as the mechanism of providing an implementation instance.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="implementationFactory">A factory which produces an instance of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddTransient<TDependency>(Func<DependencyFactory, TDependency> implementationFactory) where TDependency : class
            => this.AddDependency(typeof(TDependency), DependencyLifetime.Transient(), d => d.WithImplementationFactory(implementationFactory));

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a transient dependency.
        /// </summary>
        /// <param name="type">The type of dependency.</param>
        /// <param name="configurationAction">An action to configure the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddTransient(Type type, Action<DependencyBuilder> configurationAction = null)
            => this.AddDependency(type, DependencyLifetime.Transient(), configurationAction);

        #endregion Methods

    }

}
