namespace VideoTranslationTool
{
    public interface IModuleViewModel
    {
        /// <summary>
        /// Public method <c>CanNext</c> indicates if the trancription step is completed
        /// </summary>
        /// <returns>
        /// True if complete, otherwise false
        /// </returns>
        public abstract bool CanNext();
    }
}
