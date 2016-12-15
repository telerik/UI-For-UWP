using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    public class DataGridFormEntityProvider : RuntimeEntityProvider
    {
        public IEnumerable<DataGridColumn> Columns { get; set; }

        public DataGridFormEntityProvider(IEnumerable<DataGridColumn> columns)
        {
            this.Columns = columns;
        }

        public override Entity GenerateEntity()
        {
            Entity entity = new Entity();
            var properties = this.GetProperties();

            foreach (var property in properties)
            {
                if (!this.ShouldGenerateEntityProperty(property))
                {
                    continue;
                }
                GridFormEntityProperty entityProperty = this.GenerateEntityProperty(property) as GridFormEntityProperty;
                foreach (var column in this.Columns)
                {
                    var comboColumn = column as DataGridComboBoxColumn;
                    if(comboColumn != null)
                    {
                        if (entityProperty.PropertyName.Equals(comboColumn.PropertyName))
                        {
                            entityProperty.DisplayMemberPath = comboColumn.DisplayMemberPath;
                            entityProperty.SelectedValuePath = comboColumn.SelectedValuePath;

                            if (!String.IsNullOrEmpty(comboColumn.ItemsSourcePath))
                            {
                                var info = this.Context.GetType().GetRuntimeProperty(comboColumn.ItemsSourcePath);
                                entityProperty.ValueOptions = info.GetMethod.Invoke(this.Context, new object[0]) as IList;
                            }
                            else
                            {
                                entityProperty.ValueOptions = comboColumn.ItemsSource as IList;
                            }
                        }
                    }                 
                }

                entity.Properties.Add(entityProperty);
            }

            entity.Validator = this.GetItemValidator(entity);
            this.Entity = entity;
            return entity;
        }

        protected override Type GetEntityPropertyType(object property)
        {
            return typeof(GridFormEntityProperty); 
        }

        protected override bool ShouldGenerateEntityProperty(object property)
        {
            var propertyInfo = property as PropertyInfo;

            foreach (var column in this.Columns)
            {
                var typedColumn = column as DataGridTypedColumn;

                if (typedColumn.PropertyName.Equals(propertyInfo.Name))
                {
                    return true;
                }
            }       

            return false;
        }
    }
}
