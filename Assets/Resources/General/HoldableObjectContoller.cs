using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Rigidbody))]
public class HoldableObjectContoller : MonoBehaviourPunCallbacks, IOnPhotonViewOwnerChange
{
    #region Added
    private static int _idProvider = 0;
    public int componentID;
    private static Dictionary<int, Transform> _objectIDHash = new Dictionary<int, Transform>();

    public static Transform GetHoldableTransformByID(int index) { return _objectIDHash[index]; }

    [PunRPC]
    private void BroadcastID(int id)
    {
        componentID = id;
        _objectIDHash[id] = transform;
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        photonView.RPC("BroadcastID", newPlayer, componentID);
    }
    #endregion

    private Rigidbody _rigidbody = null;
    private bool _isHold = false;

    public bool CheckHoldByPlayer() { return _isHold; }

    private void Awake()
    {
        //
        if (PhotonNetwork.IsMasterClient)
        {
            componentID = _idProvider;
            _idProvider++;
            _objectIDHash[componentID] = transform;
        }
        //
        _rigidbody = GetComponent<Rigidbody>();

        if (!photonView.IsMine && PhotonNetwork.IsConnected) 
            _rigidbody.isKinematic = true;
    }

    // Set on Rigidbody Update and Remove Parent Transform.
    #region Release Object RPC
    public void ReleaseObject(int viewId)
    {
        photonView.RPC("_ReleaseObject", RpcTarget.All, viewId);
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

            _rigidbody.useGravity = true;
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
        photonView.RPC("_HoldObject", RpcTarget.All, viewId, offset);
    }

    [PunRPC]
    private void _HoldObject(int viewId, Vector3 offset)
    {
        PhotonView targetView = PhotonNetwork.GetPhotonView(viewId);
        photonView.TransferOwnership(targetView.Owner);
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;

        Transform hand = targetView.transform;
        transform.SetParent(hand);
        transform.position = hand.position + offset;

        _isHold = true;
    }

    public void OnOwnerChange(Player newOwner, Player previousOwner)
    {
        if (previousOwner == PhotonNetwork.LocalPlayer)
            _rigidbody.isKinematic = true;
    }
    #endregion
}
