using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Telerik.UI.Xaml.Controls.Input.DateTimePickers;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    public class DFDateTimePickerButton : DateTimePickerButton
    {
        public Style ErrorIconStyle
        {
            get { return (Style)GetValue(ErrorIconStyleProperty); }
            set { SetValue(ErrorIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ErrorIconStyleProperty =
            DependencyProperty.Register("ErrorIconStyle", typeof(Style), typeof(DFDateTimePickerButton), new PropertyMetadata(null));

        public bool HasErrors
        {
            get { return (bool)GetValue(HasErrorsProperty); }
            set { SetValue(HasErrorsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for HasErrors.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HasErrorsProperty =
            DependencyProperty.Register("HasErrors", typeof(bool), typeof(DFDateTimePickerButton), new PropertyMetadata(false));   
        
        public EditorIconDisplayMode IconDisplayMode
        {
            get { return (EditorIconDisplayMode)GetValue(IconDisplayModeProperty); }
            set { SetValue(IconDisplayModeProperty, value); } 
        }

        // Using a DependencyProperty as the backing store for IconDisplayMode.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty IconDisplayModeProperty =
            DependencyProperty.Register("IconDisplayMode", typeof(EditorIconDisplayMode), typeof(DFDateTimePickerButton), new PropertyMetadata(EditorIconDisplayMode.None));
        
        public Style LabelIconStyle
        {
            get { return (Style)GetValue(LabelIconStyleProperty); }
            set { SetValue(LabelIconStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LabelIconStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LabelIconStyleProperty =
            DependencyProperty.Register("LabelIconStyle", typeof(Style), typeof(DFDateTimePickerButton), new PropertyMetadata(null));

        public DFDateTimePickerButton()
        {
            this.DefaultStyleKey = typeof(DFDateTimePickerButton);
        }
    }
}
