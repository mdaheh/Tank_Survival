#if UNITY_EDITOR || DEBUG
using System.Collections;
using System.Collections.Generic;
using System.Text;
using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Enemy;
using TankSurvival.Player;
using UnityEngine;
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
        }

        private readonly StringBuilder _builder = new StringBuilder(512);
        private readonly List<DevParam> _enemyParams = new List<DevParam>();
        private readonly List<DevParam> _enemyHealthParams = new List<DevParam>();

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
        private float _nearestDistance;
        private WaitForSecondsRealtime _refreshInterval;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            _statBlock = Object.FindAnyObjectByType<StatBlock>();
            _playerHealth = Object.FindAnyObjectByType<PlayerHealth>();
            _playerTransform = _statBlock != null
                ? _statBlock.transform
                : (_playerHealth != null ? _playerHealth.transform : null);

            _statusEffects = GameServices.Get<StatusEffectSystem>();
            _controlEffects = GameServices.Get<ControlEffectSystem>();

            _playerParams = DevParamRegistry.Reflect(_statBlock, "Player");
            _refreshInterval = new WaitForSecondsRealtime(RefreshInterval);

            BuildOverlay();
            StartCoroutine(RefreshLoop());
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
            ObserveEnemy(FindNearestEnemy());

            _playerText.text = BuildPlayerText();
            _enemyText.text = BuildEnemyText();
            _statusText.text = BuildStatusText();
        }

        private EnemyController FindNearestEnemy()
        {
            _nearestDistance = 0f;

            if (_playerTransform == null)
                return null;

            Vector3 playerPosition = _playerTransform.position;
            EnemyController nearest = null;
            float nearestSqrDistance = float.MaxValue;

            List<EnemyController> active = EnemyController.Active;
            for (int i = 0; i < active.Count; i++)
            {
                EnemyController enemy = active[i];
                if (enemy == null || !enemy.gameObject.activeSelf)
                    continue;

                float sqrDistance = (enemy.transform.position - playerPosition).sqrMagnitude;
                if (sqrDistance < nearestSqrDistance)
                {
                    nearestSqrDistance = sqrDistance;
                    nearest = enemy;
                }
            }

            if (nearest != null)
                _nearestDistance = Mathf.Sqrt(nearestSqrDistance);

            return nearest;
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

        private string BuildEnemyText()
        {
            _builder.Clear();
            _builder.Append("<b>Враг</b>\n");

            if (_observedEnemy == null)
            {
                _builder.Append("нет активных врагов\n");
                return _builder.ToString();
            }

            AppendParams(_enemyParams);
            AppendParams(_enemyHealthParams);
            _builder.Append("Дистанция: ");
            _builder.Append(_nearestDistance.ToString("0.00"));
            _builder.Append('\n');

            return _builder.ToString();
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
            _builder.Append("Burn остаток: ");
            _builder.Append(_statusEffects != null && _observedEnemy != null
                ? _statusEffects.GetBurnRemaining(_observedEnemy).ToString("0.00")
                : "0.00");
            _builder.Append('\n');
            _builder.Append("Freeze стаки: ");
            _builder.Append(_controlEffects != null && _observedEnemy != null ? _controlEffects.GetFreezeStacks(_observedEnemy) : 0);
            _builder.Append('\n');
            _builder.Append("Frozen: ");
            _builder.Append(_controlEffects != null && _observedEnemy != null && _controlEffects.IsFrozen(_observedEnemy) ? "да" : "нет");
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
