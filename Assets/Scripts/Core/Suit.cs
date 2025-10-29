namespace CardGame.Core
{
    public enum Suit { Hearts, Diamonds, Clubs, Spades }
    public static class SuitExtensions
    {
        public static bool IsRed(this Suit s) => s == Suit.Hearts || s == Suit.Diamonds;
    }
}
