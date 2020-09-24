using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    [SerializeField]
    GameObject[] dish;
    private void Awake()
    {
        GuestGenerator.EnqueueChair(this.gameObject);
    }
}
