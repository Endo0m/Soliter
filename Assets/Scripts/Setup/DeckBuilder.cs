using System.Collections.Generic;
using UnityEngine;
using CardGame.Views;
using CardGame.Containers;
using CardGame.Utils;

namespace CardGame.Setup
{
    public class DeckBuilder : MonoBehaviour
    {
        [Header("Исходные данные")]
        [SerializeField] private List<CardData> _cardsData;
        [SerializeField] private CardTheme _theme;

        [Header("Пул карт")]
        [SerializeField] private CardPool _pool;
        [SerializeField] private Transform _cardsParent;

        [Header("Зоны")]
        [SerializeField] private DeckZone _deck;
        [SerializeField] private WasteZone _waste;
        [SerializeField] private List<TableauColumn> _tableau; 

        [Header("Параметры раздачи")]
        [SerializeField] private bool _dealOnStart = true;

        private readonly List<CardView> _allCards = new();

        private void Start()
        {
            if (_dealOnStart) BuildAndDeal();
        }

        public void BuildAndDeal()
        {
            Clear();

            var dataList = new List<CardData>(_cardsData);
            dataList.ShuffleInPlace();

            foreach (var data in dataList)
            {
                var view = _pool.Get(_cardsParent);
                view.transform.position = _deck.Anchor; 
                view.Init(data, _theme, false);
                _allCards.Add(view);
                _deck.PushOnTop(view);
            }

            for (int col = 0; col < _tableau.Count; col++)
            {
                for (int i = 0; i <= col; i++)
                {
                    var pile = _deck.ExtractPileFromTop(1);
                    var card = pile[0];
                    bool isTop = i == col;
                    if (isTop) card.Flip(true); else card.Flip(false);
                    _tableau[col].Accept(pile);
                }
            }
        }

        private void Clear()
        {
            foreach (var c in _allCards)
                if (c != null) _pool.Release(c);
            _allCards.Clear();
        }
    }

    public static class DeckZoneExtensions
    {
        public static List<CardView> ExtractPileFromTop(this DeckZone deck, int count)
        {
            var field = typeof(BaseCardContainer).GetField("_cards",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var list = (List<CardView>)field.GetValue(deck);

            var result = new List<CardView>();
            for (int i = 0; i < count && list.Count > 0; i++)
            {
                var card = list[list.Count - 1];
                list.RemoveAt(list.Count - 1);
                result.Add(card);
            }
            result.Reverse();
            return result;
        }
    }
}
