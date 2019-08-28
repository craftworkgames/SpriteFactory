namespace SpriteFactory.Sprites
{
    public class ZoomOptionViewModel : ViewModel
    {
        public ZoomOptionViewModel(int value) => Value = value;
        public int Value { get; }
        public override string ToString() => $"{Value}x";
    }
}