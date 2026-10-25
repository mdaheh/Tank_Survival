#if UNITY_EDITOR || DEBUG
using System.Collections.Generic;
using System.Globalization;
using TankSurvival.Round;
using UnityEngine;
using UnityEngine.UI;

namespace TankSurvival.Dev
{
    public class DevMenuController : MonoBehaviour
    {
        private const float PanelWidth = 470f;
        private const float RowHeight = 46f;
        private const float GroupHeaderHeight = 26f;

        private Font _font;
        private RectTransform _panel;
        private RoundController _roundController;

        private void Awake()
        {
            _font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

            DevParamRegistry.Init();
            BuildPanel();
            _roundController = Object.FindFirstObjectByType<RoundController>();
            SetVisible(true);
        }

        private void BuildPanel()
        {
            _panel = CreateRect("Panel", transform);
            _panel.anchorMin = new Vector2(0f, 0f);
            _panel.anchorMax = new Vector2(0f, 1f);
            _panel.pivot = new Vector2(0f, 0.5f);
            _panel.anchoredPosition = new Vector2(12f, 0f);
            _panel.sizeDelta = new Vector2(PanelWidth, -24f);

            Image background = _panel.gameObject.AddComponent<Image>();
            background.color = new Color(0.06f, 0.07f, 0.09f, 0.93f);

            Text title = CreateText("Title", _panel, "DEV MENU");
            RectTransform titleRect = title.rectTransform;
            titleRect.anchorMin = new Vector2(0f, 1f);
            titleRect.anchorMax = new Vector2(1f, 1f);
            titleRect.pivot = new Vector2(0.5f, 1f);
            titleRect.sizeDelta = new Vector2(-60f, 28f);
            titleRect.anchoredPosition = new Vector2(-30f, -6f);

            RectTransform content = BuildScroll(_panel);
            CreateMarkersToggle(content);
            BuildRows(content);

            Button start = CreateButton("StartRound", _panel, "Start Round");
            RectTransform startRect = (RectTransform)start.transform;
            startRect.anchorMin = new Vector2(1f, 0f);
            startRect.anchorMax = new Vector2(1f, 0f);
            startRect.pivot = new Vector2(1f, 0f);
            startRect.anchoredPosition = new Vector2(-8f, 8f);
            startRect.sizeDelta = new Vector2(150f, 32f);
            start.onClick.AddListener(StartRound);
        }

        private RectTransform BuildScroll(RectTransform parent)
        {
            RectTransform scrollRect = CreateRect("Scroll", parent);
            scrollRect.anchorMin = Vector2.zero;
            scrollRect.anchorMax = Vector2.one;
            scrollRect.offsetMin = new Vector2(8f, 8f);
            scrollRect.offsetMax = new Vector2(-8f, -48f);

            RectTransform viewport = CreateRect("Viewport", scrollRect);
            Stretch(viewport, Vector2.zero, Vector2.zero);
            viewport.gameObject.AddComponent<RectMask2D>();

            RectTransform content = CreateRect("Content", viewport);
            content.anchorMin = new Vector2(0f, 1f);
            content.anchorMax = new Vector2(1f, 1f);
            content.pivot = new Vector2(0.5f, 1f);
            content.anchoredPosition = Vector2.zero;
            content.sizeDelta = Vector2.zero;

            VerticalLayoutGroup layout = content.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.childControlWidth = true;
            layout.childControlHeight = true;
            layout.childForceExpandWidth = true;
            layout.childForceExpandHeight = false;
            layout.spacing = 2f;
            layout.padding = new RectOffset(4, 4, 4, 4);

            ContentSizeFitter fitter = content.gameObject.AddComponent<ContentSizeFitter>();
            fitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
            fitter.verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            ScrollRect scroll = scrollRect.gameObject.AddComponent<ScrollRect>();
            scroll.horizontal = false;
            scroll.vertical = true;
            scroll.movementType = ScrollRect.MovementType.Clamped;
            scroll.scrollSensitivity = 30f;
            scroll.viewport = viewport;
            scroll.content = content;

            return content;
        }

        private void CreateMarkersToggle(RectTransform content)
        {
            RectTransform row = CreateRect("Markers", content);
            LayoutElement layout = row.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = RowHeight;
            layout.preferredHeight = RowHeight;

            Toggle toggle = CreateToggle(row);
            toggle.isOn = DevVisualizer.Enabled;
            toggle.onValueChanged.AddListener(OnMarkersToggled);

            Text label = CreateText("Label", row, "Маркеры");
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = new Vector2(34f, 0f);
            labelRect.offsetMax = new Vector2(-6f, 0f);
        }

        private Toggle CreateToggle(RectTransform parent)
        {
            RectTransform rect = CreateRect("Toggle", parent);
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0f, 0.5f);
            rect.anchoredPosition = new Vector2(8f, 0f);
            rect.sizeDelta = new Vector2(20f, 20f);

            Image background = rect.gameObject.AddComponent<Image>();
            background.color = new Color(0.16f, 0.17f, 0.2f, 1f);

            RectTransform checkRect = CreateRect("Checkmark", rect);
            Stretch(checkRect, new Vector2(3f, 3f), new Vector2(-3f, -3f));

            Image checkmark = checkRect.gameObject.AddComponent<Image>();
            checkmark.color = new Color(0.25f, 0.75f, 0.45f, 1f);

            Toggle toggle = rect.gameObject.AddComponent<Toggle>();
            toggle.targetGraphic = background;
            toggle.graphic = checkmark;

            return toggle;
        }

        private static void OnMarkersToggled(bool value)
        {
            DevVisualizer.Enabled = value;
        }

        private static string GetCategory(string group)
        {
            if (group.StartsWith("ChassisData")) return "Chassis";
            if (group.EndsWith(".delivery")) return "Delivery";
            if (group.StartsWith("TurretData")) return "Turret";
            if (group.StartsWith("OnHitModuleData")) return "OnHit";
            if (group.StartsWith("DifficultyData")) return "Difficulty";
            if (group.StartsWith("EnemyData")) return "Enemy";
            return group;
        }

        private void BuildRows(RectTransform content)
        {
            List<DevParam> parameters = DevParamRegistry.All;
            string currentCategory = null;

            for (int i = 0; i < parameters.Count; i++)
            {
                DevParam parameter = parameters[i];
                string category = GetCategory(parameter.Group);

                if (category != currentCategory)
                {
                    currentCategory = category;
                    CreateSectionHeader(content, category);
                }

                CreateRow(content, parameter);
            }
        }

        private void CreateSectionHeader(RectTransform content, string category)
        {
            RectTransform header = CreateRect("Section", content);
            LayoutElement layout = header.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = GroupHeaderHeight;
            layout.preferredHeight = GroupHeaderHeight;

            Image background = header.gameObject.AddComponent<Image>();
            background.color = new Color(0.14f, 0.20f, 0.28f, 1f);

            Text label = CreateText("Label", header, category);
            label.fontStyle = FontStyle.Bold;
            label.color = new Color(0.75f, 0.9f, 1f, 1f);
            Stretch(label.rectTransform, new Vector2(8f, 0f), new Vector2(-8f, 0f));
        }

        private void CreateRow(RectTransform content, DevParam parameter)
        {
            RectTransform row = CreateRect("Row", content);
            row.anchorMin = new Vector2(0f, 1f);
            row.anchorMax = new Vector2(1f, 1f);
            row.pivot = new Vector2(0.5f, 1f);
            row.sizeDelta = new Vector2(0f, RowHeight);

            LayoutElement layout = row.gameObject.AddComponent<LayoutElement>();
            layout.minHeight = RowHeight;
            layout.preferredHeight = RowHeight;

            Text label = CreateText("Label", row, parameter.Label);
            RectTransform labelRect = label.rectTransform;
            labelRect.anchorMin = new Vector2(0f, 0f);
            labelRect.anchorMax = new Vector2(0.42f, 1f);
            labelRect.offsetMin = new Vector2(6f, 0f);
            labelRect.offsetMax = Vector2.zero;

            Slider slider = CreateSlider(row, parameter);

            InputField inputField = CreateInputField(row, parameter);
            RectTransform inputRect = (RectTransform)inputField.transform;
            inputRect.anchorMin = new Vector2(0.78f, 0f);
            inputRect.anchorMax = new Vector2(1f, 1f);
            inputRect.offsetMin = new Vector2(6f, 0f);
            inputRect.offsetMax = new Vector2(-8f, 0f);

            slider.onValueChanged.AddListener(newValue => OnSliderChanged(parameter, newValue, inputField));
            inputField.onEndEdit.AddListener(text => OnInputCommitted(parameter, text, slider, inputField));
        }

        private static Slider CreateSlider(RectTransform parent, DevParam parameter)
        {
            RectTransform sliderRect = CreateRect("Slider", parent);
            sliderRect.anchorMin = new Vector2(0.42f, 0.5f);
            sliderRect.anchorMax = new Vector2(0.78f, 0.5f);
            sliderRect.pivot = new Vector2(0.5f, 0.5f);
            sliderRect.anchoredPosition = Vector2.zero;
            sliderRect.sizeDelta = new Vector2(-8f, 18f);

            Slider slider = sliderRect.gameObject.AddComponent<Slider>();
            slider.direction = Slider.Direction.LeftToRight;
            slider.wholeNumbers = parameter.IsInteger;
            slider.minValue = parameter.Min;
            slider.maxValue = parameter.Max;

            Image background = CreateImage("Background", sliderRect, new Color(0.16f, 0.17f, 0.2f, 1f));
            Stretch(background.rectTransform, new Vector2(0f, 6f), new Vector2(0f, -6f));

            RectTransform fillArea = CreateRect("Fill Area", sliderRect);
            Stretch(fillArea, new Vector2(9f, 6f), new Vector2(-9f, -6f));

            Image fill = CreateImage("Fill", fillArea, new Color(0.25f, 0.75f, 0.45f, 1f));
            Stretch(fill.rectTransform, Vector2.zero, Vector2.zero);

            RectTransform handleArea = CreateRect("Handle Slide Area", sliderRect);
            Stretch(handleArea, new Vector2(9f, 0f), new Vector2(-9f, 0f));

            Image handle = CreateImage("Handle", handleArea, new Color(0.9f, 0.9f, 0.92f, 1f));
            RectTransform handleRect = handle.rectTransform;
            handleRect.anchorMin = new Vector2(0f, 0f);
            handleRect.anchorMax = new Vector2(0f, 1f);
            handleRect.pivot = new Vector2(0.5f, 0.5f);
            handleRect.anchoredPosition = Vector2.zero;
            handleRect.sizeDelta = new Vector2(14f, 0f);

            slider.fillRect = fill.rectTransform;
            slider.handleRect = handleRect;
            slider.targetGraphic = handle;
            slider.value = parameter.GetValue();

            return slider;
        }

        private InputField CreateInputField(RectTransform parent, DevParam parameter)
        {
            RectTransform rect = CreateRect("Input", parent);

            Image background = rect.gameObject.AddComponent<Image>();
            background.color = new Color(0.12f, 0.14f, 0.17f, 1f);

            RectTransform textRect = CreateRect("Text", rect);
            Stretch(textRect, new Vector2(4f, 0f), new Vector2(-4f, 0f));
            Text text = textRect.gameObject.AddComponent<Text>();
            text.font = _font;
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleRight;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.supportRichText = false;
            text.raycastTarget = true;

            InputField inputField = rect.gameObject.AddComponent<InputField>();
            inputField.textComponent = text;
            inputField.targetGraphic = background;
            inputField.text = FormatValue(parameter);

            return inputField;
        }

        private Text CreateText(string name, RectTransform parent, string value)
        {
            RectTransform rect = CreateRect(name, parent);

            Text text = rect.gameObject.AddComponent<Text>();
            text.font = _font;
            text.fontSize = 14;
            text.color = Color.white;
            text.alignment = TextAnchor.MiddleLeft;
            text.horizontalOverflow = HorizontalWrapMode.Overflow;
            text.verticalOverflow = VerticalWrapMode.Overflow;
            text.raycastTarget = false;
            text.text = value;

            return text;
        }

        private Button CreateButton(string name, RectTransform parent, string value)
        {
            RectTransform rect = CreateRect(name, parent);

            Image image = rect.gameObject.AddComponent<Image>();
            image.color = new Color(0.28f, 0.3f, 0.34f, 1f);

            Button button = rect.gameObject.AddComponent<Button>();
            button.targetGraphic = image;

            Text label = CreateText("Label", rect, value);
            label.alignment = TextAnchor.MiddleCenter;
            Stretch(label.rectTransform, Vector2.zero, Vector2.zero);

            return button;
        }

        private static Image CreateImage(string name, RectTransform parent, Color color)
        {
            RectTransform rect = CreateRect(name, parent);

            Image image = rect.gameObject.AddComponent<Image>();
            image.color = color;

            return image;
        }

        private static RectTransform CreateRect(string name, Transform parent)
        {
            GameObject go = new GameObject(name, typeof(RectTransform));
            RectTransform rect = (RectTransform)go.transform;
            rect.SetParent(parent, false);

            return rect;
        }

        private static void Stretch(RectTransform rect, Vector2 offsetMin, Vector2 offsetMax)
        {
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = offsetMin;
            rect.offsetMax = offsetMax;
        }

        private static void OnSliderChanged(DevParam parameter, float value, InputField inputField)
        {
            parameter.SetValue(value);
            inputField.SetTextWithoutNotify(FormatValue(parameter));
        }

        private static void OnInputCommitted(DevParam parameter, string text, Slider slider, InputField inputField)
        {
            if (parameter.IsInteger)
            {
                if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out int iv))
                    parameter.SetValue(iv);
            }
            else
            {
                if (float.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out float fv))
                    parameter.SetValue(fv);
            }

            slider.SetValueWithoutNotify(Mathf.Clamp(parameter.GetValue(), slider.minValue, slider.maxValue));
            inputField.SetTextWithoutNotify(FormatValue(parameter));
        }

        private static string FormatValue(DevParam parameter)
        {
            float value = parameter.GetValue();
            return parameter.IsInteger
                ? Mathf.RoundToInt(value).ToString(CultureInfo.InvariantCulture)
                : value.ToString("0.###", CultureInfo.InvariantCulture);
        }

        private void SetVisible(bool visible)
        {
            _panel.gameObject.SetActive(visible);
        }

        private void StartRound()
        {
            if (_roundController != null)
                _roundController.StartRound();
            SetVisible(false);
        }
    }
}
#endif
