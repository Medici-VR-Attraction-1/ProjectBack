using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;

[RequireComponent(typeof(NavMeshAgent))]
public class GuestAIController : MonoBehaviourPunCallbacks, IPunObservable
{
    private enum GuestBehaviourState
    {
        EnterToCounter,
        WaitForFood,
        ExitToDefaultPosition
    }

    private GuestBehaviourState _currentState;
    private CounterData _currentTarget;

    private NavMeshAgent _navMeshAgent = null;
    private Vector3 _defaultPosition = Vector3.zero;

    private Vector3 _serializePosition = Vector3.zero;
    private Quaternion _serializeRotation = Quaternion.identity;
    private string _serializeTargetNameCache = null;

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.enabled = false;
    }

    private void Start()
    {
        _defaultPosition = transform.position;

        if (photonView.IsMine)
        {
            _currentState = GuestBehaviourState.EnterToCounter;
            _currentTarget = GuestManager.GetInstance().GetCounterFromQueue();

            _navMeshAgent.enabled = true;
            _navMeshAgent.SetDestination(_currentTarget.CounterPosition);
        }
    }

    private void Update()
    {
        if (photonView.IsMine && _navMeshAgent.enabled)
        {
            switch (_currentState)
            {
                case GuestBehaviourState.EnterToCounter:
                    EnterToCounter();
                    break;
                case GuestBehaviourState.WaitForFood:
                    WaitForFood();
                    break;
                case GuestBehaviourState.ExitToDefaultPosition:
                    ExitToDefaultPosition();
                    break;
            }
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,
                                        _serializePosition,
                                        Time.deltaTime * 10f);

            transform.rotation = Quaternion.Lerp(transform.rotation,
                                           _serializeRotation,
                                           Time.deltaTime * 5f);
        }

    }
    #endregion

    #region Guest State Behaviours
    private void EnterToCounter()
    {
        if (_navMeshAgent.isStopped)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_currentTarget.CounterPosition);
        }

        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < Mathf.Epsilon)
        {
            _navMeshAgent.isStopped = true;
            _currentState = GuestBehaviourState.WaitForFood;
        }
    }

    private void WaitForFood()
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(_defaultPosition);
        _currentState = GuestBehaviourState.ExitToDefaultPosition;
    }

    private void ExitToDefaultPosition()
    {
        if (_navMeshAgent.isStopped)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_defaultPosition);
        }

        if (Vector3.Distance(transform.position, _navMeshAgent.destination) < Mathf.Epsilon)
        {
            _navMeshAgent.isStopped = true;
            _navMeshAgent.enabled = false;
            PhotonNetwork.Destroy(gameObject);
        }
    }
    #endregion

    private void OnDestroy()
    {
        if (photonView.IsMine) GuestManager.GetInstance().ReturnCounter(_currentTarget);
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(_currentTarget.CounterName);
            stream.SendNext(_currentState);
        }
        else
        {
            _serializePosition = (Vector3)stream.ReceiveNext();
            _serializeRotation = (Quaternion)stream.ReceiveNext();
            _serializeTargetNameCache = (string)stream.ReceiveNext();
            _currentState = (GuestBehaviourState)stream.ReceiveNext();
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        base.OnMasterClientSwitched(newMasterClient);
        if(PhotonNetwork.LocalPlayer == newMasterClient)
        {
            Debug.Log("This Client is Master Client");

            Transform tr = GuestManager.GetInstance().transform.Find(_serializeTargetNameCache);

            _currentTarget = new CounterData(tr.position,
                                         tr.GetComponent<CounterTriggerHandler>(),
                                         _serializeTargetNameCache);

            photonView.TransferOwnership(newMasterClient);

            _navMeshAgent.enabled = true;
            _navMeshAgent.isStopped = true;
        }
    }
}
