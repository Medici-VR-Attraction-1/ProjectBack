using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestTest : MonoBehaviour
{
    Animator anim;
    NavMeshAgent agent;
    
    public Transform target;
    Vector3 startPosition;

    float curTime;
    enum State
    {
        Walking,
        Leaving
    }
    State state;
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponentInChildren<Animator>();

        startPosition = this.transform.position;
        agent.SetDestination(target.position);
        agent.speed = 1.5f;
        anim.SetTrigger("Walk");
        state = State.Walking;
    }
    
    
    void Update()
    {
        switch (state)
        {
            case State.Walking:
                if (Vector3.Distance(this.transform.position, target.position) < 0.5f)
                {
                    agent.speed = 0;
                    anim.SetTrigger("Idle");
                    curTime += Time.deltaTime;
                    if (curTime > 4)
                    {
                        state = State.Leaving;
                    }
                }
                break;

            case State.Leaving:
                anim.SetTrigger("Walk");
                agent.speed = 1.5f;
                agent.SetDestination(startPosition);
                if (Vector3.Distance(this.transform.position, startPosition) < 1)
                {
                    Destroy(this.gameObject);
                }
                break;

        }  
    }
    
}
