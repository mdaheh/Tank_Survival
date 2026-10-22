using System.Collections.Generic;
using TankSurvival.Combat;
using TankSurvival.Core;
using TankSurvival.Enemy;
using TankSurvival.Player;
using TankSurvival.Progression;
using TankSurvival.Round;
using TankSurvival.UI;
using TankSurvival.Waves;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using EditorSceneManager = UnityEditor.SceneManagement.EditorSceneManager;
using MenuItem = UnityEditor.MenuItem;
using Undo = UnityEditor.Undo;

namespace TankSurvival.Editor
{
    public static class SceneOrganizer
    {
        private const string UndoName = "Organize Scene Hierarchy";

        private const string ManagersHeader = "_Managers";
        private const string PlayerHeader = "_Player";
        private const string CombatHeader = "_Combat";
        private const string SpawnPointsHeader = "_SpawnPoints";
        private const string UiHeader = "_UI";
        private const string DevHeader = "_Dev";

        private static readonly string[] HeaderNames =
        {
            ManagersHeader,
            PlayerHeader,
            CombatHeader,
            SpawnPointsHeader,
            UiHeader,
            DevHeader
        };

        private static readonly System.Type[] ManagerComponents =
        {
            typeof(GameBootstrap),
            typeof(PoolManager),
            typeof(SessionState),
            typeof(RoundController),
            typeof(WaveSpawner),
            typeof(EnemyKillSystem),
            typeof(ProgressionSystem),
            typeof(StatBlock)
        };

        private static readonly System.Type[] CombatComponents =
        {
            typeof(StatusEffectSystem),
            typeof(ControlEffectSystem),
            typeof(SlowOnHitHandler),
            typeof(FreezeOnHitHandler),
            typeof(BurnOnHitHandler),
            typeof(KnockbackOnHitHandler),
            typeof(ExplosionOnHitHandler),
            typeof(GroundOnHitHandler)
        };

        private static readonly System.Type[] UiComponents =
        {
            typeof(HUDController),
            typeof(LevelUpUI),
            typeof(RoundEndUI),
            typeof(Canvas),
            typeof(EventSystem)
        };

        [MenuItem("Tools/Dev/Organize Scene Hierarchy")]
        public static void Organize()
        {
            Scene scene = SceneManager.GetActiveScene();
            GameObject[] roots = scene.GetRootGameObjects();

            Dictionary<string, Transform> headers = GetOrCreateHeaders(roots);

            for (int i = 0; i < roots.Length; i++)
            {
                Transform root = roots[i].transform;
                if (root.parent != null)
                    continue;

                string header = GetHeader(root.gameObject);
                if (header == null)
                    continue;

                Undo.SetTransformParent(root, headers[header], UndoName);
            }

            EditorSceneManager.MarkSceneDirty(scene);
        }

        private static Dictionary<string, Transform> GetOrCreateHeaders(GameObject[] roots)
        {
            Dictionary<string, Transform> headers = new Dictionary<string, Transform>();

            for (int i = 0; i < HeaderNames.Length; i++)
            {
                string name = HeaderNames[i];
                headers[name] = FindRoot(roots, name);
            }

            for (int i = 0; i < HeaderNames.Length; i++)
            {
                string name = HeaderNames[i];
                if (headers[name] != null)
                    continue;

                GameObject header = new GameObject(name);
                Undo.RegisterCreatedObjectUndo(header, UndoName);
                headers[name] = header.transform;
            }

            return headers;
        }

        private static Transform FindRoot(GameObject[] roots, string name)
        {
            for (int i = 0; i < roots.Length; i++)
            {
                if (roots[i].name == name)
                    return roots[i].transform;
            }

            return null;
        }

        private static string GetHeader(GameObject root)
        {
            if (HasComponent(root, ManagerComponents))
                return ManagersHeader;

            if (root.GetComponent<PlayerController>() != null)
                return PlayerHeader;

            if (HasComponent(root, CombatComponents))
                return CombatHeader;

            if (root.GetComponent<SpawnPoint>() != null)
                return SpawnPointsHeader;

            if (HasComponent(root, UiComponents))
                return UiHeader;

            if (HasDevComponent(root))
                return DevHeader;

            return null;
        }

        private static bool HasComponent(GameObject root, System.Type[] components)
        {
            for (int i = 0; i < components.Length; i++)
            {
                if (root.GetComponent(components[i]) != null)
                    return true;
            }

            return false;
        }

        private static bool HasDevComponent(GameObject root)
        {
            Component[] components = root.GetComponents<Component>();

            for (int i = 0; i < components.Length; i++)
            {
                Component component = components[i];
                if (component != null && component.GetType().Name.StartsWith("Dev", System.StringComparison.Ordinal))
                    return true;
            }

            return false;
        }
    }
}
