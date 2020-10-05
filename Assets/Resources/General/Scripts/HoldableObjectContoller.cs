using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

[RequireComponent(typeof(PhotonView), typeof(Rigidbody))]
public class HoldableObjectContoller : MonoBehaviourPunCallbacks
{
    private Rigidbody _rigidbody = null;
    private bool _isHold = false;

    public bool CheckHoldByPlayer() { return _isHold; }

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        if (!photonView.IsMine && PhotonNetwork.IsConnected) 
            _rigidbody.isKinematic = true;
    }

    // Set on Rigidbody Update and Remove Parent Transform.
    #region Release Object RPC
    public void ReleaseObject(int viewId)
    {
        if (PhotonNetwork.IsConnected)
            photonView.RPC("_ReleaseObject", RpcTarget.All, viewId);
        else
            _ReleaseObject(viewId);
    }

    [PunRPC]
    private void _ReleaseObject(int viewId)
    {
        PhotonView targetView = PhotonNetwork.GetPhotonView(viewId);
        transform.SetParent(null);
        transform.position = targetView.transform.position;

        if (photonView.IsMine || !PhotonNetwork.IsConnected)
        {
            Transform hand = targetView.transform;
            Vector3 forceDirection = hand.forward + hand.up * 0.12f;
            _rigidbody.isKinematic = false;
            _rigidbody.AddForce(forceDirection * 2f, ForceMode.Impulse);
        }

        _isHold = false;
    }
    #endregion

    // Transfer OwnerShip of this Object to Caller and Set Parent to Caller`s Hand Transform.
    // Set off Rigidbody Update and Follow Hand Transform.
    #region Hold Object RPC
    public void HoldObject(int viewId, Vector3 offset)
    {
        if (PhotonNetwork.IsConnected)
            photonView.RPC("_HoldObject", RpcTarget.All, viewId, offset);
        else
            _HoldObject(viewId, offset);
    }

    [PunRPC]
    private void _HoldObject(int viewId, Vector3 offset)
    {
        PhotonView targetView = PhotonNetwork.GetPhotonView(viewId);
        photonView.TransferOwnership(targetView.Owner);

        Transform hand = targetView.transform;
        transform.SetParent(hand);
        transform.position = hand.position + offset;

        _rigidbody.isKinematic = true;
        _isHold = true;
    }
    #endregion
}
