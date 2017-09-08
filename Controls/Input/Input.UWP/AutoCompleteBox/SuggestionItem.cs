using System;
using System.Collections.Generic;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Input.AutoCompleteBox
{
    /// <summary>
    /// Represents the visual suggestion item displayed in the <see cref="SuggestionItemsControl"/> of a <see cref="RadAutoCompleteBox"/>.
    /// </summary>
    public class SuggestionItem : ListBoxItem
    {
        internal object DataItem;

        private const string RootElementPartName = "RootElement";

        private SuggestionItemsControl owner;
        private string stringToHighlight;
        private bool highlightDirty = false;
        private StringComparison comparisonMode = StringComparison.CurrentCultureIgnoreCase;
        private List<KeyValuePair<Run, HighlightStyle>> highlightedRuns;
        private VisualStateGroup mousePointerStates;
        private VisualStateGroup keyboardStates;

        /// <summary>
        /// Initializes a new instance of the <see cref="SuggestionItem" /> class.
        /// </summary>
        public SuggestionItem()
        {
            this.DefaultStyleKey = typeof(SuggestionItem);
        }

        internal void HighlightInput(string input)
        {
            if (!string.IsNullOrEmpty(input))
            {
                this.stringToHighlight = input;
                this.highlightDirty = true;
            }
        }

        internal void Attach(SuggestionItemsControl owningControl, object dataItem)
        {
            this.owner = owningControl;
            this.DataItem = dataItem;
        }

        internal void Detach()
        {
            this.owner = null;
            this.DataItem = null;

            if (this.highlightedRuns != null)
            {
                this.highlightedRuns.Clear();
            }
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal void OnItemTap()
        {
            this.owner.OnItemTapped(this);
        }

        /// <summary>
        /// This method is made Internal for testing purposes.
        /// </summary>
        internal void HighlightStartingMatch(TextBlock textBlock, string userInput, HighlightStyle styleInfo)
        {
            string text = textBlock.Text;

            if (!text.StartsWith(userInput, this.comparisonMode))
            {
                return;
            }

            textBlock.Inlines.Clear();

            Run firstRun = SuggestionItem.GetRunFromStyle(styleInfo);
            firstRun.Text = text.Substring(0, userInput.Length);
            textBlock.Inlines.Add(firstRun);

            this.highlightedRuns.Add(new KeyValuePair<Run, HighlightStyle>(firstRun, styleInfo));

            int remainingLength = text.Length - userInput.Length;
            if (remainingLength != 0)
            {
                Run secondRun = new Run();
                secondRun.Text = text.Substring(userInput.Length, remainingLength);
                textBlock.Inlines.Add(secondRun);
            }
        }

        /// <summary>
        /// This method is made Internal for testing purposes.
        /// </summary>
        internal void HighlightAllMatches(TextBlock textBlock, string userInput, HighlightStyle styleInfo)
        {
            string text = textBlock.Text;

            int indexOfOccurance = text.IndexOf(userInput, this.comparisonMode);
            if (indexOfOccurance == -1)
            {
                return;
            }

            textBlock.Inlines.Clear();

            int portionStart = 0;

            while (indexOfOccurance != -1)
            {
                string firstTextPortion = text.Substring(portionStart, indexOfOccurance - portionStart);

                if (!string.IsNullOrEmpty(firstTextPortion))
                {
                    Run firstRun = new Run();
                    firstRun.Text = firstTextPortion;
                    textBlock.Inlines.Add(firstRun);
                }

                Run highLightedRun = SuggestionItem.GetRunFromStyle(styleInfo);

                highLightedRun.Text = text.Substring(indexOfOccurance, userInput.Length);
                textBlock.Inlines.Add(highLightedRun);
                this.highlightedRuns.Add(new KeyValuePair<Run, HighlightStyle>(highLightedRun, styleInfo));

                int nextOccuranceIndex = text.IndexOf(userInput, indexOfOccurance + userInput.Length, this.comparisonMode);

                if (nextOccuranceIndex == -1)
                {
                    int lastPortionStart = indexOfOccurance + userInput.Length;
                    string lastPortion = text.Substring(lastPortionStart, text.Length - lastPortionStart);

                    if (!string.IsNullOrEmpty(lastPortion))
                    {
                        Run lastRun = new Run();
                        lastRun.Text = lastPortion;
                        textBlock.Inlines.Add(lastRun);
                    }
                }
                else
                {
                    portionStart = indexOfOccurance + userInput.Length;
                }

                indexOfOccurance = nextOccuranceIndex;
            }
        }

        /// <summary>
        /// This method is made Internal for testing purposes.
        /// </summary>
        internal void PrepareHighlightedRunsList()
        {
            if (this.highlightedRuns == null)
            {
                this.highlightedRuns = new List<KeyValuePair<Run, HighlightStyle>>();
            }
            else if (this.highlightedRuns.Count > 0)
            {
                this.highlightedRuns.Clear();
            }
        }

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            if (this.mousePointerStates != null)
            {
                this.mousePointerStates.CurrentStateChanged -= this.OnVisualStateChanged;
                this.mousePointerStates = null;
            }

            if (this.keyboardStates != null)
            {
                this.keyboardStates.CurrentStateChanged -= this.OnVisualStateChanged;
                this.keyboardStates = null;
            }

            base.OnApplyTemplate();

            FrameworkElement rootElement = this.GetTemplateChild(RootElementPartName) as FrameworkElement;
            if (rootElement == null)
            {
                return;
            }

            IList<VisualStateGroup> visualStateGroups = VisualStateManager.GetVisualStateGroups(rootElement);
            foreach (VisualStateGroup group in visualStateGroups)
            {
                if (group.Name == "CommonStates")
                {
                    this.mousePointerStates = group;
                }
                else if (group.Name == "SelectionStates")
                {
                    this.keyboardStates = group;
                }

                if (this.mousePointerStates != null && this.keyboardStates != null)
                {
                    break;
                }
            }

            if (this.mousePointerStates != null)
            {
                this.mousePointerStates.CurrentStateChanged += this.OnVisualStateChanged;
            }

            if (this.keyboardStates != null)
            {
                this.keyboardStates.CurrentStateChanged += this.OnVisualStateChanged;
            }
        }

        /// <summary>
        /// Called in the measure layout pass to determine the desired size.
        /// </summary>
        /// <param name="availableSize">The available size that was given by the layout system.</param>
        protected override Size MeasureOverride(Size availableSize)
        {
            Size result = base.MeasureOverride(availableSize);

            if (this.highlightDirty)
            {
                this.HighlightTextBlocks();
                this.highlightDirty = false;
            }

            return result;
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            this.OnItemTap();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SuggestionItemAutomationPeer(this);
        }

        private static Run GetRunFromStyle(HighlightStyle style)
        {
            Run runToHighlight = new Run();

            if (style == null)
            {
                return runToHighlight;
            }

            if (style.ReadLocalValue(HighlightStyle.FontWeightProperty) != DependencyProperty.UnsetValue)
            {
                runToHighlight.FontWeight = FontWeightNameHelper.GetFontWeight(style.FontWeight);
            }

            if (style.ReadLocalValue(HighlightStyle.ForegroundProperty) != DependencyProperty.UnsetValue)
            {
                runToHighlight.Foreground = style.Foreground;
            }

            if (style.ReadLocalValue(HighlightStyle.FontStyleProperty) != DependencyProperty.UnsetValue)
            {
                runToHighlight.FontStyle = style.FontStyle;
            }

            if (style.ReadLocalValue(HighlightStyle.FontSizeProperty) != DependencyProperty.UnsetValue)
            {
                runToHighlight.FontSize = style.FontSize;
            }

            if (style.ReadLocalValue(HighlightStyle.FontFamilyProperty) != DependencyProperty.UnsetValue)
            {
                runToHighlight.FontFamily = style.FontFamily;
            }

            return runToHighlight;
        }

        private void ResetRunHighlightedForegroundValue()
        {
            if (this.highlightedRuns == null)
            {
                return;
            }

            foreach (KeyValuePair<Run, HighlightStyle> item in this.highlightedRuns)
            {
                if (item.Key == null)
                {
                    continue;
                }

                item.Key.ClearValue(Run.ForegroundProperty);
            }
        }

        private void RestoreRunHighlightedForegroundValue()
        {
            if (this.highlightedRuns == null)
            {
                return;
            }

            foreach (KeyValuePair<Run, HighlightStyle> item in this.highlightedRuns)
            {
                if (item.Value == null || item.Key == null)
                {
                    continue;
                }

                if (item.Value.ReadLocalValue(HighlightStyle.ForegroundProperty) != DependencyProperty.UnsetValue)
                {
                    item.Key.Foreground = item.Value.Foreground;
                }
            }
        }

        private void HighlightTextBlocks()
        {
            this.PrepareHighlightedRunsList();

            IEnumerable<DependencyObject> textBlockDescendands = ElementTreeHelper.EnumVisualDescendants(this, descendand => descendand is TextBlock);
            foreach (TextBlock textBlock in textBlockDescendands)
            {
                if (!RadAutoCompleteBox.GetIsTextMatchHighlightEnabled(textBlock))
                {
                    continue;
                }

                if (this.owner.owner.FilterMode == AutoCompleteBoxFilterMode.StartsWith)
                {
                    this.HighlightStartingMatch(textBlock, this.stringToHighlight, RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
                }
                else
                {
                    this.HighlightAllMatches(textBlock, this.stringToHighlight, RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
                }
            }

            if (this.IsSelected)
            {
                this.ResetRunHighlightedForegroundValue();
            }
        }

        private void OnVisualStateChanged(object sender, VisualStateChangedEventArgs e)
        {
            if (sender == null)
            {
                return;
            }

            if (e.NewState.Name == "PointerOver" || (e.NewState.Name == "SelectedUnfocused" || e.NewState.Name == "SelectedPointerOver"))
            {
                this.ResetRunHighlightedForegroundValue();
            }
            else if (e.NewState.Name == "Normal" || e.NewState.Name == "Unselected")
            {
                this.RestoreRunHighlightedForegroundValue();
            }
        }
    }
}
