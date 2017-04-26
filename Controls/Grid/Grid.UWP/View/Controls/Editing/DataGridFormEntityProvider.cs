using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a DataGridFormEntityProvider provider.
    /// </summary>
    public class DataGridFormEntityProvider : RuntimeEntityProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFormEntityProvider"/> class.
        /// </summary>
        public DataGridFormEntityProvider(IEnumerable<DataGridColumn> columns)
        {
            this.Columns = columns;
        }

        /// <summary>
        /// Gets or sets a collection with the provider's Columns.
        /// </summary>
        public IEnumerable<DataGridColumn> Columns { get; set; }

        /// <inheritdoc/>
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
                    if (comboColumn != null)
                    {
                        if (entityProperty.PropertyName.Equals(comboColumn.PropertyName))
                        {
                            entityProperty.DisplayMemberPath = comboColumn.DisplayMemberPath;
                            entityProperty.SelectedValuePath = comboColumn.SelectedValuePath;

                            if (!string.IsNullOrEmpty(comboColumn.ItemsSourcePath))
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

        /// <inheritdoc/>
        protected override Type GetEntityPropertyType(object property)
        {
            return typeof(GridFormEntityProperty); 
        }

        /// <inheritdoc/>
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
