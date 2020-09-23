﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR;

public class GuestController : MonoBehaviour
{
    private enum GuestState 
    {
        Moving,
        OrderPending,
        LeaveRestaurant
    }
    GuestState state;

    [SerializeField]
    private GameObject Target;

    [SerializeField, Range(0.5f, 1.5f)]
    private float targetDistanceOffset = 1.0f;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    private WaitForSeconds _rotationRate = new WaitForSeconds(0.011f);
    private WaitForSeconds _pathFindRate = new WaitForSeconds(0.1f);
    private Vector3 _spawnPosition = Vector3.zero;

    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        state = GuestState.Moving;
        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;

        _spawnPosition = transform.position;
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
            case GuestState.Moving:
                break;
            case GuestState.OrderPending: 
                Order();
                break;
            case GuestState.LeaveRestaurant: 
                WalkToDoor();
                if (Vector3.Distance(this.transform.position,_spawnPosition) < 1f)
                { LeaveStore(); }      
                break;

        }
    }

    private void SetTarget()
    {
        Target = GuestGenerator.GetChair();
        Debug.Log(Target.name);
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

        //_animator.SetTrigger("Sit");
        for (int i = 0; i < 120; i++)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                           Target.transform.rotation,
                                           Time.deltaTime * 10f);
            yield return _rotationRate;
        }

        state = GuestState.OrderPending;
        yield return null;
    }

    private void Order()
    {
        state = GuestState.LeaveRestaurant;
    }
    
    private void WalkToDoor()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_spawnPosition);
    }

    private void LeaveStore()
    {
        this.gameObject.SetActive(false);
    }
}
