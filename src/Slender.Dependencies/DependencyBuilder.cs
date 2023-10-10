using System;
using System.Collections.Generic;

namespace Slender.Dependencies
{

    /// <summary>
    /// A builder that is used to configure a <see cref="Dependencies.Dependency"/>.
    /// </summary>
    public class DependencyBuilder
    {

        #region - - - - - - Constructors - - - - - -

        internal DependencyBuilder(Type dependencyType)
            => this.Dependency = new Dependency(dependencyType ?? throw new ArgumentNullException(nameof(dependencyType)));

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        internal Action OnScanForImplementations { get; set; }

        internal Dependency Dependency { get; private set; }

        internal List<Type> ScannedImplementationTypes { get; } = new List<Type>();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        internal void AddScannedImplementationTypes(IEnumerable<Type> implementationTypes, bool existingImplementationsFirst)
        {
            if (this.Dependency.AllowScannedImplementationTypes)
                foreach (var _ImplementationType in implementationTypes)
                    _ = this.HasImplementationType(_ImplementationType);

            else if (existingImplementationsFirst)
                this.ScannedImplementationTypes.AddRange(implementationTypes);

            else
                this.ScannedImplementationTypes.InsertRange(0, implementationTypes);
        }

        /// <summary>
        /// Attempts to set the behaviour of the dependency.
        /// </summary>
        /// <param name="behaviour">The new behaviour for the dependency.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="behaviour"/> is null.</exception>
        /// <remarks>
        /// The behaviour of the dependency may be implemented to disallow the change.
        /// </remarks>
        public DependencyBuilder HasBehaviour(IDependencyBehaviour behaviour)
        {
            if (behaviour is null) throw new ArgumentNullException(nameof(behaviour));

            this.Dependency.Behaviour.UpdateBehaviour(this.Dependency, behaviour);

            return this;
        }

        /// <summary>
        /// Attempts to set the implementation factory of the dependency.
        /// </summary>
        /// <param name="implementationFactory">A factory which produces an instance of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationFactory"/> is null.</exception>
        /// <remarks>
        /// The behaviour of the dependency may be implemented to disallow the change.
        /// </remarks>
        public DependencyBuilder HasImplementationFactory(Func<DependencyFactory, object> implementationFactory)
        {
            if (implementationFactory is null) throw new ArgumentNullException(nameof(implementationFactory));

            this.Dependency.Behaviour.UpdateImplementationFactory(this.Dependency, implementationFactory);

            return this;
        }

        /// <summary>
        /// Attempts to set the implementation instance of the dependency.
        /// </summary>
        /// <param name="implementationInstance">An instance of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationInstance"/> is null.</exception>
        /// <exception cref="Exception">Thrown when the dependency lifetime doesn't allow implementation instances.</exception>
        /// <remarks>
        /// The behaviour of the dependency may be implemented to disallow the change.
        /// </remarks>
        public DependencyBuilder HasImplementationInstance(object implementationInstance)
        {
            if (implementationInstance is null)
                throw new ArgumentNullException(nameof(implementationInstance));

            if (!this.Dependency.Lifetime.AllowImplementationInstances)
                throw new Exception($"{this.Dependency.Lifetime} Lifetime does not allow implementation instances.");

            this.Dependency.Behaviour.UpdateImplementationInstance(this.Dependency, implementationInstance);

            return this;
        }

        /// <summary>
        /// Attempts to add <typeparamref name="TImplementation"/> as an implementation type to the dependency.
        /// </summary>
        /// <typeparam name="TImplementation">A type that implements or inherits from the dependency.</typeparam>
        /// <returns>Itself.</returns>
        /// <remarks>
        /// The behaviour of the dependency may be implemented to disallow the change.
        /// </remarks>
        public DependencyBuilder HasImplementationType<TImplementation>()
            => this.HasImplementationType(typeof(TImplementation));

        /// <summary>
        /// Attempts to add <paramref name="implementationType"/> as an implementation type to the dependency.
        /// </summary>
        /// <param name="implementationType">A type that implements or inherits from the dependency.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="implementationType"/> is null.</exception>
        /// <remarks>
        /// The behaviour of the dependency may be implemented to disallow the change.
        /// </remarks>
        public DependencyBuilder HasImplementationType(Type implementationType)
        {
            if (implementationType is null) throw new ArgumentNullException(nameof(implementationType));

            if (!this.Dependency.ImplementationTypes.Contains(implementationType))
                this.Dependency.Behaviour.AddImplementationType(this.Dependency, implementationType);

            return this;
        }

        /// <summary>
        /// Attempts to set the lifetime of the dependency.
        /// </summary>
        /// <param name="lifetime">The new lifetime for the dependency.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="lifetime"/> is null.</exception>
        /// <remarks>
        /// If the lifetime of the dependency doesn't support implementation instances then the implementation instance will be cleared.<br/>
        /// <br/>
        /// The behaviour of the dependency may be implemented to disallow the change.
        /// </remarks>
        public DependencyBuilder HasLifetime(DependencyLifetime lifetime)
        {
            if (lifetime is null) throw new ArgumentNullException(nameof(lifetime));

            this.Dependency.Behaviour.UpdateLifetime(this.Dependency, lifetime);

            // If the current Lifetime doesn't support Implementation Instances, clear the instance.
            if (!this.Dependency.Lifetime.AllowImplementationInstances && this.Dependency.ImplementationInstance != null)
                this.Dependency.ImplementationInstance = null;

            return this;
        }

        /// <summary>
        /// Replaces the existing dependency with a copy of the specified <paramref name="newDependency"/>.
        /// </summary>
        /// <param name="newDependency">The new details for the dependency.</param>
        /// <param name="oldDependency">The old details of the dependency.</param>
        /// <returns>Itself.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="newDependency"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="newDependency"/> has a different <see cref="Dependency.DependencyType"/>.</exception>
        /// <remarks>
        /// This is only intended to be used by implementations of <see cref="IDependencyBehaviour"/>.
        /// </remarks>
        public DependencyBuilder ReplaceDependency(Dependency newDependency, out Dependency oldDependency)
        {
            if (newDependency is null) throw new ArgumentNullException(nameof(newDependency));
            if (newDependency.DependencyType != this.Dependency.DependencyType) throw new ArgumentException("Cannot change the type of the dependency.");

            oldDependency = this.Dependency;

            // If both new and old dependencies were auto-registered, then retain the link from the new dependency.
            var _LinkedDependency = newDependency.LinkedDependency != null
                                        && oldDependency.LinkedDependency != null
                                            ? newDependency.LinkedDependency
                                            : null;

            this.Dependency = new Dependency(this.Dependency.DependencyType)
            {
                AllowScannedImplementationTypes = newDependency.AllowScannedImplementationTypes,
                Behaviour = newDependency.Behaviour,
                ImplementationFactory = newDependency.ImplementationFactory,
                ImplementationInstance = newDependency.ImplementationInstance,
                ImplementationTypes = newDependency.ImplementationTypes,
                Lifetime = newDependency.Lifetime,
                LinkedDependency = _LinkedDependency
            };

            return this;
        }

        /// <summary>
        /// Scans for implementation types and adds them to the dependency.
        /// </summary>
        /// <remarks>
        /// Allowing scanning of implementation types is handled through the behaviour of the dependency,
        /// which may not result in scanning being allowed. If scanning is not allowed then scanning will not occur.<br/>
        /// <br/>
        /// For more information on allowing scanning, see <see cref="IDependencyBehaviour.AllowScannedImplementationTypes(Dependency)"/><br/>
        /// For more information on scanned types, see <see cref="DependencyCollection.AddAssemblyScan(AssemblyScanner.IAssemblyScan)"/>.
        /// </remarks>
        public DependencyBuilder ScanForImplementations()
        {
            this.Dependency.Behaviour.AllowScannedImplementationTypes(this.Dependency);

            if (this.Dependency.AllowScannedImplementationTypes)
            {
                foreach (var _ImplementationType in this.ScannedImplementationTypes)
                    _ = this.HasImplementationType(_ImplementationType);

                this.ScannedImplementationTypes.Clear();

                this.OnScanForImplementations?.Invoke();
            }

            return this;
        }

        #endregion Methods

    }

}
