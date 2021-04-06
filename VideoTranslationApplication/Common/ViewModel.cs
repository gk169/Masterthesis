using System.ComponentModel;

namespace VideoTranslationTool
{
    /// <summary>
    /// Public class <c>ViewModel</c> is parent class for all viewmodels
    /// </summary>
    public abstract class ViewModel : INotifyPropertyChanged
    {
        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion Events

        #region Methods
        /// <summary>
        /// Public method <c>OnPropertyChanged</c> informs view that a viewmodel property has changed
        /// </summary>
        /// <param name="PropertyName">
        /// Name of the property that has changed
        /// </param>
        public void OnPropertyChanged(string PropertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(PropertyName));
        }
        #endregion Methods
    }
}
