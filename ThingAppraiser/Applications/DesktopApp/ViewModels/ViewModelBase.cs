using System.ComponentModel;
using System.Runtime.CompilerServices;
using ThingAppraiser.Logging;

namespace ThingAppraiser.DesktopApp.ViewModels
{
    /// <summary>
    /// Base class for all view model classes.
    /// </summary>
    public abstract class ViewModelBase : INotifyPropertyChanged
    {
        /// <summary>
        /// Logger instance for current class.
        /// </summary>
        private static readonly LoggerAbstraction _logger =
            LoggerAbstraction.CreateLoggerInstanceFor<ViewModelBase>();


        /// <summary>
        /// Creates instance with default values.
        /// </summary>
        public ViewModelBase()
        {
        }

        /// <summary>
        /// Checks if a property already matches a desired value. Sets the property and notifies
        /// listeners only when necessary.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="storage">Reference to a property with both getter and setter.</param>
        /// <param name="value">Desired value for the property.</param>
        /// <param name="propertyName">
        /// Name of the property used to notify listeners. This value is optional and can be
        /// provided automatically when invoked from compilers that support
        /// <see cref="CallerMemberNameAttribute" />.
        /// </param>
        /// <returns>
        /// <c>true</c> if the value was changed, <c>false</c> if the existing value matched the
        /// desired value.
        /// </returns>
        protected virtual bool SetProperty<T>(ref T storage, T value,
            [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;

            storage = value;

            _logger.Debug($"{GetType().Name}.{propertyName} = {storage}");

            OnPropertyChanged(propertyName);
            return true;
        }

        #region INotifyPropertyChanged Implementation

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notifies listeners that a property value has changed.
        /// </summary>
        /// <param name="propertyName">
        /// Name of the property used to notify listeners. This value is optional and can be
        /// provided automatically when invoked from compilers that support
        /// <see cref="CallerMemberNameAttribute" />.
        /// </param>
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
