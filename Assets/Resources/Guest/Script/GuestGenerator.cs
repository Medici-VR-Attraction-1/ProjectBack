using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestGenerator : MonoBehaviour
{
    // Chair Queue For Publish Object
    private static Queue<GameObject> _chairQueue = new Queue<GameObject>();
    public static void EnqueueChair(GameObject chair) { _chairQueue.Enqueue(chair); }
    public static GameObject DequeueChair() { return _chairQueue.Dequeue(); }

    // Guest Object Pool
    private static Queue<GameObject> _guestQueue = new Queue<GameObject>();
    public static void EnqueueGuest(GameObject guest) { _guestQueue.Enqueue(guest); }

    [SerializeField]
    private GameObject GuestPrefab = null;

    [SerializeField, Range(0.0f, 10.0f)]
    private float GuestSpawnRate = 2.0f;

    private WaitForSeconds _guestSpawnRate;

    #region MonoBehaivour Callbacks

    private void Start()
    {
        // Check Object Reference in Inspector
        if (GuestPrefab == null)
        {
            Debug.Log("GuestGenerator : Guest is UnSet. Please Check Properties.");
            gameObject.SetActive(false);
        }

        // Instantiate Guests amount of Chair
        for (int i = 0; i < _chairQueue.Count; i++)
        {
            GameObject guest = Instantiate(GuestPrefab);
            guest.SetActive(false);
        }

        _guestSpawnRate = new WaitForSeconds(GuestSpawnRate);
        StartCoroutine(_SpawnGuest());
    }
    #endregion

    private IEnumerator _SpawnGuest() // 일정시간마다 손님을 생성한다
    {
        while (this.enabled)
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