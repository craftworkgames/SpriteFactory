using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace SpriteFactory.Controls
{
    public partial class EditableTextBlock : UserControl
    {
        public EditableTextBlock()
        {
            InitializeComponent();

            TextBox.Visibility = Visibility.Hidden;
            PreviewMouseDoubleClick += (sender, args) => BeginEdit();
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(EditableTextBlock), new FrameworkPropertyMetadata
        {
            BindsTwoWayByDefault = true,
            DefaultUpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged
        });

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        private string _oldText;

        public bool IsEditing { get; set; }

        private async void BeginEdit()
        {
            if (IsEditing)
                return;

            IsEditing = true;
            _oldText = TextBlock.Text;
            TextBlock.Visibility = Visibility.Hidden;
            TextBox.Visibility = Visibility.Visible;
            TextBox.LostFocus += (sender, args) => CommitEdit();
            TextBox.PreviewKeyDown += TextBox_PreviewKeyDown;

            Debug.Assert(Dispatcher != null);
            await Dispatcher.BeginInvoke(DispatcherPriority.Input,
                new Action(delegate
                {
                    TextBox.SelectAll();
                    TextBox.Focus();
                    Keyboard.Focus(TextBox);
                }));
        }

        private void TextBox_PreviewKeyDown(object sender, KeyEventArgs args)
        {
            switch (args.Key)
            {
                case Key.Enter:
                    CommitEdit();
                    break;
                case Key.Escape:
                    CancelEdit();
                    break;
            }
        }

        private void CancelEdit()
        {
            TextBox.Text = _oldText;
            CommitEdit();
        }

        private void CommitEdit()
        {
            if (!IsEditing)
                return;

            IsEditing = false;
            TextBox.PreviewKeyDown -= TextBox_PreviewKeyDown;
            TextBlock.Visibility = Visibility.Visible;
            TextBox.Visibility = Visibility.Hidden;
        }

        private void RenameButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (!IsEditing)
                BeginEdit();
            else
                CommitEdit();
        }
    }
}
