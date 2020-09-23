using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chair : MonoBehaviour
{
    public static Queue<GameObject> ChairQueue = new Queue<GameObject>();

    void Start()
    {
        ChairQueue.Enqueue(this.gameObject);
    }

}
