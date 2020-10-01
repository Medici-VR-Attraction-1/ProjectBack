/// <summary>
/// 1. Use Static Method SetupObjectPoolItemByTargetPrefab Target Prefab Game Object
/// 2. Get Object By Static Method GetObjectItemAtPool
/// 3. Set Position and Rotation, SetActive(true) to Start Action
/// 4. Just SetActive(false) and Object will Return To Pool Automatically
/// </summary>

using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    // Inner Class For Object Pool Item Callbacks
    public class ObjectPoolItem : MonoBehaviour
    {
        private GameObject _originalPrefab;
        public GameObject InstanceType
        {
            get { return _originalPrefab; }
            set { _originalPrefab = value; }
        }

        private void OnDisable()
        {
            ObjectPool.ReturnObjectToPool(this);
        }
    }

    private static Dictionary<int, Queue<GameObject>>
        _objectPoolHash = new Dictionary<int, Queue<GameObject>>();

    private static Dictionary<int, int>
        _objectPoolMaxSize = new Dictionary<int, int>();

    // If pool item disabled, return item to pool .
    public static void ReturnObjectToPool(ObjectPoolItem expiredObject)
    {
        int objectTypeKey = expiredObject.InstanceType.GetInstanceID();
        _objectPoolHash[objectTypeKey].Enqueue(expiredObject.gameObject);
    }

    // Return object item at pool. If pool is empty, instantiate more item to use.
    public static GameObject GetObjectItemAtPool(GameObject targetPrefab)
    {
        int objectTypeKey = targetPrefab.GetInstanceID();
        if (_objectPoolHash[objectTypeKey].Count == 0)
        {
            SetupObjectPoolItemByTargetPrefab(targetPrefab, _objectPoolMaxSize[objectTypeKey]);
        }

        return _objectPoolHash[objectTypeKey].Dequeue();
    }

    // Set up Object Pool Item before use.
    public static void SetupObjectPoolItemByTargetPrefab(GameObject targetPrefab, int prePoolingItemSize)
    {
        // If target prefab pool does not initialize, set up queue and max size.
        int objectTypeKey = targetPrefab.GetInstanceID();
        if (!_objectPoolHash.ContainsKey(objectTypeKey))
        {
            _objectPoolHash.Add(objectTypeKey, new Queue<GameObject>());
            _objectPoolMaxSize.Add(objectTypeKey, 0);
        }

        // Instantiate pool item and disable object for ready to use it.
        GameObject gameObjectCache;
        ObjectPoolItem objectPoolItemCache;
        for (int i = 0; i < prePoolingItemSize; i++)
        {
            gameObjectCache = GameObject.Instantiate(targetPrefab);

            // Set up return address for recycle object.
            objectPoolItemCache = gameObjectCache.AddComponent<ObjectPoolItem>();
            objectPoolItemCache.InstanceType = targetPrefab;

            gameObjectCache.SetActive(false);
        }
        _objectPoolMaxSize[objectTypeKey] += prePoolingItemSize;
    }
}