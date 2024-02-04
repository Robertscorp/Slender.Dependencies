using System;
using System.Collections.Generic;

namespace Slender.Dependencies
{

    /// <summary>
    /// Represents a dependency that has been registered with an <see cref="IDependencyCollection"/>.
    /// </summary>
    public class Dependency : IDependency
    {

        #region - - - - - - Fields - - - - - -

        private readonly bool m_AutoLockLifetime;
        private readonly List<Type> m_Decorators = new List<Type>();
        private readonly Type m_DependencyType;
        private readonly List<object> m_Implementations = new List<object>();

        private DependencyLifetime m_Lifetime;
        private bool m_LockDecorators;
        private bool m_LockImplementations;
        private bool m_LockLifetime;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        /// <summary>
        /// Initialises a new instance of the <see cref="Dependency"/> class.
        /// </summary>
        /// <param name="dependencyType">The <see cref="Type"/> of dependency. Cannot be <see langword="null"/>.</param>
        /// <param name="autoLockLifetime">When enabled, prevents the lifetime of the dependency from changing once it has been set.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="dependencyType"/> is <see langword="null"/>.</exception>
        public Dependency(Type dependencyType, bool autoLockLifetime)
        {
            this.m_AutoLockLifetime = autoLockLifetime;
            this.m_DependencyType = dependencyType ?? throw new ArgumentNullException(nameof(dependencyType));
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <inheritdoc/>
        public void AddDecorator(Type decoratorType)
        {
            if (decoratorType == null) throw new ArgumentNullException(nameof(decoratorType));

            if (!this.m_LockDecorators && !this.m_Decorators.Contains(decoratorType))
                this.m_Decorators.Add(decoratorType);
        }

        /// <inheritdoc/>
        public void AddImplementation(object implementation)
        {
            if (implementation == null) throw new ArgumentNullException(nameof(implementation));

            if (!this.m_LockImplementations && !this.m_Implementations.Contains(implementation))
                this.m_Implementations.Add(implementation);
        }

        /// <inheritdoc/>
        public void AddToDependency(IDependency dependency)
        {
            if (dependency == null) throw new ArgumentNullException(nameof(dependency));

            dependency.SetLifetime(this.m_Lifetime);

            this.m_Decorators.ForEach(d => dependency.AddDecorator(d));
            this.m_Implementations.ForEach(i => dependency.AddImplementation(i));
        }

        /// <inheritdoc/>
        public Type GetDependencyType()
            => this.m_DependencyType;

        /// <summary>
        /// Locks the decorators, preventing new ones from being added.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>This operation cannot be undone.</remarks>
        public Dependency LockDecorators()
        {
            this.m_LockDecorators = true;
            return this;
        }

        /// <summary>
        /// Locks the dependency, preventing any changes.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>This operation cannot be undone.</remarks>
        public Dependency LockDependency()
            => this.LockDecorators().LockImplementations().LockLifetime();

        /// <summary>
        /// Locks the implementations, preventing new ones from being added.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>This operation cannot be undone.</remarks>
        public Dependency LockImplementations()
        {
            this.m_LockImplementations = true;
            return this;
        }

        /// <summary>
        /// Locks the lifetime, preventing it from changing.
        /// </summary>
        /// <returns>Itself.</returns>
        /// <remarks>This operation cannot be undone.</remarks>
        public Dependency LockLifetime()
        {
            this.m_LockLifetime = true;
            return this;
        }

        /// <inheritdoc/>
        public void SetLifetime(DependencyLifetime lifetime)
        {
            if (!this.m_LockLifetime)
                this.m_Lifetime = this.m_Lifetime ?? lifetime ?? throw new ArgumentNullException(nameof(lifetime));

            this.m_LockLifetime = this.m_LockLifetime || this.m_AutoLockLifetime;
        }

        #endregion Methods

    }

}
