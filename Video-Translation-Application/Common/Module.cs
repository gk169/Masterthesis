namespace VideoTranslationTool
{
    /// <summary>
    /// Public abstract class <c>Module</c> as parent class for all modules
    /// </summary>
    public abstract class Module
    {
        #region Properties
        /// <summary>
        /// Public property <c>Name</c> to get module name as string
        /// </summary>
        public string Name { get; }
        #endregion Properties

        #region Constructors
        /// <summary>
        /// Constructor for class <c>Module</c>
        /// </summary>
        /// <param name="name">
        /// Module name as string
        /// </param>
        protected Module(string name) => Name = name;
        #endregion Constructors
    }
}
