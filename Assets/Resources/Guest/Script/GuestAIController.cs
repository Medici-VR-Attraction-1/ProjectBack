using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class GuestAIController : MonoBehaviourPun, IPunObservable
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
    private Vector3 _serializePosition = Vector3.zero;
    private Quaternion _serializeRotation = Quaternion.identity;

    private WaitForSeconds _rotationRate = new WaitForSeconds(0.011f);

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();

        if (!PhotonNetwork.IsMasterClient)
        {
            _navMeshAgent.enabled = false;
        }
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
        if (PhotonNetwork.IsMasterClient)
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
        else
        {
            if(Vector3.Distance(transform.position, _serializePosition) > 2f)
            {
                transform.position = _serializePosition;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, 
                                            _serializePosition, 
                                            Time.deltaTime * 8f);
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, 
                                           _serializeRotation,
                                           Time.deltaTime * 5f);
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _serializePosition = (Vector3)stream.ReceiveNext();
            _serializeRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
