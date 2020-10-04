using Photon.Pun;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(PhotonView))]
public class PlayerHandAction : MonoBehaviour
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

    // Property Get Set
    public bool CheckHandUsing() { return _isHandUsing; }

    // Property Initializer
    public void SetHandProperties(float handDistance, string buttonName, bool isLeftHand)
    {
        _isLeftHand = isLeftHand;

        _handDistance = handDistance * handDistance;
        _inputButtonName = buttonName;
    }

    private void Start()
    {
        _photonView = GetComponent<PhotonView>();
        if (!_photonView.IsMine) this.enabled = false;
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

        ///
        if (_grabTarget != null && _grabTarget.activeSelf)
        {
            Vector3 forceVector = (transform.forward + transform.up * 0.12f) * 2f;
            _grabTarget.GetComponent<HoldableObjectContoller>().ReleaseObject(forceVector);
            _grabTarget.transform.SetParent(null);
            _grabTarget = null;
        }
        _isHandUsing = false;
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



    // Grab object if object is in Trigger
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ingredient" && !_isHandUsing)
        {
            _grabTarget = other.gameObject;
            
            _grabTarget.transform.SetParent(transform);
            _grabTarget.transform.position = transform.position;
            Vector3 offset = transform.forward + transform.right * (_isLeftHand ? 1 : -1);
            _grabTarget.GetComponent<HoldableObjectContoller>().HoldObject();
            _grabTarget.transform.position += offset * 0.12f;

            _isHandUsing = true;
        }
    }
}