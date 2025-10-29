using System.Collections.Generic;
using UnityEngine;
using CardGame.Views;

namespace CardGame.Setup
{
    public class CardPool : MonoBehaviour
    {
        [SerializeField] private CardView _prefab;
        [SerializeField] private int _capacity = 52;
        [SerializeField] private Transform _inactiveRoot;

        private readonly Stack<CardView> _pool = new();

        private void Awake()
        {
            for (int i = 0; i < _capacity; i++)
            {
                var go = Instantiate(_prefab, _inactiveRoot != null ? _inactiveRoot : transform);
                go.gameObject.SetActive(false);
                _pool.Push(go);
            }
        }

        public CardView Get(Transform parent = null)
        {
            var cv = _pool.Count > 0 ? _pool.Pop() : Instantiate(_prefab);
            if (parent != null) cv.transform.SetParent(parent, worldPositionStays: false);
            cv.gameObject.SetActive(true);
            return cv;
        }

        public void Release(CardView cv)
        {
            cv.gameObject.SetActive(false);
            cv.transform.SetParent(_inactiveRoot != null ? _inactiveRoot : transform, worldPositionStays: false);
            _pool.Push(cv);
        }
    }
}
