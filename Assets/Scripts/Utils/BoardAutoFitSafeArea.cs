using UnityEngine;

namespace CardGame.Setup
{

    [ExecuteAlways]
    public sealed class BoardAutoFitSafeArea : MonoBehaviour
    {
        [Header("Ссылки")]
        [SerializeField] private Camera cam;              
        [SerializeField] private Transform boardRoot;      
        [SerializeField] private Transform contentRoot;     

        [Header("Поля вокруг стола (в мировых единицах при scale=1)")]
        [SerializeField] private float marginX = 0.4f;
        [SerializeField] private float marginY = 0.6f;

        [Header("Настройки")]
        [SerializeField] private bool rebuildOnResize = true;

        private int _w, _h; private Rect _safe;

        private void Reset()
        {
            cam = Camera.main;
            boardRoot = transform;
            contentRoot = transform; 
        }

        private void OnEnable() => Apply();
        private void Update()
        {
            if (!rebuildOnResize) return;
            if (_w != Screen.width || _h != Screen.height || _safe != Screen.safeArea)
                Apply();
        }

        [ContextMenu("Apply Now")]
        public void Apply()
        {
            if (cam == null) cam = Camera.main;
            if (boardRoot == null) boardRoot = transform;
            if (contentRoot == null) contentRoot = transform;
            if (cam == null) return;

            _w = Screen.width; _h = Screen.height; _safe = Screen.safeArea;

            var safe = GetSafeWorldRect(cam);
            float safeW = safe.width, safeH = safe.height;

            var localBounds = GetLocalBounds(contentRoot);
            float contentW = localBounds.size.x + 2f * marginX;
            float contentH = localBounds.size.y + 2f * marginY;

            float sX = safeW / Mathf.Max(0.0001f, contentW);
            float sY = safeH / Mathf.Max(0.0001f, contentH);
            float s = Mathf.Min(sX, sY);

            boardRoot.localScale = new Vector3(s, s, 1f);
            Vector3 safeCenter = new Vector3(safe.center.x, safe.center.y, boardRoot.position.z);
            boardRoot.position = safeCenter;
        }


        private static Rect GetSafeWorldRect(Camera cam)
        {
            Rect sa = Screen.safeArea;
            float vxMin = sa.xMin / Screen.width;
            float vxMax = sa.xMax / Screen.width;
            float vyMin = sa.yMin / Screen.height;
            float vyMax = sa.yMax / Screen.height;

            float zPlane = -cam.transform.position.z; 
            Vector3 bl = cam.ViewportToWorldPoint(new Vector3(vxMin, vyMin, zPlane));
            Vector3 tr = cam.ViewportToWorldPoint(new Vector3(vxMax, vyMax, zPlane));
            return Rect.MinMaxRect(bl.x, bl.y, tr.x, tr.y);
        }

        private static Bounds GetLocalBounds(Transform root)
        {
            bool hasAny = false;
            Bounds b = new Bounds(Vector3.zero, Vector3.zero);
            var children = root.GetComponentsInChildren<Transform>(true);

            foreach (var t in children)
            {
                if (t == root) continue;
                Vector3 lp = root.InverseTransformPoint(t.position);
                if (!hasAny) { b = new Bounds(lp, Vector3.zero); hasAny = true; }
                else { b.Encapsulate(lp); }
            }

            if (!hasAny) b = new Bounds(Vector3.zero, new Vector3(1f, 1f, 0f)); 
            if (b.size.y < 0.01f) b.Encapsulate(b.center + Vector3.up * 0.5f);
            return b;
        }
    }
}
