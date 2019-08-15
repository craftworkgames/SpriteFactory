using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpriteFactory.GameObjects
{
    public partial class GameObjectView : UserControl
    {
        public GameObjectView()
        {
            InitializeComponent();
        }

        private object _dragItem;

        private void ListBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && ListBox.IsMouseDirectlyOver)
            {
                if (_dragItem == null)
                    _dragItem = ListBox.SelectedItem;

                DragDrop.DoDragDrop(ListBox, _dragItem, DragDropEffects.Copy);
            }
        }

        private void ListBox_OnMouseLeave(object sender, MouseEventArgs e)
        {
            _dragItem = null;
        }
    }
}
