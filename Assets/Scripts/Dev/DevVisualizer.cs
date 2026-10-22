#if UNITY_EDITOR || DEBUG
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace TankSurvival.Dev
{
    public class DevVisualizer : MonoBehaviour
    {
        private const int BeamInitial = 8;
        private const int BeamMax = 32;
        private const int SectorInitial = 4;
        private const int SectorMax = 16;
        private const int CircleInitial = 16;
        private const int CircleMax = 64;

        private const int SectorSegments = 32;
        private const int CircleSegments = 64;

        private const float BeamWidth = 0.08f;
        private const float CircleWidth = 0.06f;
        private const float MinDirectionSqr = 0.0001f;
        private const float MinMarkerRadius = 0.01f;

        private static readonly int ColorId = Shader.PropertyToID("_Color");

        private static DevVisualizer _instance;
        private static bool _enabled = true;

        public static bool Enabled
        {
            get => _enabled;
            set
            {
                _enabled = value;

                if (!_enabled && _instance != null)
                    _instance.HideAll();
            }
        }

        private readonly List<BeamMarker> _beams = new List<BeamMarker>();
        private readonly List<SectorMarker> _sectors = new List<SectorMarker>();
        private readonly List<CircleMarker> _circles = new List<CircleMarker>();

        private Material _markerMaterial;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateVisualizer()
        {
            if (_instance != null)
                return;

            GameObject host = new GameObject("DevVisualizer");
            _instance = host.AddComponent<DevVisualizer>();
        }

        public static void ShowBeam(Vector3 from, Vector3 to, float seconds, Color color)
        {
            if (!_enabled || _instance == null)
                return;

            BeamMarker marker = _instance.Acquire(_instance._beams, BeamMax);
            marker.Show(from, to, color, Time.time + seconds);
        }

        public static void ShowSector(Vector3 origin, Vector3 flatDir, float angle, float range, float seconds, Color color)
        {
            if (!_enabled || _instance == null)
                return;

            SectorMarker marker = _instance.Acquire(_instance._sectors, SectorMax);
            marker.Show(origin, flatDir, angle, range, color, Time.time + seconds);
        }

        public static void ShowCircle(Vector3 center, float radius, float seconds, Color color)
        {
            if (!_enabled || _instance == null)
                return;

            CircleMarker marker = _instance.Acquire(_instance._circles, CircleMax);
            marker.Show(center, radius, color, Time.time + seconds);
        }

        private void Awake()
        {
            _markerMaterial = CreateMarkerMaterial();

            Prewarm(_beams, BeamInitial);
            Prewarm(_sectors, SectorInitial);
            Prewarm(_circles, CircleInitial);
        }

        private void Prewarm<T>(List<T> pool, int count) where T : Marker, new()
        {
            for (int i = 0; i < count; i++)
            {
                T marker = new T();
                marker.Build(transform, _markerMaterial);
                pool.Add(marker);
            }
        }

        private T Acquire<T>(List<T> pool, int max) where T : Marker, new()
        {
            T oldest = null;

            for (int i = 0; i < pool.Count; i++)
            {
                T marker = pool[i];
                if (marker.IsFree)
                {
                    marker.shownTime = Time.time;
                    return marker;
                }

                if (oldest == null || marker.shownTime < oldest.shownTime)
                    oldest = marker;
            }

            if (pool.Count < max)
            {
                T marker = new T();
                marker.Build(transform, _markerMaterial);
                pool.Add(marker);
                marker.shownTime = Time.time;

                return marker;
            }

            oldest.shownTime = Time.time;
            return oldest;
        }

        private void Update()
        {
            float now = Time.time;

            Expire(_beams, now);
            Expire(_sectors, now);
            Expire(_circles, now);
        }

        private static void Expire<T>(List<T> pool, float now) where T : Marker
        {
            for (int i = 0; i < pool.Count; i++)
            {
                T marker = pool[i];
                if (!marker.IsFree && now >= marker.expireTime)
                    marker.Hide();
            }
        }

        private void HideAll()
        {
            HideAll(_beams);
            HideAll(_sectors);
            HideAll(_circles);
        }

        private static void HideAll<T>(List<T> pool) where T : Marker
        {
            for (int i = 0; i < pool.Count; i++)
            {
                pool[i].Hide();
            }
        }

        private static Material CreateMarkerMaterial()
        {
            Material material = new Material(FindMarkerShader());
            material.color = Color.white;

            return material;
        }

        private static Shader FindMarkerShader()
        {
            Shader shader = Shader.Find("Sprites/Default");
            if (shader == null)
                shader = Shader.Find("Unlit/Color");

            return shader;
        }

        private static Vector3 FlatDirection(Vector3 flatDir)
        {
            Vector3 direction = new Vector3(flatDir.x, 0f, flatDir.z);

            return direction.sqrMagnitude > MinDirectionSqr ? direction.normalized : Vector3.forward;
        }

        private abstract class Marker
        {
            public GameObject gameObject;
            public float shownTime;
            public float expireTime;

            public bool IsFree => !gameObject.activeSelf;

            public abstract void Build(Transform parent, Material material);

            public void Hide()
            {
                gameObject.SetActive(false);
            }
        }

        private sealed class BeamMarker : Marker
        {
            private readonly Vector3[] _points = new Vector3[2];

            private LineRenderer _line;

            public override void Build(Transform parent, Material material)
            {
                gameObject = new GameObject("DevBeam");
                gameObject.transform.SetParent(parent, false);

                _line = gameObject.AddComponent<LineRenderer>();
                _line.useWorldSpace = false;
                _line.positionCount = _points.Length;
                _line.sharedMaterial = material;
                _line.startWidth = BeamWidth;
                _line.endWidth = BeamWidth;
                _line.numCapVertices = 0;
                _line.textureMode = LineTextureMode.Stretch;
                _line.alignment = LineAlignment.View;
                _line.shadowCastingMode = ShadowCastingMode.Off;
                _line.receiveShadows = false;

                gameObject.SetActive(false);
            }

            public void Show(Vector3 from, Vector3 to, Color color, float expireTime)
            {
                gameObject.transform.position = from;

                _points[0] = Vector3.zero;
                _points[1] = to - from;
                _line.SetPositions(_points);

                _line.startColor = color;
                _line.endColor = color;

                this.expireTime = expireTime;
                gameObject.SetActive(true);
            }
        }

        private sealed class SectorMarker : Marker
        {
            private readonly Vector3[] _vertices = new Vector3[SectorSegments + 2];

            private Mesh _mesh;
            private MeshRenderer _renderer;
            private MaterialPropertyBlock _block;
            private float _lastAngle;
            private bool _hasAngle;

            public override void Build(Transform parent, Material material)
            {
                gameObject = new GameObject("DevSector");
                gameObject.transform.SetParent(parent, false);

                _mesh = new Mesh();
                _mesh.MarkDynamic();

                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                filter.sharedMesh = _mesh;

                _renderer = gameObject.AddComponent<MeshRenderer>();
                _renderer.sharedMaterial = material;
                _renderer.shadowCastingMode = ShadowCastingMode.Off;
                _renderer.receiveShadows = false;

                _block = new MaterialPropertyBlock();

                int[] triangles = new int[SectorSegments * 3];
                for (int i = 0; i < SectorSegments; i++)
                {
                    triangles[i * 3] = 0;
                    triangles[i * 3 + 1] = i + 1;
                    triangles[i * 3 + 2] = i + 2;
                }

                _mesh.vertices = _vertices;
                _mesh.triangles = triangles;
                _mesh.RecalculateNormals();

                gameObject.SetActive(false);
            }

            public void Show(Vector3 origin, Vector3 flatDir, float angle, float range, Color color, float expireTime)
            {
                if (!_hasAngle || angle != _lastAngle)
                {
                    _lastAngle = angle;
                    _hasAngle = true;
                    RebuildArc(angle);
                }

                gameObject.transform.position = origin;
                gameObject.transform.rotation = Quaternion.LookRotation(FlatDirection(flatDir));
                gameObject.transform.localScale = new Vector3(range, 1f, range);

                _block.SetColor(ColorId, color);
                _renderer.SetPropertyBlock(_block);

                this.expireTime = expireTime;
                gameObject.SetActive(true);
            }

            private void RebuildArc(float angle)
            {
                float startAngle = angle * -0.5f;
                float step = angle / SectorSegments;

                _vertices[0] = Vector3.zero;

                for (int i = 0; i <= SectorSegments; i++)
                {
                    float radians = (startAngle + step * i) * Mathf.Deg2Rad;
                    _vertices[i + 1] = new Vector3(Mathf.Sin(radians), 0f, Mathf.Cos(radians));
                }

                _mesh.vertices = _vertices;
            }
        }

        private sealed class CircleMarker : Marker
        {
            private readonly Vector3[] _points = new Vector3[CircleSegments];

            private LineRenderer _line;

            public override void Build(Transform parent, Material material)
            {
                gameObject = new GameObject("DevCircle");
                gameObject.transform.SetParent(parent, false);

                _line = gameObject.AddComponent<LineRenderer>();
                _line.useWorldSpace = false;
                _line.loop = true;
                _line.positionCount = _points.Length;
                _line.sharedMaterial = material;
                _line.startWidth = CircleWidth;
                _line.endWidth = CircleWidth;
                _line.numCapVertices = 0;
                _line.textureMode = LineTextureMode.Stretch;
                _line.alignment = LineAlignment.View;
                _line.shadowCastingMode = ShadowCastingMode.Off;
                _line.receiveShadows = false;

                for (int i = 0; i < _points.Length; i++)
                {
                    float radians = 2f * Mathf.PI * i / _points.Length;
                    _points[i] = new Vector3(Mathf.Cos(radians), 0f, Mathf.Sin(radians));
                }

                _line.SetPositions(_points);

                gameObject.SetActive(false);
            }

            public void Show(Vector3 center, float radius, Color color, float expireTime)
            {
                gameObject.transform.position = center;
                gameObject.transform.rotation = Quaternion.identity;
                gameObject.transform.localScale = new Vector3(radius, 1f, radius);

                _line.widthMultiplier = 1f / Mathf.Max(radius, MinMarkerRadius);
                _line.startColor = color;
                _line.endColor = color;

                this.expireTime = expireTime;
                gameObject.SetActive(true);
            }
        }
    }
}
#endif
