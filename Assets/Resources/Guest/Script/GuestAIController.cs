using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuestAIController : MonoBehaviour
{
    // Public For Debug, Fix to Private After Release
    public enum GuestState
    {
        Moving,
        OrderPending,
        Eating,
        Leaving
    }
    public GuestState currentState;

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
        _spawnPositionCache = transform.position;

        _targetChair = GuestGenerator.DequeueChair();
        _chairComponentCache = _targetChair.GetComponent<Chair>();

        currentState = GuestState.Moving;

        _navMeshAgent.enabled = false;
        _navMeshAgent.enabled = true;
        _navMeshAgent.SetDestination(_targetChair.transform.position);

        _navMeshAgent.isStopped = false;
    }

    private void OnDisable()
    {
        GuestGenerator.EnqueueChair(_targetChair);
        _targetChair = null;
        _chairComponentCache = null;
    }

    private void Update()
    {
        switch (currentState)
        {
            case GuestState.Moving:
                MoveToChair();
                break;

            case GuestState.OrderPending:
                OrderPending();
                break;

            case GuestState.Eating:
                EatDish();
                break;

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
        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < Mathf.Epsilon)
        {
            _navMeshAgent.isStopped = true;
            StartCoroutine(_MoveToChair());
            currentState = GuestState.OrderPending;
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
        yield return null;
    }

    private void OrderPending()
    {
        // Order Dish
        currentState = GuestState.Eating;
    }

    private void EatDish()
    {
        GameObject dish = null;
        _chairComponentCache.CheckDish(out dish);

        if (dish != null)
        {
            dish.SetActive(false);
            _navMeshAgent.SetDestination(_spawnPositionCache);
            _navMeshAgent.isStopped = false;
            currentState = GuestState.Leaving;
        }
    }

    private void WalkToDoor()
    {
        if(Vector3.Distance(transform.position, _spawnPositionCache) < 1f)
        {
            gameObject.SetActive(false);
        }
    }
    #endregion

    #region Private Method

    #endregion
}
