using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.AI;
using Valve.VR;

public class GuestAIController : MonoBehaviour
{
    [SerializeField, Range(0.5f, 1.5f)]
    private float targetDistanceOffset = 1.0f;

    private enum GuestState
    {
        Moving,
        OrderPending,
        Eating,
        Leaving
    }
    private GuestState currentState;

    private NavMeshAgent _navMeshAgent;
    private Animator _animator;
    
    private GameObject _targetChair;
    private Chair _chairComponentCache = null;

    private Vector3 _spawnPositionCache = Vector3.zero;

    private WaitForSeconds _rotationRate = new WaitForSeconds(0.011f);

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        currentState = GuestState.Moving;
        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;

        _spawnPositionCache = transform.position;
        SetTarget();
    }

    private void Update()
    {
        switch (currentState)
        {
            //
            case GuestState.Moving:
                MoveToChair();
                break;
            //
            case GuestState.OrderPending:
                OrderPending();
                break;
            //
            case GuestState.Eating:
                EatDish();
                break;
            //
            case GuestState.Leaving:
                WalkToDoor();
                break;
        }
    }
    #endregion

    #region FSM Behaviour
    // Sense if the chair has arrived and match the distance between the chair and the guest.
    private void MoveToChair()
    {
        if (Vector3.Distance(this.transform.position, _navMeshAgent.destination) < Mathf.Epsilon)
        {
            _navMeshAgent.isStopped = true;
            StartCoroutine(_MoveToChair());
        }
    }
    // Coroutine For Animation
    private IEnumerator _MoveToChair()
    {
        // Rotate transform to Chiar Transform Forward
        for (int i = 0; i < 120; i++)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation,
                                           _targetChair.transform.rotation,
                                           Time.deltaTime * 10f);
            yield return _rotationRate;
        }

        currentState = GuestState.OrderPending;
        yield return null;
    }

    private void OrderPending()
    {
        GameObject dish;
        _chairComponentCache.CheckDish(out dish);

        if (dish != null) currentState = GuestState.Eating;
    }

    private void EatDish()
    {
        // Eat Food
        currentState = GuestState.Leaving;
    }

    private void WalkToDoor()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_spawnPositionCache);

        if (Vector3.Distance(this.transform.position, _spawnPositionCache) < 1f)
        {
            GuestGenerator.EnqueueChair(_targetChair);
            GuestGenerator.EnqueueGuest(this.gameObject);
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Private Method
    // Set the destination as a chair.
    private void SetTarget()
    {
        _targetChair = GuestGenerator.DequeueChair();
        _chairComponentCache = _targetChair.GetComponent<Chair>();

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_targetChair.transform.position
                               + _targetChair.transform.forward * targetDistanceOffset);

        currentState = GuestState.Moving;
    }
    #endregion
}
