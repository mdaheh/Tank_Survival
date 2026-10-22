#if UNITY_EDITOR || DEBUG
using UnityEngine;
using UnityEngine.UI;

namespace TankSurvival.Dev
{
    public static class DevBootstrap
    {
        private const int DevCanvasSortingOrder = 100;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void CreateDevMenu()
        {
            GameObject menu = new GameObject("DevMenu", typeof(RectTransform));

            Canvas canvas = menu.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = DevCanvasSortingOrder;

            CanvasScaler scaler = menu.AddComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920f, 1080f);
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;

            menu.AddComponent<GraphicRaycaster>();
            menu.AddComponent<DevMenuController>();
        }
    }
}
#endif
