using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuestGenerator : MonoBehaviour
{
    
    [SerializeField]
    int GuestCountMax = 10;
    [SerializeField]
    float CreatTime = 10f;
    [SerializeField]
    GameObject GuestFactory;

    private Queue<GameObject> _queue = new Queue<GameObject>();
    private float _currentTime;
    
    void Start()
    {
        for (int i = 0; i < GuestCountMax; i++)
        {
            GameObject _guest = Instantiate(GuestFactory);
            _queue.Enqueue(_guest);
            _guest.SetActive(false);
        }
    }

    void Update()
    {
        _currentTime += Time.deltaTime;
        if (_currentTime > CreatTime)
        {
            GetGuest();
            _currentTime = 0;
        }

    }

    private void GetGuest() // 손님을 큐에서 가져오는 함수
    {
        if (_queue.Count == 0)
            return;

        GameObject _guest = _queue.Dequeue();
        _guest.transform.position = this.transform.position;
        _guest.SetActive(true);
    }
    private void ReturnGuest(GameObject guest) // 손님을 큐에 반환하는 함수
    {
        _queue.Enqueue(guest);
        guest.SetActive(false);
    }
}
