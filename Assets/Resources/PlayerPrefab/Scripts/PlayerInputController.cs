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
    private float KMCameraRotationSpeed = 180f;

    private PlayerInputValue _currentInput = new PlayerInputValue();
    private PlayerMovement _playerMovement = null;

    #region MonoBehaviour Callbacks
    private void OnEnable()
    {
        _currentInput.PositionInput = Vector3.zero;
        _currentInput.RotationInput = Vector3.zero;

        if (XRDevice.isPresent)
        {

        }
        else
        {
            InputBinderForUpdate += new InputBinder(KMPositionInput);
            InputBinderForUpdate += new InputBinder(KMRotationInput);
        }
    }

    private void Start()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        if (_playerMovement == null || PlayerCamera == null)
        {
            Debug.Log("Player InputController : Components are Unset, Please Check Object");
            gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        InputBinderForUpdate();
        _playerMovement.SetTargetMovement(_currentInput);
    }

    private void OnDisable()
    {
        InputBinderForUpdate = null;
    }
    #endregion

    #region Input Handler
    private void KMPositionInput()
    {
        _currentInput.PositionInput = new Vector3(Input.GetAxisRaw("Horizontal"), 
                                            0, 
                                            Input.GetAxisRaw("Vertical")).normalized;
    }

    private void KMRotationInput()
    {
        _currentInput.RotationInput = new Vector3(Input.GetAxisRaw("Mouse Y"), 
                                            Input.GetAxisRaw("Mouse X"), 
                                            0).normalized * KMCameraRotationSpeed;

        Vector3 cameraEulerAngleCache = PlayerCamera.transform.rotation.eulerAngles;
        cameraEulerAngleCache.x -= _currentInput.RotationInput.x * Time.deltaTime;
        PlayerCamera.transform.rotation = Quaternion.Euler(cameraEulerAngleCache);
    }
    #endregion
}

public struct PlayerInputValue
{
    public Vector3 PositionInput;
    public Vector3 RotationInput;
}