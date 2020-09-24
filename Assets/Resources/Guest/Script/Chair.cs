using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    private GameObject connectedDish = null;

    public void CheckDish(out GameObject dish)
    {
        dish = connectedDish;
        connectedDish = null;
    }

    private void Awake()
    {
        GuestGenerator.EnqueueChair(this.gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Result")
        {
            connectedDish = other.gameObject;
        }
    }
}
