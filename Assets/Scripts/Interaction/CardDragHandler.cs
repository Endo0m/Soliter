using UnityEngine;
using System.Collections.Generic;
using CardGame.Views;
using CardGame.Containers;

namespace CardGame.Interaction
{
    [RequireComponent(typeof(CardView))]
    public class CardDragHandler : MonoBehaviour
    {
        [SerializeField] private LayerMask _dropMask;   
        [SerializeField] private float _pileOffsetY = -0.25f;

        private CardView _card;
        private Vector3 _dragOffset;
        private bool _dragging;
        private List<CardView> _dragPile = new();
        private ICardContainer _originContainer;
        private int _originIndex;

        private void Awake()
        {
            _card = GetComponent<CardView>();
        }

        private void OnMouseDown()
        {
            if (!_card.IsFaceUp) return;
            if (_card.Container == null) return;

            _originContainer = _card.Container;
            _originIndex = _originContainer.IndexOf(_card);

            _dragPile = _originContainer.ExtractPileFrom(_card);
            if (_dragPile == null || _dragPile.Count == 0) return;

            foreach (var cv in _dragPile)
                cv.SetSorting(500 + cv.StackIndex);

            _dragging = true;
            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0f;
            _dragOffset = _card.transform.position - mouse;
        }

        private void OnMouseDrag()
        {
            if (!_dragging) return;

            var mouse = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mouse.z = 0f;
            Vector3 basePos = mouse + _dragOffset;

            for (int i = 0; i < _dragPile.Count; i++)
            {
                _dragPile[i].transform.position = basePos + new Vector3(0, _pileOffsetY * i, 0);
            }
        }

        private void OnMouseUp()
        {
            if (!_dragging) return;
            _dragging = false;

            var baseCard = _dragPile[0];
            var pos = baseCard.transform.position;

         
            var bc = baseCard.GetComponent<BoxCollider2D>();
            Vector2 size = bc != null ? bc.size : new Vector2(1.6f, 2.2f);
            float shrink = 0.85f; 
            size *= shrink;

            var hits = Physics2D.OverlapBoxAll(pos, size, 0f, _dropMask);

            ICardContainer targetZone = null;

            foreach (var h in hits)
            {
                var z = h.GetComponent<ICardContainer>();
                if (z != null) { targetZone = z; break; }
            }

            if (targetZone == null)
            {
                var anyHits = Physics2D.OverlapBoxAll(pos, size, 0f);
                foreach (var h in anyHits)
                {
                    var cv = h.GetComponent<CardGame.Views.CardView>();
                    if (cv != null && cv.Container != null)
                    {
                        targetZone = cv.Container;
                        break;
                    }
                }
            }

            if (targetZone != null && targetZone.CanAccept(_dragPile))
            {
                targetZone.Accept(_dragPile);
                _originContainer.OnPileMovedAway(_originIndex); 
                _dragPile.Clear();
                return;
            }

            _originContainer.InsertAt(_originIndex, _dragPile);
            _dragPile.Clear();
        }


    }
}
