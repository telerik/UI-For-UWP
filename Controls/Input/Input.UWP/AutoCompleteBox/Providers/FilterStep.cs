namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    internal struct FilterStep
    {
        internal string Input;
        internal int Index;
        internal int Length;

        internal FilterStep(int index, int length, string input)
        {
            this.Index = index;
            this.Length = length;
            this.Input = input;
        }
    }
}
