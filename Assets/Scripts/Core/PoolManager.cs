using System.Collections.Generic;
using UnityEngine;

namespace TankSurvival.Core
{
    public class PoolManager : MonoBehaviour
    {
        private readonly Dictionary<GameObject, Queue<GameObject>> _pools = new Dictionary<GameObject, Queue<GameObject>>();
        private readonly Dictionary<GameObject, GameObject> _instancePrefabs = new Dictionary<GameObject, GameObject>();
        private readonly Dictionary<GameObject, IPoolable[]> _instancePoolables = new Dictionary<GameObject, IPoolable[]>();

        public void Prewarm(GameObject prefab, int count)
        {
            if (prefab == null)
            {
                Debug.LogError("PoolManager: Prewarm got null prefab.", this);
                return;
            }

            Queue<GameObject> pool = GetOrCreatePool(prefab);
            for (int i = 0; i < count; i++)
            {
                GameObject instance = CreateInstance(prefab);
                instance.SetActive(false);
                pool.Enqueue(instance);
            }
        }

        public GameObject Get(GameObject prefab)
        {
            if (prefab == null)
            {
                Debug.LogError("PoolManager: Get got null prefab.", this);
                return null;
            }

            Queue<GameObject> pool = GetOrCreatePool(prefab);
            GameObject instance = pool.Count > 0 ? pool.Dequeue() : CreateInstance(prefab);
            instance.SetActive(true);

            IPoolable[] poolables = _instancePoolables[instance];
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnGetFromPool();
            }

            return instance;
        }

        public void Release(GameObject instance)
        {
            if (instance == null || !_instancePrefabs.TryGetValue(instance, out GameObject prefab))
            {
                Debug.LogError("PoolManager: Release got an instance not created by this pool.", instance);
                return;
            }

            IPoolable[] poolables = _instancePoolables[instance];
            for (int i = 0; i < poolables.Length; i++)
            {
                poolables[i].OnReleaseFromPool();
            }

            instance.SetActive(false);
            _pools[prefab].Enqueue(instance);
        }

        private Queue<GameObject> GetOrCreatePool(GameObject prefab)
        {
            if (!_pools.TryGetValue(prefab, out Queue<GameObject> pool))
            {
                pool = new Queue<GameObject>();
                _pools[prefab] = pool;
            }

            return pool;
        }

        private GameObject CreateInstance(GameObject prefab)
        {
            GameObject instance = Instantiate(prefab, transform);
            _instancePrefabs[instance] = prefab;
            _instancePoolables[instance] = instance.GetComponents<IPoolable>();
            return instance;
        }
    }
}
