#if UNITY_EDITOR || DEBUG
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Enemy;
using TankSurvival.Player;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace TankSurvival.Dev
{
    public class DebugOverlayController : MonoBehaviour
    {
        private const float OverlayWidth = 360f;
        private const float RefreshInterval = 0.25f;
        private const int OverlaySortingOrder = 90;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateOverlay()
        {
            GameObject overlay = new GameObject("DebugOverlay", typeof(RectTransform));

            Canvas canvas = overlay.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = OverlaySortingOrder;

            CanvasScaler scaler = overlay.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            overlay.AddComponent<DebugOverlayController>();
            overlay.AddComponent<GraphicRaycaster>();
        }

        private readonly StringBuilder _builder = new StringBuilder(512);
        private readonly List<DevParam> _enemyParams = new List<DevParam>();
        private readonly List<DevParam> _enemyHealthParams = new List<DevParam>();
        private readonly List<DevParam> _previewParams = new List<DevParam>();
        private readonly List<DevParam> _previewHealthParams = new List<DevParam>();

        private Font _font;
        private StatBlock _statBlock;
        private PlayerHealth _playerHealth;
        private Transform _playerTransform;
        private StatusEffectSystem _statusEffects;
        private ControlEffectSystem _controlEffects;
        private List<DevParam> _playerParams;
        private Text _playerText;
        private Text _enemyText;
        private Text _statusText;
        private EnemyController _observedEnemy;
        [SerializeField] private float hoverRadius = 2f;
        private UnityEngine.Camera _camera;
        private EnemyController _pinnedEnemy;
        private EnemyController _hoveredEnemy;
        private RectTransform _previewPanel;
        private Text _previewText;
        private EnemyController _observedPreviewEnemy;
        private Button _closeButton;
        private WaitForSecondsRealtime _refreshInterval;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            _statBlock = Object.FindAnyObjectByType<StatBlock>();
            _playerHealth = Object.FindAnyObjectByType<PlayerHealth>();
            _playerTransform = _statBlock != null
                ? _statBlock.transform
                : (_playerHealth != null ? _playerHealth.transform : null);

            _camera = UnityEngine.Camera.main != null ? UnityEngine.Camera.main : Object.FindAnyObjectByType<UnityEngine.Camera>();

            _statusEffects = GameServices.Get<StatusEffectSystem>();
            _controlEffects = GameServices.Get<ControlEffectSystem>();

            _playerParams = DevParamRegistry.Reflect(_statBlock, "Player");
            _refreshInterval = new WaitForSecondsRealtime(RefreshInterval);

            BuildOverlay();
            StartCoroutine(RefreshLoop());
        }

        private void Update()
        {
            if (_pinnedEnemy != null && !IsAlive(_pinnedEnemy))
                _pinnedEnemy = null;

            _hoveredEnemy = FindEnemyUnderCursor();

            Mouse mouse = Mouse.current;
            if (mouse == null)
                return;

            if (mouse.leftButton.wasPressedThisFrame)
                _pinnedEnemy = _hoveredEnemy;
        }

        private IEnumerator RefreshLoop()
        {
            while (true)
            {
                Refresh();

                yield return _refreshInterval;
            }
        }

        private void Refresh()
        {
            EnemyController main = MainEnemy();
            EnemyController preview = PreviewEnemy();

            ObserveEnemy(main);
            ObservePreviewEnemy(preview);

            _playerText.text = BuildPlayerText();
            _enemyText.text = BuildEnemyBlock("Враг", main, _enemyParams, _enemyHealthParams);
            _previewText.text = BuildEnemyBlock("Враг (preview)", preview, _previewParams, _previewHealthParams);
            _statusText.text = BuildStatusText();

            _previewPanel.gameObject.SetActive(preview != null);
            _closeButton.gameObject.SetActive(_pinnedEnemy != null);
        }

        private static bool IsAlive(EnemyController enemy)
        {
            return enemy != null
                && enemy.gameObject.activeSelf
                && enemy.health != null
                && enemy.health.currentHealth > 0f;
        }

        private EnemyController FindEnemyUnderCursor()
        {
            Mouse mouse = Mouse.current;
            if (mouse == null || _camera == null)
                return null;

            Ray ray = _camera.ScreenPointToRay(mouse.position.ReadValue());

            if (Mathf.Abs(ray.direction.y) < 0.0001f)
                return null;

            float t = -ray.origin.y / ray.direction.y;
            if (t < 0f)
                return null;

            Vector3 groundPoint = ray.origin + ray.direction * t;

            EnemyController best = null;
            float bestSqrDistance = hoverRadius * hoverRadius;

            List<EnemyController> active = EnemyController.Active;
            for (int i = 0; i < active.Count; i++)
            {
                EnemyController enemy = active[i];
                if (!IsAlive(enemy))
                    continue;

                float dx = enemy.transform.position.x - groundPoint.x;
                float dz = enemy.transform.position.z - groundPoint.z;
                float sqrDistance = dx * dx + dz * dz;
                if (sqrDistance < bestSqrDistance)
                {
                    bestSqrDistance = sqrDistance;
                    best = enemy;
                }
            }

            return best;
        }

        private EnemyController MainEnemy()
        {
            return _pinnedEnemy != null ? _pinnedEnemy : _hoveredEnemy;
        }

        private float DistanceToPlayer(EnemyController enemy)
        {
            if (enemy == null || _playerTransform == null)
                return 0f;

            return (enemy.transform.position - _playerTransform.position).magnitude;
        }

        private EnemyController PreviewEnemy()
        {
            if (_pinnedEnemy == null || _hoveredEnemy == null || _hoveredEnemy == _pinnedEnemy)
                return null;

            return _hoveredEnemy;
        }

        private void ObservePreviewEnemy(EnemyController enemy)
        {
            if (enemy == _observedPreviewEnemy)
                return;

            _observedPreviewEnemy = enemy;

            _previewParams.Clear();
            _previewParams.AddRange(DevParamRegistry.Reflect(enemy, "Enemy"));

            _previewHealthParams.Clear();
            _previewHealthParams.AddRange(DevParamRegistry.Reflect(enemy != null ? enemy.health : null, "Enemy.Health"));
        }

        private void ObserveEnemy(EnemyController enemy)
        {
            if (enemy == _observedEnemy)
                return;

            _observedEnemy = enemy;

            _enemyParams.Clear();
            _enemyParams.AddRange(DevParamRegistry.Reflect(enemy, "Enemy"));

            _enemyHealthParams.Clear();
            _enemyHealthParams.AddRange(DevParamRegistry.Reflect(enemy != null ? enemy.health : null, "Enemy.Health"));
        }

        private void BuildOverlay()
        {
            RectTransform panel = CreateRect("Panel", transform);
            panel.anchorMin = new Vector2(0f, 1f);
            panel.anchorMax = new Vector2(0f, 1f);
            panel.pivot = new Vector2(0f, 1f);
            panel.anchoredPosition = new Vector2(12f, -12f);
            panel.sizeDelta = new Vector2(OverlayWidth, 0f);

            Image background = panel.gameObject.AddComponent<Image>();
            background.color = new Color(0f, 0f, 0f, 0.55f);
            background.raycastTarget = false;

            VerticalLayoutGroup layout = panel.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 4f;
            layout.padding = new RectOffset(8, 8, 8, 8);

            ContentSizeFitter fitter = panel.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _playerText = CreateText("Player", panel);
            _enemyText = CreateText("Enemy", panel);
            _statusText = CreateText("Statuses", panel);

            RectTransform previewPanel = CreateRect("PreviewPanel", transform);
            previewPanel.anchorMin = new Vector2(0f, 1f);
            previewPanel.anchorMax = new Vector2(0f, 1f);
            previewPanel.pivot = new Vector2(0f, 1f);
            previewPanel.anchoredPosition = new Vector2(12f + OverlayWidth + 8f, -12f);
            previewPanel.sizeDelta = new Vector2(OverlayWidth, 0f);

            Image previewBackground = previewPanel.gameObject.AddComponent<Image>();
            previewBackground.color = new Color(0.05f, 0.08f, 0.12f, 0.6f);
            previewBackground.raycastTarget = false;

            VerticalLayoutGroup previewLayout = previewPanel.gameObject.AddComponent<VerticalLayoutGroup>();
            previewLayout.childControlWidth = true;
            previewLayout.childControlHeight = true;
            previewLayout.childForceExpandWidth = true;
            previewLayout.childForceExpandHeight = false;
            previewLayout.spacing = 4f;
            previewLayout.padding = new RectOffset(8, 8, 8, 8);

            ContentSizeFitter previewFitter = previewPanel.gameObject.AddComponent<ContentSizeFitter>();
            previewFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            previewFitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _previewText = CreateText("Preview", previewPanel);

            _previewPanel = previewPanel;
            _previewPanel.gameObject.SetActive(false);

            _closeButton = CreateCloseButton(panel);
            _closeButton.gameObject.SetActive(false);
        }

        private string BuildPlayerText()
        {
            _builder.Clear();
            _builder.Append("<b>Игрок</b>\n");
            AppendParams(_playerParams);
            _builder.Append("HP: ");
            _builder.Append(_playerHealth != null ? _playerHealth.CurrentHealth.ToString("0.##") : "-");
            _builder.Append(" / ");
            _builder.Append(_statBlock != null ? _statBlock.maxHealth.ToString("0.##") : "-");
            _builder.Append('\n');

            return _builder.ToString();
        }

        private string BuildEnemyBlock(string header, EnemyController enemy, List<DevParam> enemyParams, List<DevParam> healthParams)
        {
            _builder.Clear();
            _builder.Append("<b>").Append(header).Append("</b>\n");

            if (enemy == null)
            {
                _builder.Append("нет врага под курсором\n");
                return _builder.ToString();
            }

            AppendParams(enemyParams);
            AppendParams(healthParams);
            _builder.Append("Дистанция: ");
            _builder.Append(DistanceToPlayer(enemy).ToString("0.00"));
            _builder.Append('\n');

            _builder.Append("Скорость: ");
            _builder.Append(ActualSpeed(enemy).ToString("0.##"));
            _builder.Append('\n');

            _builder.Append("Burn остаток: ");
            _builder.Append(_statusEffects != null ? _statusEffects.GetBurnRemaining(enemy).ToString("0.##") : "0");
            _builder.Append('\n');
            _builder.Append("Freeze стаки: ");
            _builder.Append(_controlEffects != null ? _controlEffects.GetFreezeStacks(enemy) : 0);
            _builder.Append('\n');
            _builder.Append("Frozen: ");
            _builder.Append(_controlEffects != null && _controlEffects.IsFrozen(enemy) ? "да" : "нет");
            _builder.Append('\n');
            _builder.Append("Slow: ");
            _builder.Append(_controlEffects != null && _controlEffects.GetSlowRemaining(enemy) > 0f ? "да" : "нет");
            _builder.Append(" (");
            _builder.Append(_controlEffects != null ? _controlEffects.GetSlowAmount(enemy).ToString("0.##") : "0");
            _builder.Append(")\n");
            _builder.Append("Ground: ");
            _builder.Append(IsEnemyInGround(enemy) ? "да" : "нет");
            _builder.Append('\n');
            _builder.Append("Knockback: ");
            _builder.Append(enemy != null && enemy.KnockbackRemaining > 0f ? "да" : "нет");
            _builder.Append(" (");
            _builder.Append(enemy != null ? enemy.KnockbackRemaining.ToString("0.##") : "0");
            _builder.Append(")\n");

            return _builder.ToString();
        }

        private static float ActualSpeed(EnemyController enemy)
        {
            if (enemy == null || enemy.data == null)
                return 0f;

            return enemy.data.speed * enemy.speedMultiplier * enemy.controlSpeedMultiplier;
        }

        private static bool IsEnemyInGround(EnemyController enemy)
        {
            if (enemy == null)
                return false;

            for (int i = 0; i < GroundZone.Active.Count; i++)
            {
                GroundZone zone = GroundZone.Active[i];
                if (zone != null && zone.Contains(enemy))
                    return true;
            }

            return false;
        }

        private string BuildStatusText()
        {
            _builder.Clear();
            _builder.Append("<b>Статусы</b>\n");
            _builder.Append("Врагов активно: ");
            _builder.Append(EnemyController.Active.Count);
            _builder.Append('\n');
            _builder.Append("Burn активно: ");
            _builder.Append(_statusEffects != null ? _statusEffects.ActiveBurnCount : 0);
            _builder.Append('\n');

            return _builder.ToString();
        }

        private void AppendParams(List<DevParam> parameters)
        {
            for (int i = 0; i < parameters.Count; i++)
            {
                DevParam parameter = parameters[i];
                _builder.Append(parameter.Label);
                _builder.Append(": ");
                _builder.Append(FormatValue(parameter));
                _builder.Append('\n');
            }
        }

        private static string FormatValue(DevParam parameter)
        {
            float value = parameter.GetValue();

            return parameter.IsInteger ? Mathf.RoundToInt(value).ToString() : value.ToString("0.###");
        }

        private Text CreateText(string name, RectTransform parent)
        {
            RectTransform rect = CreateRect(name, parent);

            Text text = rect.gameObject.AddComponent<Text>();
            text.font = _font;
            text.fontSize = 13;
            text.color = Color.white;
            text.alignment = TextAnchor.UpperLeft;
            text.horizontalOverflow = HorizontalWrapMode.Wrap;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            text.text = string.Empty;

            return text;
        }

        private Button CreateCloseButton(RectTransform parent)
        {
            RectTransform rect = CreateRect("CloseButton", parent);

            LayoutElement ignore = rect.gameObject.AddComponent<LayoutElement>();
            ignore.ignoreLayout = true;

            rect.anchorMin = new Vector2(1f, 1f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = new Vector2(-4f, -4f);
            rect.sizeDelta = new Vector2(20f, 20f);

            Image background = rect.gameObject.AddComponent<Image>();
            background.color = new Color(0.8f, 0.2f, 0.2f, 0.9f);
            background.raycastTarget = true;

            Text label = CreateText("X", rect);
            label.text = "✕";
            label.fontSize = 12;
            label.alignment = TextAnchor.MiddleCenter;
            label.raycastTarget = false;
            label.rectTransform.anchorMin = Vector2.zero;
            label.rectTransform.anchorMax = Vector2.one;
            label.rectTransform.offsetMin = Vector2.zero;
            label.rectTransform.offsetMax = Vector2.zero;

            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = background;
            button.onClick.AddListener(Unpin);

            return button;
        }

        private void Unpin()
        {
            _pinnedEnemy = null;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);

            return rect;
        }
    }
}
#endif
