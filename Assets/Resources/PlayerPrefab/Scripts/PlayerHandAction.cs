using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

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

    public bool CheckHandUsing() { return _isHandUsing; }

    public void SetHandProperties(float handDistance, string buttonName)
    {
        _handDistance = handDistance * handDistance;
        _inputButtonName = buttonName;
    }

    #region Keyboad And Mouse Player Hand Input Action
    public void KMPlayerGrabAction(Vector3 point)
    {
        Debug.Log(_inputButtonName);
        if (!_isHandMove) StartCoroutine(_KMPlayerGrapAction(point));
        _isHandMove = true;
    }

    private IEnumerator _KMPlayerGrapAction(Vector3 point)
    {
        Vector3 movement;
        transform.LookAt(point);

        while(Input.GetButton(_inputButtonName) && _moveAmount.sqrMagnitude < _handDistance)
        {
            movement = transform.forward * Time.deltaTime * HandAnimationSpeed;
            transform.position += movement;
            _moveAmount += movement;
            yield return _handMoveRate;
        }

        while(Vector3.Distance(transform.position, HandJoint.transform.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, HandJoint.transform.position, 0.5f);
            yield return _handMoveRate;
        }

        _moveAmount = Vector3.zero;
        _isHandMove = false;
    }

    public void KMPlayerPutAction(Vector3 point)
    {
        if (!_isHandMove) StartCoroutine(_KMPlayerPutAction(point));
        _isHandMove = true;
    }

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
        _grabTarget.GetComponent<Rigidbody>().isKinematic = false;
        _grabTarget.transform.SetParent(null);
        _isHandUsing = false;
        ///
        while (Vector3.Distance(transform.position, HandJoint.transform.position) > 0.1f)
        {
            transform.position = Vector3.Lerp(transform.position, HandJoint.transform.position, 0.5f);
            yield return _handMoveRate;
        }

        _moveAmount = Vector3.zero;
        _isHandMove = false;
        yield return null;
    }
    #endregion

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Ingredient")
        {
            _grabTarget = other.gameObject;
            _grabTarget.GetComponent<Rigidbody>().isKinematic = true;
            _grabTarget.transform.SetParent(transform);
            _isHandUsing = true;
        }
    }
}
