using System.Collections.Generic;
using UnityEngine;
using CardGame.Views;

namespace CardGame.Containers
{
    public class DeckZone : BaseCardContainer
    {
        [SerializeField] private WasteZone _waste;
        [SerializeField] private int _drawCount = 1;

        // В колоде карты не перехватывают клик — коллайдер ВЫКЛ
        protected override void ConfigureCollider(CardView c) => c.SetColliderEnabled(false);

        public override bool CanAccept(List<CardView> pile) => false;
        public override void Accept(List<CardView> pile) { }

        private void OnMouseDown() => Draw();

        public void Draw()
        {
            if (_cards.Count == 0)
            {
                RecycleFromWaste();
                return;
            }

            int count = Mathf.Min(_drawCount, _cards.Count);
            for (int i = 0; i < count; i++)
            {
                var card = _cards[_cards.Count - 1];
                _cards.RemoveAt(_cards.Count - 1);

                card.SetContainer(_waste);
                card.StackIndex = 0;
                card.Flip(true);
                _waste.Accept(new List<CardView> { card });
            }
        }

        private void RecycleFromWaste()
        {
            while (true)
            {
                var top = _waste.PopTop();
                if (top == null) break;
                top.SetContainer(this);
                _cards.Add(top);
                top.Flip(false);
            }
            Reflow(); // позиции/сортинг/коллайдеры в колоде
        }
    }

    public static class WasteZoneHelpers
    {
        public static CardView PopTop(this WasteZone waste)
        {
            var field = typeof(BaseCardContainer).GetField("_cards", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var list = (System.Collections.Generic.List<CardView>)field.GetValue(waste);
            if (list.Count == 0) return null;
            var c = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return c;
        }
    }
}
