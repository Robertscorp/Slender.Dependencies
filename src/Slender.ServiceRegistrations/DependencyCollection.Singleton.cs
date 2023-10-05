using System;

namespace Slender.ServiceRegistrations
{

    public partial class DependencyCollection
    {

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as a singleton dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddSingleton<TImplementation>()
            => this.AddDependency(typeof(TImplementation), DependencyLifetime.Singleton(), d => d.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TImplementation"/> as a singleton dependency with <paramref name="implementationInstance"/> as the implementation instance.
        /// </summary>
        /// <typeparam name="TImplementation">The type of singleton instance.</typeparam>
        /// <param name="implementationInstance">An instance of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddSingleton<TImplementation>(TImplementation implementationInstance)
            => this.AddDependency(typeof(TImplementation), DependencyLifetime.Singleton(), d => d.WithImplementationInstance(implementationInstance));

        /// <summary>
        /// Registers <typeparamref name="TDependency"/> as a singleton dependency with <typeparamref name="TImplementation"/> as the implementation type.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <typeparam name="TImplementation">The type of implementation.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddSingleton<TDependency, TImplementation>() where TImplementation : TDependency
            => this.AddDependency(typeof(TDependency), DependencyLifetime.Singleton(), d => d.AddImplementationType<TImplementation>());

        /// <summary>
        /// Registers <typeparamref name="TDependency"/> as a singleton dependency with <paramref name="implementationFactory"/> as the mechanism of providing an implementation instance.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="implementationFactory">A factory which produces an instance of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddSingleton<TDependency>(Func<DependencyFactory, TDependency> implementationFactory) where TDependency : class
            => this.AddDependency(typeof(TDependency), DependencyLifetime.Singleton(), d => d.WithImplementationFactory(implementationFactory));

        /// <summary>
        /// Registers <typeparamref name="TDependency"/> as a singleton dependency with <paramref name="implementationInstance"/> as the implementation instance.
        /// </summary>
        /// <typeparam name="TDependency">The type of dependency.</typeparam>
        /// <param name="implementationInstance">An instance of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddSingleton<TDependency>(object implementationInstance)
            => this.AddDependency(typeof(TDependency), DependencyLifetime.Singleton(), d => d.WithImplementationInstance(implementationInstance));

        /// <summary>
        /// Registers the specified <paramref name="type"/> as a singleton dependency.
        /// </summary>
        /// <param name="type">The type of dependency.</param>
        /// <param name="configurationAction">An action to configure the dependency.</param>
        /// <returns>Itself.</returns>
        /// <remarks>For more information on adding dependencies, see <see cref="DependencyCollection.AddDependency(Type, DependencyLifetime, Action{DependencyBuilder})"/>.</remarks>
        public DependencyCollection AddSingleton(Type type, Action<DependencyBuilder> configurationAction)
            => this.AddDependency(type, DependencyLifetime.Singleton(), configurationAction);

        #endregion Methods

    }

}
