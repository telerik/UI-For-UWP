using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Telerik.UI.Xaml.Controls.Input.NumericBox;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    public class DFNumericTextBox : NumericTextBox
    {
        public EditorIconDisplayMode IconDisplayMode
        {
            get { return (EditorIconDisplayMode)GetValue(IconDisplayModeProperty); }
            set { SetValue(IconDisplayModeProperty, value); }
        }

        // Using a DependencyProperty as the backing store for IconDisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register("IconDisplayMode", typeof(EditorIconDisplayMode), typeof(DFNumericTextBox), new PropertyMetadata(null));

        public Style LabelIconStyle
        {
            get { return (Style)GetValue(LabelIconStyleProperty); }
            set { SetValue(LabelIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register("LabelIconStyle", typeof(Style), typeof(DFNumericTextBox), new PropertyMetadata(null));

        public Style ErrorIconStyle
        {
            get { return (Style)GetValue(ErrorIconStyleProperty); }
            set { SetValue(ErrorIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register("ErrorIconStyle", typeof(Style), typeof(DFNumericTextBox), new PropertyMetadata(null));

        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasErrors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register("HasErrors", typeof(bool), typeof(DFNumericTextBox), new PropertyMetadata(false));
        
        public DFNumericTextBox()
        {
            this.DefaultStyleKey = typeof(DFNumericTextBox);
        }
    }
}
