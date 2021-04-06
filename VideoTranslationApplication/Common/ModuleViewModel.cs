using System.Windows.Input;
using VideoTranslationTool.Commands;

namespace VideoTranslationTool
{
    /// <summary>
    /// Public class <c>ModuleViewModel</c> as parent class for all module viewmodels
    /// </summary>
    public abstract class ModuleViewModel : ViewModel
    {
        #region Members
        private ICommand _exportCommand;
        #endregion Members

        #region Properties
        /// <summary>
        /// Public command <c>ExportCommand</c> calls Export method if executable
        /// </summary>
        public ICommand ExportCommand
        {
            get
            {
                if (_exportCommand is null) _exportCommand = new RelayCommand(param => this.Export(), param => this.CanExport());
                return _exportCommand;
            }
        }
        #endregion Properties

        #region Methods
        /// <summary>
        /// Private method <c>CanExport</c> indicates if Export method is executable
        /// </summary>
        /// <returns>
        /// True if executable, otherwise false
        /// </returns>
        protected abstract bool CanExport();

        /// <summary>
        /// Public method <c>CanNext</c> indicates if the trancription step is completed
        /// </summary>
        /// <returns>
        /// True if complete, otherwise false
        /// </returns>
        public abstract bool CanNext();

        /// <summary>
        /// Private method <c>Export</c> exports the result to file
        /// </summary>
        protected abstract void Export();

        /// <summary>
        /// Public method <c>GetResultForNextStep</c> returns results of current step
        /// </summary>
        /// <returns>
        /// Results of the current step as string
        /// </returns>
        public abstract string GetResultforNextStep();

        /// <summary>
        /// Public method <c>SetResultOfPreviousStep</c> sets the input data (result of previous step) of the current step
        /// </summary>
        /// <param name="resultsOfPrevious">
        /// Results of the previous step as string
        /// </param>
        public abstract void SetResultOfPreviousStep(string resultsOfPrevious);
        #endregion Methods
    }
}
