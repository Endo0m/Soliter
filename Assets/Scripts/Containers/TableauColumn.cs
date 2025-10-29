using System.Collections.Generic;
using UnityEngine;
using CardGame.Core;
using CardGame.Views;

public class TableauColumn : BaseCardContainer
{
    [SerializeField] private float _stepY = -0.35f;

    protected override void ConfigureCollider(CardView c)
    {
        c.SetColliderEnabled(c.IsFaceUp);
    }

    protected void Reflow()
    {
        for (int i = 0; i < _cards.Count; i++)
        {
            var c = _cards[i];
            c.StackIndex = i;
            c.MoveTo(GetWorldPositionFor(c));
            c.SetSorting(GetSortingOrderFor(c));
            ConfigureCollider(c);
        }
        UpdateColliderBounds();
    }

    private void UpdateColliderBounds()
    {
        var bc = GetComponent<BoxCollider2D>();
        if (bc == null) return;

        if (_cards.Count == 0)
        {
            bc.size = new Vector2(0.5f, 1.3f);
            bc.offset = Vector2.zero;
            return;
        }

        float height = Mathf.Abs(_stepY) * (_cards.Count - 1) + 1.3f;
        bc.size = new Vector2(0.5f, height);
        bc.offset = new Vector2(0f, -height / 2f + 0.65f);
    }

    protected override Vector3 GetWorldPositionFor(CardView card)
    {
        return Anchor + new Vector3(0f, _stepY * card.StackIndex, 0f);
    }

    public override bool CanAccept(List<CardView> pile)
    {
        if (pile == null || pile.Count == 0) return false;
        var top = pile[0];
        if (!top.IsFaceUp) return false;

        if (_cards.Count == 0)
            return top.Data.rank == Rank.King;

        var target = _cards[_cards.Count - 1];
        bool colorsAlternate = target.Data.suit.IsRed() != top.Data.suit.IsRed();
        bool descending = (int)top.Data.rank == (int)target.Data.rank - 1;
        return colorsAlternate && descending;
    }

    public override void Accept(List<CardView> pile)
    {
        for (int i = 0; i < pile.Count; i++)
        {
            var card = pile[i];
            card.SetContainer(this);
            _cards.Add(card);
        }
        Reflow();
    }

    public override void OnPileMovedAway(int originalIndex)
    {
        if (_cards.Count > 0)
        {
            var newTop = _cards[_cards.Count - 1];
            if (!newTop.IsFaceUp) newTop.Flip(true);
        }
        Reflow();
    }
}
