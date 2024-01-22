using System;
using System.Collections.Generic;
using System.Linq;

namespace Slender.Dependencies.Internals
{

    internal class ReadOnlyDependencyCollection : IDependencyCollection
    {

        #region - - - - - - Fields - - - - - -

        private readonly bool m_IsReadOnly;

        #endregion Fields

        #region - - - - - - Constructors - - - - - -

        public ReadOnlyDependencyCollection(IDependencyCollection dependencies)
        {
            dependencies.AddToDependencyCollection(this);
            this.m_IsReadOnly = true;
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        public List<ReadOnlyDependency> Dependencies { get; } = new List<ReadOnlyDependency>();

        public List<Type> TransitiveDependencies { get; } = new List<Type>();

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        void IDependencyCollection.AddDependency(IDependency dependency)
        {
            if (!this.m_IsReadOnly)
                this.Dependencies.Add(new ReadOnlyDependency(dependency));
        }

        IDependency IDependencyCollection.AddDependency(Type dependencyType)
            => throw new NotImplementedException();

        void IDependencyCollection.AddToDependencyCollection(IDependencyCollection dependencies)
            => throw new NotImplementedException();

        void IDependencyCollection.AddTransitiveDependency(Type transitiveDependencyType)
        {
            if (!this.m_IsReadOnly)
                this.TransitiveDependencies.Add(transitiveDependencyType);
        }

        IDependency IDependencyCollection.GetDependency(Type dependencyType)
            => this.Dependencies.FirstOrDefault(d => d.DependencyType == dependencyType);

        #endregion Methods

    }

}
