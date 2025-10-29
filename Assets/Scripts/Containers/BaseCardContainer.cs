using CardGame.Views;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCardContainer : MonoBehaviour, CardGame.Containers.ICardContainer
{
    [SerializeField] protected Transform _anchor;
    protected readonly List<CardView> _cards = new();

    public Vector3 Anchor => _anchor != null ? _anchor.position : transform.position;

    public abstract bool CanAccept(List<CardView> pile);
    public abstract void Accept(List<CardView> pile);

    // По умолчанию карты кликабельны; зоны могут переопределить.
    protected virtual void ConfigureCollider(CardView c) => c.SetColliderEnabled(true);
    protected virtual int GetSortingOrderFor(CardView c) => c.StackIndex;
    protected virtual Vector3 GetWorldPositionFor(CardView card) => Anchor;

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
    }

    public virtual int IndexOf(CardView card) => _cards.IndexOf(card);

    public virtual List<CardView> ExtractPileFrom(CardView topCard)
    {
        // НИЧЕГО НЕ ПЕРЕВОРАЧИВАЕМ ЗДЕСЬ!
        int index = _cards.IndexOf(topCard);
        if (index < 0) return new List<CardView>();
        var result = _cards.GetRange(index, _cards.Count - index);
        _cards.RemoveRange(index, _cards.Count - index);
        return result;
    }

    public virtual void PushOnTop(CardView card)
    {
        card.SetContainer(this);
        _cards.Add(card);
        Reflow();
    }

    public virtual void InsertAt(int index, List<CardView> pile)
    {
        if (pile == null || pile.Count == 0) return;
        if (index < 0) index = 0;
        if (index > _cards.Count) index = _cards.Count;

        for (int i = 0; i < pile.Count; i++)
        {
            var card = pile[i];
            card.SetContainer(this);
            _cards.Insert(index + i, card);
        }
        Reflow();
    }

    // По умолчанию просто переставим оставшиеся карты
    public virtual void OnPileMovedAway(int originalIndex) { Reflow(); }
}
