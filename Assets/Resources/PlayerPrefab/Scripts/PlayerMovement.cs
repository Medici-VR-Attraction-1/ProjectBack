using UnityEngine;
using Photon.Pun;
using Valve.VR;

[RequireComponent(typeof(CharacterController),typeof(PhotonView))]
public class PlayerMovement : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private float MoveSpeed = 1.0f;

    private CharacterController _characterController = null;
    private PlayerInputValue _targetValue = new PlayerInputValue();
    private float _rotationHorizontalValue = 0f;
    private PhotonView _photonView = null;
    private Vector3 _recieveTargetPosition = Vector3.zero;
    private Quaternion _recieveTargetRotation = Quaternion.identity;
    public void SetTargetMovement(PlayerInputValue playerInputValue)
    {
        _targetValue = playerInputValue;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
        _photonView = GetComponent<PhotonView>();
    }

    // Apply Movement by Current Input
    private void Update()
    {
        if (_photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            _rotationHorizontalValue += _targetValue.RotationInput.y * Time.deltaTime;
            Vector3 nextRotation = new Vector3(0, _rotationHorizontalValue, 0);
            transform.rotation = Quaternion.Euler(nextRotation);

            Vector3 nextMovement = transform.rotation * _targetValue.PositionInput * MoveSpeed;
            _characterController.SimpleMove(nextMovement);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position,
                                        _recieveTargetPosition,
                                        Time.deltaTime * 10f);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                                           _recieveTargetRotation,
                                           Time.deltaTime * 5f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else
        {
            _recieveTargetPosition = (Vector3)stream.ReceiveNext();
            _recieveTargetRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}