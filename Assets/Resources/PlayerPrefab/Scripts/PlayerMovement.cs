using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    public void SetInputVector(Vector3 inputVector) { _inputVector = inputVector.normalized; }

    [SerializeField]
    private float PlayerMoveSpeed = 1.0f;

    private Rigidbody _rigidbody = null;
    private Vector3 _inputVector = Vector3.zero;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX 
                           | RigidbodyConstraints.FreezeRotationZ;
    }

    private void FixedUpdate()
    {
        Vector3 nextDirection = _inputVector * Time.fixedDeltaTime * PlayerMoveSpeed;
        _rigidbody.MovePosition(_rigidbody.position + nextDirection);

        Vector3 lockedRotationEuler = Camera.main.transform.rotation.eulerAngles;
        lockedRotationEuler.x = 0;
        lockedRotationEuler.z = 0;
        _rigidbody.rotation = Quaternion.Euler(lockedRotationEuler);
    }
}