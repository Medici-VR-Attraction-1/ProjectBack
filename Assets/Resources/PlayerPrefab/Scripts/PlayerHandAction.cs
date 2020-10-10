using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(PhotonView))]
public class PlayerHandAction : MonoBehaviour, IPunObservable
{
    [SerializeField]
    private GameObject HandJoint = null;

    [SerializeField]
    private int HandAnimationSpeed = 10;

    private bool _isHandUsing = false;
    private bool _isHandMove = false;
    private GameObject _grabTarget = null;
    private WaitForSeconds _handMoveRate = new WaitForSeconds(0.016f);

    private float _handDistance = 1f;
    private string _inputButtonName = "";
    private Vector3 _moveAmount = Vector3.zero;
    private bool _isLeftHand;
    private PhotonView _photonView;

    private Vector3 _serializePosition = Vector3.zero;
    private Quaternion _serializeRotation = Quaternion.identity;

    // Property Get Set
    public bool CheckHandUsing() { return _isHandUsing; }

    // Property Initializer
    public void SetKMPlayerHandProperties(float handDistance, string buttonName, bool isLeftHand)
    {
        _isLeftHand = isLeftHand;

        _handDistance = handDistance * handDistance;
        _inputButtonName = buttonName;
    }

    public void SetVRPlayerHandProperties(bool isLeftHand)
    {
        _isLeftHand = isLeftHand;
    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine && PhotonNetwork.IsConnected)
            GetComponent<SphereCollider>().enabled = false;
    }

    private void Update()
    {
        if (!_photonView.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position,
                                        _serializePosition,
                                        Time.deltaTime * 10f);

            transform.rotation = Quaternion.Slerp(transform.rotation,
                                           _serializeRotation,
                                           Time.deltaTime * 5f);
        }
    }

    #region Keyboad And Mouse Player Hand Input Action
    // If Hand is not Hold Object, Invoke Grab Object Action
    public void KMPlayerGrabAction(Vector3 point)
    {
        if (!_isHandMove) StartCoroutine(_KMPlayerGrapAction(point));
        _isHandMove = true;
    }

    // Move to Camera Middle Point Fast
    private IEnumerator _KMPlayerGrapAction(Vector3 point)
    {
        Vector3 movement;
        transform.LookAt(point);

        while(Input.GetButton(_inputButtonName) && _moveAmount.sqrMagnitude < _handDistance)
        {
            movement = transform.forward * Time.deltaTime * HandAnimationSpeed * 2f;
            transform.position += movement;
            _moveAmount += movement;
            yield return _handMoveRate;
        }

        while(Vector3.Distance(transform.position, HandJoint.transform.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, HandJoint.transform.position, 0.5f);
            yield return _handMoveRate;
        }

        transform.rotation = HandJoint.transform.rotation;
        _moveAmount = Vector3.zero;
        _isHandMove = false;
        yield return null;
    }

    // If Hand is not Hold Object, Invoke Put Object Action
    public void KMPlayerPutAction(Vector3 point)
    {
        if (!_isHandMove) StartCoroutine(_KMPlayerPutAction(point));
        _isHandMove = true;
    }

    // Move to Camera Middle Point Slowly. If Mouse Button Up, Release Hold Object.
    private IEnumerator _KMPlayerPutAction(Vector3 point)
    {
        Vector3 movement;
        transform.LookAt(point);
        
        while (Input.GetButton(_inputButtonName) && _moveAmount.sqrMagnitude < _handDistance)
        {
            movement = transform.forward * Time.deltaTime * HandAnimationSpeed;
            transform.position += movement;
            _moveAmount += movement;
            yield return _handMoveRate;
        }

        /// Remove Grab Object Reference and Release Object
        ReleaseGrabObject();
        ///

        while (Vector3.Distance(transform.position, HandJoint.transform.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, HandJoint.transform.position, 0.5f);
            yield return _handMoveRate;
        }

        transform.rotation = HandJoint.transform.rotation;
        _moveAmount = Vector3.zero;
        _isHandMove = false;
        yield return null;
    }
    #endregion

    #region Private Method
    public void HoldGrabObject(GameObject target)
    {
        _grabTarget = target;
        HoldableObjectContoller targetComponent = _grabTarget.GetComponent<HoldableObjectContoller>();

        if (!targetComponent.CheckHoldByPlayer())
        {
            Vector3 offset = transform.forward + transform.right * (_isLeftHand ? 1 : -1);
            offset = offset * 0.05f;

            targetComponent.HoldObject(_photonView.ViewID, offset);

            _isHandUsing = true;
        }
    }

    public void ReleaseGrabObject()
    {
        if (_grabTarget != null && _grabTarget.activeSelf)
        {
            _grabTarget.GetComponent<HoldableObjectContoller>().ReleaseObject(_photonView.ViewID, _isLeftHand);
            _grabTarget = null;
        }
        _isHandUsing = false;
    }
    #endregion

    // Grab object if object is in Trigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ingredient" && !_isHandUsing && !XRDevice.isPresent)
        {
            HoldGrabObject(other.gameObject);
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
            _serializePosition = (Vector3)stream.ReceiveNext();
            _serializeRotation = (Quaternion)stream.ReceiveNext();
        }
    }
}