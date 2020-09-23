using System.Collections;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputController : MonoBehaviour
{
    private delegate void InputBinder();
    private InputBinder InputBinderForUpdate = null;

    [SerializeField]
    private GameObject PlayerCamera = null;

    [SerializeField]
    private GameObject PlayerHandR = null;

    [SerializeField]
    private GameObject PlayerHandL = null;

    [SerializeField]
    private GameObject TargetTemp = null;

    [SerializeField]
    private float KMCameraRotationSpeed = 180f;

    private PlayerInputValue _currentInput = new PlayerInputValue();
    private PlayerMovement _playerMovement = null;
    private PlayerHandAction _leftHandAction = null;
    private PlayerHandAction _rightHandAction = null;

    #region MonoBehaviour Callbacks
    private void OnEnable()
    {
        _currentInput.PositionInput = Vector3.zero;
        _currentInput.RotationInput = Vector3.zero;

        // Delegate : Bind Input by Controller Type
        if (XRDevice.isPresent)
        {

        }
        else
        {
            InputBinderForUpdate += new InputBinder(KMPositionInput);
            InputBinderForUpdate += new InputBinder(KMRotationInput);
            InputBinderForUpdate += new InputBinder(KMActionInput);
        }
    }

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null || PlayerCamera == null || PlayerHandR == null || PlayerHandL == null)
        {
            Debug.Log("Player InputController : Components are Unset, Please Check Object");
            gameObject.SetActive(false);
        }

        _rightHandAction = PlayerHandR.GetComponent<PlayerHandAction>();
        _leftHandAction = PlayerHandL.GetComponent<PlayerHandAction>();
    }

    private void Update()
    {
        // Delegate : Bind Input by Controller Type
        InputBinderForUpdate();

        _playerMovement.SetTargetMovement(_currentInput);
    }

    private void OnDisable()
    {
        // Clear Delegate on Disabled
        InputBinderForUpdate = null;
    }
    #endregion

    #region Input Handler
    // Update Player Position Input By Key and Mouse Controller
    private void KMPositionInput()
    {
        _currentInput.PositionInput = new Vector3(Input.GetAxisRaw("Horizontal"), 
                                            0, 
                                            Input.GetAxisRaw("Vertical")).normalized;
    }
    
    // Update Player Rotation Input By Key and Mouse Controller
    private void KMRotationInput()
    {
        _currentInput.RotationInput = new Vector3(Input.GetAxisRaw("Mouse Y"), 
                                            Input.GetAxisRaw("Mouse X"), 
                                            0).normalized * KMCameraRotationSpeed;

        Vector3 cameraEulerAngleCache = PlayerCamera.transform.rotation.eulerAngles;
        cameraEulerAngleCache.x -= _currentInput.RotationInput.x * Time.deltaTime;
        PlayerCamera.transform.rotation = Quaternion.Euler(cameraEulerAngleCache);
    }

    private void KMActionInput()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (!_leftHandAction.CheckHandUsing())
            {
                _leftHandAction.ActiveHandAction(TargetTemp);
            }
        }
        if(Input.GetButtonDown("Fire2"))
        { 
            if(!_rightHandAction.CheckHandUsing())
            {
                _rightHandAction.ActiveHandAction(TargetTemp);
            }
        }
    }
    #endregion
}

public struct PlayerInputValue
{
    public Vector3 PositionInput;
    public Vector3 RotationInput;
}