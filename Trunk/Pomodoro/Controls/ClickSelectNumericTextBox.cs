using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Pomodoro.Controls
{
    public class ClickSelectNumericTextBox : TextBox
    {
        public ClickSelectNumericTextBox()
        {
            AddHandler(PreviewMouseLeftButtonDownEvent,
              new MouseButtonEventHandler(SelectivelyIgnoreMouseButton), true);
            AddHandler(GotKeyboardFocusEvent,
              new RoutedEventHandler(SelectAllText), true);
            AddHandler(MouseDoubleClickEvent,
              new RoutedEventHandler(SelectAllText), true);
         
        }

        protected override void OnPreviewTextInput(TextCompositionEventArgs e)
        {
            var c = Convert.ToChar(e.Text);
            if (!Char.IsDigit(c) && !Char.IsControl(c)) e.Handled = true;

            base.OnPreviewTextInput(e);
        }

        private static void SelectivelyIgnoreMouseButton(object sender,
                                                         MouseButtonEventArgs e)
        {
            // Find the TextBox
            DependencyObject parent = e.OriginalSource as UIElement;
            while (parent != null && !(parent is TextBox))
                parent = VisualTreeHelper.GetParent(parent);

            if (parent != null)
            {
                var textBox = (TextBox)parent;
                if (!textBox.IsKeyboardFocusWithin)
                {
                    // If the text box is not yet focussed, give it the focus and
                    // stop further processing of this click event.
                    textBox.Focus();
                    e.Handled = true;
                }
            }
        }

        private static void SelectAllText(object sender, RoutedEventArgs e)
        {
            var textBox = e.OriginalSource as TextBox;
            if (textBox != null)
                textBox.SelectAll();
        }
    }
}
