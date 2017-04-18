using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Data.Core;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.DataForm
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Validation : ExamplePageBase
    {
        public Validation()
        {
            this.InitializeComponent();

            this.DataContext = new Item() { Name = "Ivaylo", Age = 17 };
        }

        public class Item : ViewModelBase, ISupportEntityValidation
        {
            Dictionary<string, List<string>> errors = new Dictionary<string, List<string>>();

            private string name;
            public string Name
            {
                get { return this.name; }
                set { this.name = value; OnPropertyChanged("Name"); }
            }

            private double age;

            public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

            public double Age
            {
                get { return this.age; }
                set { this.age = value; OnPropertyChanged("Age"); }
            }

            public bool HasErrors
            {
                get
                {
                    return this.errors.Count > 0;
                }
            }

            public Task ValidatePropertyAsync(Entity entity, string propertyName)
            {
                if (propertyName.Equals("Age"))
                {
                    var property = entity.GetEntityProperty(propertyName);
                    double value;
                    Double.TryParse(property.PropertyValue.ToString(), out value);

                    if (value < 18)
                    {
                        this.errors[propertyName] = new List<string>() { "Age under 18 is not allowed!" };
                    }
                    else
                    {
                        this.errors[propertyName] = new List<string>();
                    }

                    this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
                }
                else if (propertyName.Equals("Name"))
                {
                    var name = entity.GetEntityProperty(propertyName).PropertyValue.ToString();
                    if (String.IsNullOrEmpty(name))
                    {
                        this.errors[propertyName] = new List<string>() { "The Name cannot be empty!" };
                    }
                    else
                    {
                        this.errors[propertyName] = new List<string>();
                    }

                    this.ErrorsChanged(this, new DataErrorsChangedEventArgs(propertyName));
                }

               
                return null;
            }

            public IEnumerable GetErrors(string propertyName)
            {
                if (this.errors.ContainsKey(propertyName))
                {
                    return this.errors[propertyName];
                }

                return new List<string>();
            }
        }

        private void SaveButtonClicked(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.DataForm.TransactionService.CommitAll();
        }
    }
}
