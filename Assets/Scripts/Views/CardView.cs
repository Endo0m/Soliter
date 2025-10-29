using UnityEngine;
using DG.Tweening;
using CardGame.Containers;

namespace CardGame.Views
{
    [RequireComponent(typeof(SpriteRenderer))]
    [RequireComponent(typeof(BoxCollider2D))]
    public class CardView : MonoBehaviour
    {
        [Header("Данные (runtime)")]
        [SerializeField] private CardData _data;
        [SerializeField] private CardTheme _theme;

        [Header("Ссылки (необяз. в инспекторе)")]
        [SerializeField] private SpriteRenderer _renderer;
        [SerializeField] private BoxCollider2D _collider;

        [Header("Состояние")]
        [SerializeField] private bool _isFaceUp;
        [SerializeField] private int _stackIndex;

        private ICardContainer _container;

        public CardData Data => _data;
        public bool IsFaceUp => _isFaceUp;
        public int StackIndex { get => _stackIndex; set => _stackIndex = value; }
        public ICardContainer Container => _container;

        public void Init(CardData data, CardTheme theme, bool faceUp)
        {
            _data = data;
            _theme = theme;
            _isFaceUp = faceUp;
            EnsureRenderer();
            EnsureCollider();
            UpdateVisualImmediate();
        }

        public void SetContainer(ICardContainer c) => _container = c;

        public void UpdateVisualImmediate()
        {
            EnsureRenderer();
            _renderer.sprite = _isFaceUp
                ? (_data != null ? _data.face : null)
                : (_theme != null ? _theme.back : null);
        }

        public Tween Flip(bool faceUp)
        {
            if (_isFaceUp == faceUp) return null;
            _isFaceUp = faceUp;

            float half = (_theme != null ? _theme.flipDuration : 0.2f) * 0.5f;
            var seq = DOTween.Sequence();
            seq.Append(transform.DOScaleX(0f, half));
            seq.AppendCallback(UpdateVisualImmediate);
            seq.Append(transform.DOScaleX(1f, half));
            return seq;
        }

        public Tween MoveTo(Vector3 worldPos)
        {
            float dur = _theme != null ? _theme.moveDuration : 0.25f;
            return transform.DOMove(worldPos, dur).SetEase(Ease.InOutQuad);
        }

        public void SetSorting(int order)
        {
            EnsureRenderer();
            _renderer.sortingOrder = order;
        }

        public void SetColliderEnabled(bool enabled)
        {
            EnsureCollider();
            if (_collider != null) _collider.enabled = enabled;
        }

        private void EnsureRenderer()
        {
            if (_renderer == null) _renderer = GetComponent<SpriteRenderer>();
        }

        private void EnsureCollider()
        {
            if (_collider == null) _collider = GetComponent<BoxCollider2D>();
        }
    }
}
