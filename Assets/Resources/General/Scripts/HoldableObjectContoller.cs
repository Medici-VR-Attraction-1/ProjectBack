using Photon.Pun;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(Rigidbody))]
public class HoldableObjectContoller : MonoBehaviour
{
    private Rigidbody _rigidbody = null;
    private PhotonView _photonView = null;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _photonView = GetComponent<PhotonView>();

        if (!PhotonNetwork.IsMasterClient && PhotonNetwork.IsConnected) 
            _rigidbody.isKinematic = true;
    }

    public void ReleaseObject(Vector3 forceVector)
    {
        if (PhotonNetwork.IsConnected)
            _photonView.RPC("_ReleaseObject", RpcTarget.All, forceVector);
        else
            _ReleaseObject(forceVector);
    }

    [PunRPC]
    private void _ReleaseObject(Vector3 forceVector)
    {
        if (PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(forceVector, ForceMode.Impulse);
        }
    }

    public void HoldObject()
    {
        if (PhotonNetwork.IsConnected)
            _photonView.RPC("_HoldObject", RpcTarget.All, null);
        else
            _HoldObject();
    }

    [PunRPC]
    private void _HoldObject()
    {
        if(PhotonNetwork.IsMasterClient || !PhotonNetwork.IsConnected)
        {
            _rigidbody.isKinematic = true;
        }
    }
}
