using System.Collections.Generic;
using UnityEngine;
using CardGame.Core;
using CardGame.Views;

public class FoundationZone : BaseCardContainer
{
    [SerializeField] private bool _bindToFirstAceSuit = true;
    [SerializeField] private Suit _presetSuit;

    private Suit _currentSuit;
    private bool _hasSuit;

    private void Awake()
    {
        if (_bindToFirstAceSuit)
            _hasSuit = false;
        else
        {
            _currentSuit = _presetSuit;
            _hasSuit = true;
        }
    }

    protected override void ConfigureCollider(CardView c) => c.SetColliderEnabled(false);
    protected override int GetSortingOrderFor(CardView c) => 200 + c.StackIndex;

    public override bool CanAccept(List<CardView> pile)
    {
        if (pile == null || pile.Count != 1) return false;

        var card = pile[0];
        if (!card.IsFaceUp) return false;

        if (_cards.Count == 0)
            return card.Data.rank == Rank.Ace;

        if (_hasSuit && card.Data.suit != _currentSuit)
            return false;

        var top = _cards[_cards.Count - 1];
        return (int)card.Data.rank == (int)top.Data.rank + 1;
    }

    public override void Accept(List<CardView> pile)
    {
        var card = pile[0];

        if (_cards.Count == 0 && !_hasSuit && _bindToFirstAceSuit && card.Data.rank == Rank.Ace)
        {
            _currentSuit = card.Data.suit;
            _hasSuit = true;
        }

        card.SetContainer(this);
        _cards.Add(card);
        Reflow();
    }

    public void ResetSuitBinding()
    {
        if (_bindToFirstAceSuit)
            _hasSuit = false;
        else
        {
            _currentSuit = _presetSuit;
            _hasSuit = true;
        }
        Reflow();
    }
}
