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
    private float CreatTime = 10f;

    [SerializeField]
    private GameObject GuestPrefab;

    
    //private float _currentTime = 0f;
    private WaitForSeconds _guestSpawnRate = new WaitForSeconds(1.0f);
    private bool _isEnabled;

    #region MonoBehaivour Callbacks
    private void OnEnable() { _isEnabled = true; }
    private void OnDisable() { _isEnabled = false; }

    private void Start()
    {
        for (int i = 0; i < _chairQueue.Count; i++)
        {
            GameObject guest = Instantiate(GuestPrefab);
            guest.SetActive(false);
        }
        StartCoroutine(_SpawnGuest());
    }

    private void Update()
    {
        //_currentTime += Time.deltaTime;
        //if (_currentTime > CreatTime)
        //{
        //    if(_guestQueue.Count != 0)
        //    {
        //        GameObject guest = _guestQueue.Dequeue();
        //        guest.SetActive(true);
        //        guest.transform.position = transform.position;
        //    }
        //    _currentTime = 0;
        //}
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