using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputController : MonoBehaviour
{
    [SerializeField]
    private GameObject PlayerCamera = null;

    [SerializeField]
    private float KMPlayerCameraRotateSpeed = 120.0f;

    private delegate void InputHandler();
    private InputHandler HandlePlayerInput;

    private PlayerMovement _playerMovement = null;

    private float _rotateVertical = 0f;
    private float _rotateHorizontal = 0f;

    private void Awake()
    {
        if(PlayerCamera == null)
        {
            Debug.Log("PlayerInputController : Player Camera is UnSet. Please Check Properties.");
            gameObject.SetActive(false);
        }
        else
        {
            PlayerCamera.tag = "MainCamera";
        }
        _playerMovement = GetComponent<PlayerMovement>();
        
        if (XRDevice.isPresent)
        {

        }
        else
        {
            HandlePlayerInput += new InputHandler(MovePlayerByKey);
            HandlePlayerInput += new InputHandler(RotateCameraByMouse);
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        HandlePlayerInput();
    }

    private void MovePlayerByKey()
    {
        Vector3 inputVector = Vector3.zero;
        inputVector += Input.GetAxisRaw("Vertical") * transform.forward;
        inputVector += Input.GetAxisRaw("Horizontal") * transform.right;

        _playerMovement.SetInputVector(inputVector);
    }

    private void RotateCameraByMouse()
    {
        _rotateVertical -= Input.GetAxisRaw("Mouse Y") * KMPlayerCameraRotateSpeed * Time.deltaTime;
        _rotateHorizontal += Input.GetAxisRaw("Mouse X") * KMPlayerCameraRotateSpeed * Time.deltaTime;
        Vector3 rotateEuler = new Vector3(_rotateVertical, _rotateHorizontal, 0);

        PlayerCamera.transform.rotation = Quaternion.Euler(rotateEuler);
    }
    
    private void MovePlayerByKatWalk()
    {

    }

    private void MovePlayerByTrackBall()
    {

    }
}
