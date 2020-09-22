using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GuestController : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;

    [SerializeField,Range(1.5f, 2.5f)]
    private float targetDistanceOffset = 1.5f;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private WaitForSeconds _pathfindRate = new WaitForSeconds(0.1f);

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _navMeshAgent.speed = 3.5f;
        StartCoroutine(_MoveToChair());
    }

    private void Update()
    {

    }

    private void Sit()
    {
        _navMeshAgent.isStopped = true;
        // Guest Transform 방향 의자 방향으로 변경할 것
        _animator.SetTrigger("Sit");
    }

    private IEnumerator _MoveToChair()
    {
        while(Vector3.Distance(this.transform.position, Target.transform.position) > 1)
        {
            _navMeshAgent.SetDestination(Target.transform.position
                                     + Target.transform.forward * targetDistanceOffset);
            yield return _pathfindRate;
        }

        Sit();
        yield return null;
    }
}
