using System.Collections.Generic;
using CardGame.Views;

public class WasteZone : BaseCardContainer
{
    protected override void ConfigureCollider(CardView c) => c.SetColliderEnabled(true);
    protected override int GetSortingOrderFor(CardView c) => 300 + c.StackIndex;

    public override bool CanAccept(List<CardView> pile) => pile != null && pile.Count == 1;

    public override void Accept(List<CardView> pile)
    {
        var c = pile[0];
        c.SetContainer(this);
        _cards.Add(c);
        Reflow();
    }

    public override List<CardView> ExtractPileFrom(CardView topCard)
    {
        if (_cards.Count == 0) return new List<CardView>();
        var top = _cards[_cards.Count - 1];
        if (top != topCard) return new List<CardView>();
        _cards.RemoveAt(_cards.Count - 1);
        return new List<CardView> { top };
    }
}
