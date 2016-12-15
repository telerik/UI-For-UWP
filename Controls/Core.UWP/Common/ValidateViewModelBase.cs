using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Telerik.Core
{
    /// <summary>
    /// Provides basic implementation of the <see cref="IAsyncDataErrorInfo"/> interface.
    /// </summary>
    public abstract class ValidateViewModelBase : ViewModelBase, IAsyncDataErrorInfo
    {
        private static readonly Task Completed = Task.Delay(0);

        private Dictionary<string, HashSet<object>> errors = new Dictionary<string, HashSet<object>>();

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <inheritdoc />
        [SkipAutoGenerate]
        public bool HasErrors
        {
            get
            {
                return this.errors.Count > 0;
            }
        }

        /// <inheritdoc />
        public IEnumerable GetErrors(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                return this.errors.SelectMany(c => c.Value);
            }

            HashSet<object> propertyErrors;

            this.errors.TryGetValue(propertyName, out propertyErrors);

            return propertyErrors ?? Enumerable.Empty<object>();
        }

        /// <inheritdoc />
        public async Task ValidateAsync(string propertyName)
        {
            await this.ValidateAsyncOverride(propertyName);
        }

        /// <summary>
        /// Removes the errors for the specified property.
        /// </summary>
        /// <param name="propertyName">The name of the property to remove validation errors
        /// for; or null or <see cref="F:System.String.Empty"/>, to clear entity-level
        /// errors.</param>
        protected virtual void RemoveErrors(string propertyName = null)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                if (this.errors.Count > 0)
                {
                    this.errors.Clear();
                    this.OnErrorsChanged(propertyName);
                }
            }
            else
            {
                if (this.errors.Remove(propertyName))
                {
                    this.OnErrorsChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Adds error message to the specified property.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="errorMessage">The error message.</param>
        protected virtual void AddError(string propertyName, object errorMessage)
        {
            HashSet<object> propertyErrors;

            if (!this.errors.TryGetValue(propertyName, out propertyErrors))
            {
                propertyErrors = new HashSet<object>();
                this.errors.Add(propertyName, propertyErrors);

                propertyErrors.Add(errorMessage);

                this.OnErrorsChanged(propertyName);
            }
            else
            {
                if (!propertyErrors.Contains(errorMessage))
                {
                    propertyErrors.Add(errorMessage);
                    this.OnErrorsChanged(propertyName);
                }
            }
        }

        /// <summary>
        /// Called by the <see cref="M:ValidateAsync"/> method. Allows inheritors to provide custom validation logic.
        /// </summary>
        /// <param name="propertyName">The name of the property that needs validation.</param>
        protected async virtual Task ValidateAsyncOverride(string propertyName)
        {
            await Completed;
        }

        /// <summary>
        /// Raises the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnErrorsChanged(string propertyName)
        {
            if (this.ErrorsChanged != null)
            {
                this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
            }
        }
    }
}
