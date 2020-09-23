using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestGenerator : MonoBehaviour
{
    private static Queue<GameObject> _chairQueue = new Queue<GameObject>();
    public static void EnqueueChair(GameObject chair) { _chairQueue.Enqueue(chair); }
    public static GameObject GetChair() { return _chairQueue.Dequeue(); }

    private static Queue<GameObject> _guestQueue = new Queue<GameObject>();
    public static void EnqueueGuest(GameObject guest) { _guestQueue.Enqueue(guest); }

    [SerializeField]
    private GameObject GuestPrefab = null;

    [SerializeField, Range(0.0f, 10.0f)]
    private float GuestSpawnRate = 2.0f;

    private WaitForSeconds _guestSpawnRate;
    private bool _isEnabled;

    #region MonoBehaivour Callbacks
    private void OnEnable() { _isEnabled = true; }
    private void OnDisable() { _isEnabled = false; }

    private void Start()
    {
        if (GuestPrefab == null)
        {
            Debug.Log("GuestGenerator : Guest is UnSet. Please Check Properties.");
            gameObject.SetActive(false);
        }

        for (int i = 0; i < _chairQueue.Count; i++)
        {
            GameObject guest = Instantiate(GuestPrefab);
            guest.SetActive(false);
        }

        _guestSpawnRate = new WaitForSeconds(GuestSpawnRate);
        StartCoroutine(_SpawnGuest());
    }
    #endregion

    private IEnumerator _SpawnGuest()
    {
        while (this._isEnabled)
        {
            if(_guestQueue.Count != 0)
            {
                GameObject guest = _guestQueue.Dequeue();
                guest.transform.position = transform.position;
                guest.SetActive(true);
            }

            yield return _guestSpawnRate;
        }
        yield return null;
    }
}