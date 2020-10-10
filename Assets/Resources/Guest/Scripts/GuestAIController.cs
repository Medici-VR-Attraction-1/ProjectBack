using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

[RequireComponent(typeof(NavMeshAgent))]
public class GuestAIController : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private Text GuestTextRenderer = null;

    [SerializeField]
    private Image GuestImageRenderer = null;

    [SerializeField]
    private Animator animator = null;

    private enum GuestBehaviourState
    {
        EnterToCounter,
        WaitForFood,
        ExitToDefaultPosition
    }

    private GuestBehaviourState _currentState;
    private CounterData _currentTarget;
    private string _targetRecipeCode;
    private bool _isAte = false;

    private NavMeshAgent _navMeshAgent = null;
    private Vector3 _defaultPosition = Vector3.zero;

    private Vector3 _serializePosition;
    private Quaternion _serializeRotation = Quaternion.identity;
    private string _serializeTargetNameCache = null;

    #region MonoBehaviour Callbacks
    private void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _navMeshAgent.enabled = false;

        _serializePosition = transform.position;
    }

    private void Start()
    {
        _defaultPosition = transform.position;

        if (photonView.IsMine)
        {
            _currentState = GuestBehaviourState.EnterToCounter;
            _currentTarget = GuestManager.GetInstance().GetCounterFromQueue();

            _navMeshAgent.enabled = true;
            _navMeshAgent.SetDestination(_currentTarget.CounterPosition - Vector3.right);

            _targetRecipeCode = RecipeManager.GetInstance().GetRandomRecipeCode();
            GuestTextRenderer.text = _targetRecipeCode;

            _currentTarget.CounterComponent.SetTargetRecipeCode(_targetRecipeCode);
            GuestImageRenderer.sprite = RecipeManager.RecipeImageHash[_targetRecipeCode];
        }
        else
        {
            photonView.RPC("_RequestRecipeCode", photonView.Owner, null);
        }
    }

    private void Update()
    {
        if (photonView.IsMine && _navMeshAgent.enabled)
        {
            switch (_currentState)
            {
                case GuestBehaviourState.EnterToCounter:
                    animator.SetTrigger("Walk");
                    GuestImageRenderer.enabled = false;
                    GuestTextRenderer.enabled = false;
                    EnterToCounter();
                    break;

                case GuestBehaviourState.WaitForFood:
                    animator.SetTrigger("Idle");
                    GuestImageRenderer.enabled = true;
                    GuestTextRenderer.enabled = true;
                    WaitForFood();
                    break;

                case GuestBehaviourState.ExitToDefaultPosition:
                    animator.SetTrigger("Walk");
                    GuestImageRenderer.enabled = false;
                    GuestTextRenderer.enabled = false;
                    ExitToDefaultPosition();
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
                                            Time.deltaTime * 10f);
            }

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
        _isAte = _currentTarget.CounterComponent.IsAvailableRecipeCode();
        if (_isAte)
        {
            if (MultiPlayGameManager.GetInstance() != null)
            {
                MultiPlayGameManager.GetInstance().AddPlayerScore(_targetRecipeCode.Length * 1000);
            }

            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_defaultPosition);
            _currentState = GuestBehaviourState.ExitToDefaultPosition;
        }
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

    [PunRPC]
    private void _RequestRecipeCode()
    {
        photonView.RPC("_BroadcastTargetRecipeCode", RpcTarget.Others, _targetRecipeCode);
    }

    [PunRPC]
    private void _BroadcastTargetRecipeCode(string code)
    {
        _targetRecipeCode = code;
        GuestTextRenderer.text = code;
        GuestImageRenderer.sprite = RecipeManager.RecipeImageHash[code];
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
            stream.SendNext(_currentTarget.CounterName);
            stream.SendNext(_currentState);
            stream.SendNext(GuestImageRenderer.enabled);
            stream.SendNext(GuestTextRenderer.enabled);
        }
        else
        {
            _serializePosition = (Vector3)stream.ReceiveNext();
            _serializeRotation = (Quaternion)stream.ReceiveNext();
            _serializeTargetNameCache = (string)stream.ReceiveNext();
            _currentState = (GuestBehaviourState)stream.ReceiveNext();
            GuestImageRenderer.enabled = (bool)stream.ReceiveNext();
            GuestTextRenderer.enabled = (bool)stream.ReceiveNext();
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
                                         tr.GetComponent<BurgerTrayController>(),
                                         _serializeTargetNameCache);

            photonView.TransferOwnership(newMasterClient);

            _navMeshAgent.enabled = true;
            _navMeshAgent.isStopped = true;
        }
    }
}
