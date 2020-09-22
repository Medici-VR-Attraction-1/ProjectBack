using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestController : MonoBehaviour
{
    [SerializeField]
    GameObject Target;

    NavMeshAgent _navMeshAgent;
    Animator _animator;
    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        
        _navMeshAgent.SetDestination(Target.transform.position);
        _navMeshAgent.speed = 3.5f;
    }

    void Update()
    {
        if (Vector3.Distance(this.transform.position,Target.transform.position) < 1)
        {
            Target = GameObject.Find("Chair");
            _navMeshAgent.SetDestination(Target.transform.position);
            if (Vector3.Distance(this.transform.position, Target.transform.position) < 1)
            {
                Sit();
            }
        }
    }

    public void Sit()
    {
        _navMeshAgent.speed = 0;
        _animator.SetTrigger("Sit");
    }
}
