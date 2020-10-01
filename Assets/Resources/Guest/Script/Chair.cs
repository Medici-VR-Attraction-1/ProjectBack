using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    private GameObject _connectedDish = null;

    private void Awake()
    {
        GuestGenerator.EnqueueChair(this.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Ingredient")
        {
            _connectedDish = other.gameObject;
        }
    }

    public void CheckDish(out GameObject dish) // 음식오브젝트가 담긴 변수를 외부로 내보낸다
    {
        dish = _connectedDish;
        _connectedDish = null;
    }
}
