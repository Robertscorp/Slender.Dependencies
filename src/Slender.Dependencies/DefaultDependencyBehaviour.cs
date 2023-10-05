using System;
using System.Linq;

namespace Slender.Dependencies
{

    /// <summary>
    /// The default dependency behaviour.
    /// </summary>
    public class DefaultDependencyBehaviour : IDependencyBehaviour
    {

        #region - - - - - - Fields - - - - - -

        private readonly bool m_AllowBehaviourToChange;
        private readonly bool m_AllowMultipleImplementationTypes;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        private DefaultDependencyBehaviour(bool allowBehaviourToChange, bool allowMultipleImplementationTypes)
        {
            this.m_AllowBehaviourToChange = allowBehaviourToChange;
            this.m_AllowMultipleImplementationTypes = allowMultipleImplementationTypes;
        }

        #endregion Constructors

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// Gets a new instance of the default dependency behaviour.
        /// </summary>
        /// <param name="allowBehaviourToChange">Determines if the behaviour of a dependency can be changed. Default = true.</param>
        /// <param name="allowMultipleImplementationTypes">Determines if a dependency can have multiple implementation types. Default = false.</param>
        /// <returns>An instance of the default dependency behaviour.</returns>
        public static DefaultDependencyBehaviour Instance(bool allowBehaviourToChange = true, bool allowMultipleImplementationTypes = false)
            => new DefaultDependencyBehaviour(allowBehaviourToChange, allowMultipleImplementationTypes);

        private bool CanRegisterImplementation(Dependency dependency, bool isImplementationType)
            => dependency.ImplementationFactory == null &&
                dependency.ImplementationInstance == null &&
                (!dependency.ImplementationTypes.Any() || (this.m_AllowMultipleImplementationTypes && isImplementationType));

        void IDependencyBehaviour.AddImplementationType(Dependency dependency, Type type)
        {
            if (this.CanRegisterImplementation(dependency, true)) dependency.ImplementationTypes.Add(type);
        }

        void IDependencyBehaviour.AllowScannedImplementationTypes(Dependency dependency)
            => dependency.AllowScannedImplementationTypes = true;

        void IDependencyBehaviour.MergeDependencies(DependencyBuilder builder, Dependency dependency)
        {
            // Force the builder to take on the incoming dependency, so any custom behaviours and provided implementations will carry through the merge.
            _ = builder.WithDependency(dependency, out dependency);

            // Try and restore the original behaviour, if the incoming behaviour allows that.
            _ = builder.WithBehaviour(dependency.Behaviour);

            // Update the Lifetime before the ImplementationInstances to ensure we handle instances properly.
            _ = builder.WithLifetime(dependency.Lifetime);

            if (dependency.AllowScannedImplementationTypes)
                _ = builder.ScanForImplementations();

            if (dependency.ImplementationFactory != null)
                _ = builder.WithImplementationFactory(dependency.ImplementationFactory);

            // Check if the lifetime allows instances, otherwise the builder will throw an exception.
            if (dependency.ImplementationInstance != null && builder.Dependency.Lifetime.AllowImplementationInstances)
                _ = builder.WithImplementationInstance(dependency.ImplementationInstance);

            foreach (var _ImplementationType in dependency.ImplementationTypes)
                _ = builder.AddImplementationType(_ImplementationType);
        }

        void IDependencyBehaviour.UpdateBehaviour(Dependency dependency, IDependencyBehaviour behaviour)
        {
            if (this.m_AllowBehaviourToChange) dependency.Behaviour = behaviour;
        }

        void IDependencyBehaviour.UpdateImplementationFactory(Dependency dependency, Func<DependencyFactory, object> implementationFactory)
        {
            if (this.CanRegisterImplementation(dependency, false)) dependency.ImplementationFactory = implementationFactory;
        }

        void IDependencyBehaviour.UpdateImplementationInstance(Dependency dependency, object implementationInstance)
        {
            if (this.CanRegisterImplementation(dependency, false)) dependency.ImplementationInstance = implementationInstance;
        }

        void IDependencyBehaviour.UpdateLifetime(Dependency dependency, DependencyLifetime lifetime) { }

        #endregion Methods

    }

}
