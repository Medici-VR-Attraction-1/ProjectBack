using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR;

public class GuestController : MonoBehaviour
{
    [SerializeField]
    private GameObject Target;

    [SerializeField,Range(0.5f, 1.5f)]
    private float targetDistanceOffset = 1.0f;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private WaitForSeconds _rotationRate = new WaitForSeconds(0.011f);
    private WaitForSeconds _pathFindRate = new WaitForSeconds(0.1f);

    private void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        _navMeshAgent.speed = 3.5f;
        _navMeshAgent.SetDestination(Target.transform.position
                                 + Target.transform.forward * targetDistanceOffset);

        StartCoroutine(_MoveToChair());
    }

    private void Update()
    {

    }

    private IEnumerator _MoveToChair()
    {
        while(Vector3.Distance(this.transform.position, _navMeshAgent.destination) > Mathf.Epsilon)
        {
            yield return _pathFindRate;
        }

        _navMeshAgent.isStopped = true;
        StartCoroutine(_SlowRotate());
        _animator.SetTrigger("Sit");

        yield return null;
    }

    private IEnumerator _SlowRotate()
    {
        for (int i = 0; i < 120; i++)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                           Target.transform.rotation, 
                                           Time.deltaTime * 10f);
            yield return _rotationRate;
        }

        yield return null;
    }
}
