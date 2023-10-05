using System;
using System.Collections.Generic;

namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// A dependency that has been registered with a <see cref="DependencyCollection"/>.
    /// </summary>
    public class Dependency
    {

        #region - - - - - - Fields - - - - - -

        private bool? m_AllowScannedImplementationTypes;
        private IDependencyBuilderBehaviour m_Behaviour;
        private DependencyLifetime m_Lifetime;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        internal Dependency(Type dependencyType)
            => this.DependencyType = dependencyType;

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Determines if scanned implementation types will be automatically added to the dependency.
        /// </summary>
        /// <remarks>
        /// Implementation types will be added through the <see cref="Behaviour"/> of the dependency,
        /// which may not result in the scanned implementation type being added.
        /// </remarks>
        public bool AllowScannedImplementationTypes
        {
            get => this.m_AllowScannedImplementationTypes ?? this.LinkedDependency?.AllowScannedImplementationTypes ?? false;
            set => this.m_AllowScannedImplementationTypes = value;
        }

        /// <summary>
        /// The configuration behaviour of the dependency.
        /// </summary>
        public IDependencyBuilderBehaviour Behaviour
        {
            get => this.m_Behaviour ?? this.LinkedDependency?.Behaviour ?? DefaultDependencyBehaviour.Instance();
            set => this.m_Behaviour = value;
        }

        /// <summary>
        /// A factory which produces an instance of the dependency.
        /// </summary>
        public Func<DependencyFactory, object> ImplementationFactory { get; set; }

        /// <summary>
        /// An instance of the dependency.
        /// </summary>
        public object ImplementationInstance { get; set; }

        /// <summary>
        /// A list of implementation types for the dependency.
        /// </summary>
        /// <remarks>
        /// An implementation type is any type that implements or inherits from the dependency.
        /// </remarks>
        public List<Type> ImplementationTypes { get; set; } = new List<Type>();

        /// <summary>
        /// The lifetime of the dependency.
        /// </summary>
        public DependencyLifetime Lifetime
        {
            get => this.m_Lifetime ?? this.LinkedDependency?.Lifetime;
            set => this.m_Lifetime = value;
        }

        internal Dependency LinkedDependency { get; set; }

        /// <summary>
        /// The type of dependency.
        /// </summary>
        public Type DependencyType { get; }

        #endregion Properties

    }

}
