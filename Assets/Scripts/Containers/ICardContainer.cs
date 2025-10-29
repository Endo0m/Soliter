using System.Collections.Generic;
using UnityEngine;
using CardGame.Views;

namespace CardGame.Containers
{
    public interface ICardContainer
    {
        Vector3 Anchor { get; }
        bool CanAccept(List<CardView> pile);
        void Accept(List<CardView> pile);
        List<CardView> ExtractPileFrom(CardView topCard);
        void InsertAt(int index, List<CardView> pile);
        int IndexOf(CardView card);
        void OnPileMovedAway(int originalIndex);
    }
}
