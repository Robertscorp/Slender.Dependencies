namespace Slender.ServiceRegistrations
{

    /// <summary>
    /// An enumeration of possible service registration lifetimes.
    /// </summary>
    public class RegistrationLifetime
    {

        #region - - - - - - Constructors - - - - - -

        private RegistrationLifetime(string name, bool allowImplementationInstances)
        {
            this.Name = name;
            this.AllowImplementationInstances = allowImplementationInstances;
        }

        #endregion Constructors

        #region - - - - - - Properties - - - - - -

        /// <summary>
        /// Used to determine if service registrations with this lifetime can have implementation instances registered.
        /// </summary>
        public bool AllowImplementationInstances { get; }

        /// <summary>
        /// The name of the registration lifetime.
        /// </summary>
        public string Name { get; }

        #endregion Properties

        #region - - - - - - Methods - - - - - -

        /// <summary>
        /// The scoped lifetime. Services with this lifetime will use the same instance each time the service is requested within its scope.
        /// </summary>
        /// <returns>The scoped lifetime.</returns>
        public static RegistrationLifetime Scoped() => new RegistrationLifetime("Scoped", false);

        /// <summary>
        /// The singleton lifetime. Services with this lifetime will use the same instance each time the service is requested.
        /// </summary>
        /// <returns>The singleton lifetime.</returns>
        public static RegistrationLifetime Singleton() => new RegistrationLifetime("Singleton", true);

        /// <summary>
        /// The transient lifetime. Services with this lifetime will use a new instance each time the service is requested.
        /// </summary>
        /// <returns>The transient lifetime.</returns>
        public static RegistrationLifetime Transient() => new RegistrationLifetime("Transient", false);

        #endregion Methods

    }

}
