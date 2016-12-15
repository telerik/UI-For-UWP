namespace Telerik.UI.Xaml.Controls.Map
{
    internal class ViewChangedCommand : MapCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner != null && parameter is ViewChangedContext;
        }

        public override void Execute(object parameter)
        {
            var context = parameter as ViewChangedContext;
            this.Owner.OnViewChanged(context);
        }
    }
}
