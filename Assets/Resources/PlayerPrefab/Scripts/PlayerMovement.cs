using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.XR;

[RequireComponent(typeof(CharacterController),typeof(PhotonView))]
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    [SerializeField]
    private float MoveSpeed = 1.0f;

    private CharacterController _characterController = null;
    private PlayerInputValue _targetValue = new PlayerInputValue();
    private float _rotationHorizontalValue = 0f;
    private Vector3 _recieveTargetPosition = Vector3.zero;
    private Quaternion _recieveTargetRotation = Quaternion.identity;

    public void SetTargetMovement(PlayerInputValue playerInputValue)
    {
        _targetValue = playerInputValue;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Apply Movement by Current Input
    private void Update()
    {
        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            Vector3 nextMovement;

            if (!XRDevice.isPresent)
            {
                _rotationHorizontalValue += _targetValue.RotationInput.y * Time.deltaTime;
                Vector3 nextRotation = new Vector3(0, _rotationHorizontalValue, 0);
                transform.rotation = Quaternion.Euler(nextRotation);

                nextMovement = transform.rotation * _targetValue.PositionInput * MoveSpeed;
            }
            else
            {
                nextMovement = _targetValue.PositionInput * MoveSpeed;
            }

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