using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Core;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class EditingService : ServiceBase<RadDataGrid>
    {
        private EditOperation operation;

        public EditingService(RadDataGrid owner)
            : base(owner)
        {
        }

        public object EditItem
        {
            get
            {
                if (this.IsEditing && this.operation != null)
                {
                    return this.operation.EditItemInfo.Item;
                }

                return null;
            }
        }

        public bool IsEditing
        {
            get;
            private set;
        }

        public bool BeginEdit(ItemInfo rowInfo)
        {
            var canStartEdit = true;

            if (this.IsEditing && rowInfo.Item != this.EditItem)
            {
                canStartEdit = this.Owner.CommitEdit();
            }

            if (this.Owner.UserEditMode == DataGridUserEditMode.None || !canStartEdit)
            {
                return false;
            }

            this.IsEditing = true;

            this.InitializeEditOperation(rowInfo);

               DelegateUpdate<UpdateFlags> update = new DelegateUpdate<UpdateFlags>(() =>
                {
                    if (this.Owner.UserEditMode == DataGridUserEditMode.External)
                    {
                        if (this.Owner.ExternalEditor != null)
                        {
                            this.Owner.ExternalEditor.EditCommitted += this.OnExternalEditorCommit;
                            this.Owner.ExternalEditor.EditCancelled += this.OnExternalEditorCancel;

                            var frElement = this.Owner.ExternalEditor as FrameworkElement;
                            if (frElement != null)
                            {
                                frElement.Height = this.Owner.ActualHeight;

                                var id = this.Owner.ExternalEditor.Position == ExternalEditorPosition.Left ? DataGridFlyoutId.EditorLeft : DataGridFlyoutId.EditorRight;
                                this.Owner.ContentFlyout.Show(id, frElement);
                            }
                        }
   
                        this.Owner.ExternalEditor.BeginEdit(this.EditItem, this.Owner);
                    }
                    else
                    {
                        this.Owner.Model.BeginEdit(rowInfo);
                        this.Owner.EditRowLayer.ScheduleFirstEditorForFocus();
                    }

                    this.Owner.Model.CurrentDataProvider.CommitEditOperation(this.EditItem);

                    // hide the CurrentItem decoration
                    this.Owner.visualStateService.UpdateCurrentDecoration(-1); 
                })
                {
                    Flags = UpdateFlags.AffectsContent
                };

            this.Owner.updateService.RegisterUpdate(update);

            return true;
        }

        public bool CommitEdit(ActionTrigger trigger)
        {
            if (!this.ShouldCommitEdit())
            {
                return false;
            }

            this.IsEditing = false;

            DelegateUpdate<UpdateFlags> update = new DelegateUpdate<UpdateFlags>(() =>
                {
                    if (this.Owner.UserEditMode == DataGridUserEditMode.External)
                    {
                        var id = this.Owner.ExternalEditor.Position == ExternalEditorPosition.Left ? DataGridFlyoutId.EditorLeft : DataGridFlyoutId.EditorRight;
                        this.Owner.ContentFlyout.Hide(id);

                        if (trigger != ActionTrigger.ExternalEditor)
                        {
                            this.Owner.ExternalEditor.CommitEdit();
                        }
                    }
                    else
                    {
                        this.Owner.Model.CommitEdit();
                        this.Owner.EditRowLayer.EditorLayoutSlots.Clear();
                    }

                    this.Owner.Model.CurrentDataProvider.CommitEditOperation(this.EditItem);
                })
                {
                    Flags = UpdateFlags.AffectsContent,
                    Priority = CoreDispatcherPriority.Low
                };

            // Focusing the grid will force the Text property of all textboxes to be pushed to the underlying ViewModel.
            this.Owner.TryFocus(FocusState.Programmatic, true);

            this.Owner.updateService.RegisterUpdate(update);

                update = new DelegateUpdate<UpdateFlags>(() =>
                {
                    if (this.Owner.UserEditMode == DataGridUserEditMode.External)
                    {
                        this.Owner.CurrencyService.RefreshCurrentItem(true);
                    }
                    else
                    {
                        this.Owner.EditRowLayer.EditorLayoutSlots.Clear();
                        this.Owner.FrozenEditRowLayer.EditorLayoutSlots.Clear();
                        this.Owner.CurrencyService.RefreshCurrentItem(true);
                    }
                })
                {
                    RequiresValidMeasure = true,
                    Priority = CoreDispatcherPriority.Low
                };          

            this.Owner.updateService.RegisterUpdate(update);

            return true;
        }

        public bool CancelEdit(ActionTrigger trigger)
        {
            if (!this.IsEditing)
            {
                return false;
            }
            
            this.IsEditing = false;
            var currentEditMode = this.operation.EditMode;

            DelegateUpdate<UpdateFlags> update = new DelegateUpdate<UpdateFlags>(() =>
            {
                if (currentEditMode == DataGridUserEditMode.External)
                {
                    var id = this.Owner.ExternalEditor.Position == ExternalEditorPosition.Left ? DataGridFlyoutId.EditorLeft : DataGridFlyoutId.EditorRight;
                    this.Owner.ContentFlyout.Hide(id);                

                    if (trigger != ActionTrigger.ExternalEditor)
                    {
                        this.Owner.ExternalEditor.CancelEdit();
                    }
                }
                else
                {
                    this.Owner.Model.CancelEdit();
                    this.Owner.EditRowLayer.EditorLayoutSlots.Clear();
                }

                this.Owner.Model.CurrentDataProvider.CommitEditOperation(this.EditItem);
            })
            {
                Flags = UpdateFlags.AffectsContent
            };

            this.Owner.updateService.RegisterUpdate(update);

            foreach (var pair in this.operation.OriginalValues)
            {
                pair.Key.SetValueForInstance(this.operation.EditItemInfo.Item, pair.Value);
            }

            if (this.operation.EditMode == DataGridUserEditMode.Inline)
            {
                this.Owner.EditRowLayer.EditorLayoutSlots.Clear();
            }

            this.operation = null;

            // Return the focus to the owning grid.
            this.Owner.TryFocus(FocusState.Programmatic, true);

            return true;
        }

        internal bool ShouldCommitEdit()
        {
            return this.IsEditing && this.IsDataValid();
        }

        private void OnExternalEditorCommit(object sender, EventArgs e)
        {
            this.Owner.commandService.ExecuteCommand(CommandId.CommitEdit, new EditContext(new DataGridCellInfo(this.EditItem, null), ActionTrigger.ExternalEditor, null));
            var editor = sender as IGridExternalEditor;
            if (editor != null)
            {
                editor.EditCommitted -= this.OnExternalEditorCommit;
                editor.EditCancelled -= this.OnExternalEditorCancel;
            }
        }

        private void OnExternalEditorCancel(object sender, EventArgs e)
        {
            this.Owner.commandService.ExecuteCommand(CommandId.CancelEdit, new EditContext(new DataGridCellInfo(this.EditItem, null), ActionTrigger.ExternalEditor, null));
            var editor = sender as IGridExternalEditor;
            if (editor != null)
            {
                editor.EditCommitted -= this.OnExternalEditorCommit;
                editor.EditCancelled -= this.OnExternalEditorCancel;
            }
        }

        private bool IsDataValid()
        {            
            bool valid = true;

            if (this.Owner.UserEditMode == DataGridUserEditMode.External)
            {
                foreach (var column in this.Owner.Columns)
                {
                    valid &= column.IsCellValid(this.EditItem);
                }
            }
            else
            {
                foreach (var cell in this.Owner.Model.GetEditCells())
                {
                    valid &= cell.Column.IsCellValid(cell);
                }
            }

            return valid;
        }

        private void InitializeEditOperation(ItemInfo rowInfo)
        {
            this.operation = new EditOperation(rowInfo, this.Owner.UserEditMode);

            foreach (var column in this.Owner.Columns)
            {
                if (column is DataGridTypedColumn)
                {
                    this.operation.OriginalValues.Add(column, column.GetValueForInstance(rowInfo.Item));
                }
            }
        }
    }
}
