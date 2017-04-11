using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Linq;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;
using Windows.UI;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Documents;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input.Tests.AutoCompleteBox
{
    [TestClass]
    public class SuggestionItemTests
    {
        private static string DefaultFontName = "Segoe UI";

        private static int DefaultFontSize = 15;

        private SuggestionItem suggestionItem;
        private TextBlock textBlock;

        [TestInitialize]
        public void TestInitialize()
        {
            this.suggestionItem = new SuggestionItem();
            this.suggestionItem.PrepareHighlightedRunsList();
            this.textBlock = new TextBlock();
        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_NoMatch()
        {
            this.textBlock.Text = "This is test";
            suggestionItem.HighlightStartingMatch(textBlock, "Thin", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 1, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>((textBlock.Inlines.ElementAt(0) as Run).Text, "This is test", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle((textBlock.Inlines.ElementAt(0) as Run), true);
        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_StartWith()
        {
            this.textBlock.Text = "TestTest";
            suggestionItem.HighlightStartingMatch(textBlock, "Test", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(1) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more(or less) then two Run in the textBlock");
            Assert.AreEqual<string>(highlightedRun.Text, "Test", "Current text is not valid");
            Assert.AreEqual<string>(regularRun.Text, "Test", "Current text is not valid");
            ItemHighlightHelper.AssertHighlightedRun_InDefaultStyle(highlightedRun, false);
            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);

        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_AllTextMatch()
        {
            this.textBlock.Text = "Testing String";
            suggestionItem.HighlightStartingMatch(textBlock, "Testing String", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 1, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>((textBlock.Inlines.ElementAt(0) as Run).Text, "Testing String", "Current text is not valid");
            ItemHighlightHelper.AssertHighlightedRun_InDefaultStyle(highlightedRun, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_EmptySpaceInput()
        {
            this.textBlock.Text = "This is test";
            suggestionItem.HighlightStartingMatch(textBlock, " is", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
            Run regularRun = textBlock.Inlines.ElementAt(0) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 1, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>(regularRun.Text, "This is test", "Current text is not valid");
            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_StartWith_CustomHighlightStyle()
        {
            this.textBlock.Text = "TestTest";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);

            suggestionItem.HighlightStartingMatch(textBlock, "Test", customHightlightStyle);
            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(1) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more(or less) then two Run in the textBlock");
            Assert.AreEqual<string>(highlightedRun.Text, "Test", "Current text is not valid");
            Assert.AreEqual<string>(regularRun.Text, "Test", "Current text is not valid");

            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_AllTextMatch_CustomHighlightStyle()
        {
            this.textBlock.Text = "Testing String";

            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);

            suggestionItem.HighlightStartingMatch(textBlock, "Testing String", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));

            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Assert.AreEqual<int>(textBlock.Inlines.Count(), 1, "There is more(or less) then two Run in the textBlock");
            Assert.AreEqual<string>(highlightedRun.Text, "Testing String", "Current text is not valid");
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_LastHalfTextMatch_CustomHighlightStyle()
        {
            this.textBlock.Text = "SomethingToTest";

            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);

            suggestionItem.HighlightStartingMatch(textBlock, "Test", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
            Run regularRun = textBlock.Inlines.ElementAt(0) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 1, "There is more(or less) then two Run in the textBlock");
            Assert.AreEqual<string>(regularRun.Text, "SomethingToTest", "Current text is not valid");
            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_MiddleWordInput()
        {
            this.textBlock.Text = "That is test";
            suggestionItem.HighlightAllMatches(textBlock, "is", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
            Run highlightedRun = textBlock.Inlines.ElementAt(1) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(0) as Run;
            Run regularRun2 = textBlock.Inlines.ElementAt(2) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 3, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>(regularRun.Text, "That ", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "is", "Current text is not valid");
            Assert.AreEqual<string>(regularRun2.Text, " test", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            this.AssertDefaultSuggestionTem_RunStyle(regularRun2, true);
            ItemHighlightHelper.AssertHighlightedRun_InDefaultStyle(highlightedRun, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_AllTextMatch()
        {
            this.textBlock.Text = "This is test";
            suggestionItem.HighlightAllMatches(textBlock, "This is test", RadAutoCompleteBox.GetTextMatchHighlightStyle(textBlock));
            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 1, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>(highlightedRun.Text, "This is test", "Current text is not valid");
            ItemHighlightHelper.AssertHighlightedRun_InDefaultStyle(highlightedRun, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_StartWith()
        {
            this.textBlock.Text = "This is test";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);
            suggestionItem.HighlightAllMatches(textBlock, "This", customHightlightStyle);

            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(1) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more(or less) then two Run in the textBlock");
            Assert.AreEqual<string>(highlightedRun.Text, "This", "Current text is not valid");
            Assert.AreEqual<string>(regularRun.Text, " is test", "Current text is not valid");

            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_Contains()
        {
            this.textBlock.Text = "Thisisthis";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);
            suggestionItem.HighlightAllMatches(textBlock, "This", customHightlightStyle);

            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(1) as Run;
            Run highlightedRun2 = textBlock.Inlines.ElementAt(2) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 3, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>(regularRun.Text, "is", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "This", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun2.Text, "this", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun2, customHightlightStyle, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_ContainsAtTheEnd()
        {
            this.textBlock.Text = "NewTestTest";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);
            suggestionItem.HighlightAllMatches(textBlock, "Test", customHightlightStyle);

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 3, "There is more then one Run in the textBlock");

            Run highlightedRun = textBlock.Inlines.ElementAt(1) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(0) as Run;
            Run highlightedRun2 = textBlock.Inlines.ElementAt(2) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 3, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>(regularRun.Text, "New", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "Test", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun2.Text, "Test", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun2, customHightlightStyle, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_ContainsAtTheBeginning()
        {
            this.textBlock.Text = "TestTestNew";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);
            suggestionItem.HighlightAllMatches(textBlock, "Test", customHightlightStyle);

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 3, "There is more then one Run in the textBlock");

            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(2) as Run;
            Run highlightedRun2 = textBlock.Inlines.ElementAt(1) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 3, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>(regularRun.Text, "New", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "Test", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun2.Text, "Test", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun2, customHightlightStyle, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_LastContains()
        {
            this.textBlock.Text = "ThisIsNew";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);
            suggestionItem.HighlightAllMatches(textBlock, "New", customHightlightStyle);

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more then one Run in the textBlock");

            Run highlightedRun = textBlock.Inlines.ElementAt(1) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(0) as Run;

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more then one Run in the textBlock");
            Assert.AreEqual<string>(regularRun.Text, "ThisIs", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "New", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightAllMatch_RepeatingLetters()
        {
            this.textBlock.Text = "rrrrrrr!!!";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);
            suggestionItem.HighlightAllMatches(textBlock, "rrr", customHightlightStyle);

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 3, "There is more then one Run in the textBlock");

            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Run highlightedRun2 = textBlock.Inlines.ElementAt(1) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(2) as Run;

            Assert.AreEqual<string>(regularRun.Text, "r!!!", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "rrr", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun2.Text, "rrr", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun2, customHightlightStyle, false);

            this.textBlock.Text = "rrrrrrr!!!";
            suggestionItem.HighlightAllMatches(textBlock, "rrrr", customHightlightStyle);
            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more then one Run in the textBlock");

             highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
             regularRun = textBlock.Inlines.ElementAt(1) as Run;

             Assert.AreEqual<string>(regularRun.Text, "rrr!!!", "Current text is not valid");
             Assert.AreEqual<string>(highlightedRun.Text, "rrrr", "Current text is not valid");

             this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
             ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
        }

        [TestMethod]
        public void SuggestionItem_HighlightStartingMatch_RepeatingLetters()
        {
            this.textBlock.Text = "rrrrrrr!!!";
            HighlightStyle customHightlightStyle = new HighlightStyle();
            customHightlightStyle.Foreground = new SolidColorBrush(Colors.Red);
            customHightlightStyle.FontSize = 30;

            RadAutoCompleteBox.SetTextMatchHighlightStyle(textBlock, customHightlightStyle);
            suggestionItem.HighlightStartingMatch(textBlock, "rrr", customHightlightStyle);

            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more then one Run in the textBlock");

            Run highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            Run regularRun = textBlock.Inlines.ElementAt(1) as Run;

            Assert.AreEqual<string>(regularRun.Text, "rrrr!!!", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "rrr", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);

            this.textBlock.Text = "rrrrrrr!!!";
            suggestionItem.HighlightStartingMatch(textBlock, "rrrr", customHightlightStyle);
            Assert.AreEqual<int>(textBlock.Inlines.Count(), 2, "There is more then one Run in the textBlock");

            highlightedRun = textBlock.Inlines.ElementAt(0) as Run;
            regularRun = textBlock.Inlines.ElementAt(1) as Run;

            Assert.AreEqual<string>(regularRun.Text, "rrr!!!", "Current text is not valid");
            Assert.AreEqual<string>(highlightedRun.Text, "rrrr", "Current text is not valid");

            this.AssertDefaultSuggestionTem_RunStyle(regularRun, true);
            ItemHighlightHelper.AssertHighlightedRun_InCustomStyle(highlightedRun, customHightlightStyle, false);
        }

        private void AssertDefaultSuggestionTem_RunStyle(Run HighlightRun, bool isItemSelected)
        {
            if (isItemSelected)
            {
                Assert.AreEqual((HighlightRun.Foreground as SolidColorBrush).Color, Colors.White);
            }
            else
            {
                Assert.AreEqual((HighlightRun.Foreground as SolidColorBrush).Color, Colors.Black);
            }

            Assert.AreEqual(HighlightRun.FontWeight.Weight, 400);
            Assert.AreEqual(HighlightRun.FontSize, DefaultFontSize);
            Assert.AreEqual(HighlightRun.FontStyle.ToString(), "Normal");
            Assert.AreEqual(HighlightRun.FontFamily.Source, DefaultFontName);
        }
    }
}
