using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR;

public class GuestController : MonoBehaviour
{
    private enum State 
    {
        Enter,
        Order,
        Leave
    }
    State state;

    [SerializeField]
    private GameObject Target;

    [SerializeField, Range(0.5f, 1.5f)]
    private float targetDistanceOffset = 1.0f;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private WaitForSeconds _rotationRate = new WaitForSeconds(0.011f);
    private WaitForSeconds _pathFindRate = new WaitForSeconds(0.1f);




    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        state = State.Enter;
        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;
        SetTarget();
    }

    private void OnDisable()
    {
        GuestGenerator.EnqueueChair(Target);
        GuestGenerator.EnqueueGuest(this.gameObject);
    }

    private void Update()
    {
        switch (state)
        {
            case State.Enter:
                break;
            case State.Order: Order();
                break;
            case State.Leave: 
                WalkToDoor();
                if (Vector3.Distance(this.transform.position,Target.transform.position) < 0.1f)
                { LeaveStore(); }      
                break;

        }
    }

    private void SetTarget()
    {
        Target = GuestGenerator.GetChair();
        _navMeshAgent.speed = 3.5f;
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(Target.transform.position
                                    + Target.transform.forward * targetDistanceOffset);
        StartCoroutine(_MoveToChair());
    }

    private IEnumerator _MoveToChair()
    {
        while (Vector3.Distance(this.transform.position, _navMeshAgent.destination) > Mathf.Epsilon)
        {
            yield return _pathFindRate;
        }
        _navMeshAgent.isStopped = true;

        for (int i = 0; i < 120; i++)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                           Target.transform.rotation,
                                           Time.deltaTime * 10f);
            yield return _rotationRate;
        }
        //_animator.SetTrigger("Sit");
        
        yield return null;
        state = State.Order;
    }

    private void Order()
    {
        print("주문상태");
        state = State.Leave;
    }
    
    private void WalkToDoor()
    {
        _navMeshAgent.isStopped = false;
        Target = GameObject.Find("GuestGenerator");
        _navMeshAgent.SetDestination(Target.transform.position);
        _navMeshAgent.speed = 3.5f;
    }
    private void LeaveStore()
    {
        this.gameObject.SetActive(false);
        GuestGenerator.EnqueueGuest(this.gameObject);
        print("가게 나가기 완료");
    }
}
