using Photon.Pun;
using UnityEngine;
using UnityEngine.XR;

[RequireComponent(typeof(PlayerMovement))]
public class PlayerInputController : MonoBehaviourPunCallbacks
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
    private float KMCameraRotationSpeed = 180f;

    [SerializeField]
    private float KMPlayerHandLenght = 1.0f;

    private PlayerInputValue _currentInput = new PlayerInputValue();

    private PlayerMovement _playerMovement = null;
    private PlayerHandAction _leftHandAction = null;
    private PlayerHandAction _rightHandAction = null;
    private Camera _playerCameraComponent = null;
    private float _verticalRotationAmount = 0f;

    #region MonoBehaviour Callbacks
    public override void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            this.enabled = false;
        }
        else
        {
            _currentInput.PositionInput = Vector3.zero;
            _currentInput.RotationInput = Vector3.zero;

            _playerMovement = GetComponent<PlayerMovement>();
            if (CheckComponentValue())
            {
                Debug.Log("Player InputController : Components are Unset, Please Check Object");
                gameObject.SetActive(false);
            }

            _playerCameraComponent = PlayerCamera.GetComponent<Camera>();
            _playerCameraComponent.enabled = true;
            PlayerCamera.GetComponent<AudioListener>().enabled = true;

            _leftHandAction = PlayerHandL.GetComponent<PlayerHandAction>();
            _rightHandAction = PlayerHandR.GetComponent<PlayerHandAction>();

            // Delegate : Bind Input by Controller Type
            if (XRDevice.isPresent)
            {
                InputBinderForUpdate += new InputBinder(SVRPositionInput);
                InputBinderForUpdate += new InputBinder(SVRRotationInput);
                InputBinderForUpdate += new InputBinder(SVRActionInput);
            }
            else
            {
                InputBinderForUpdate += new InputBinder(KMPositionInput);
                InputBinderForUpdate += new InputBinder(KMRotationInput);
                InputBinderForUpdate += new InputBinder(KMActionInput);

                _leftHandAction.SetHandProperties(KMPlayerHandLenght, "Fire1", true);
                _rightHandAction.SetHandProperties(KMPlayerHandLenght, "Fire2", false);
            }
        }
    }

    private void Update()
    {
        // Delegate : Bind Input by Controller Type
        InputBinderForUpdate();
        _playerMovement.SetTargetMovement(_currentInput);
    }

    public override void OnDisable()
    {
        // Clear Delegate on Disabled
        InputBinderForUpdate = null;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    #endregion

    #region Keyboard and Mouse Player Input Handler
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
        _verticalRotationAmount -= _currentInput.RotationInput.x * Time.deltaTime * 0.8f;
        _verticalRotationAmount = Mathf.Clamp(_verticalRotationAmount, -75f, 75f);
        cameraEulerAngleCache.x = _verticalRotationAmount;
        PlayerCamera.transform.rotation = Quaternion.Euler(cameraEulerAngleCache);
    }

    // Get Mouse Input And Handle Bind Action
    private void KMActionInput()
    {
        Vector3 targetHandPoint = new Vector3(_playerCameraComponent.scaledPixelWidth / 2f,
                                          _playerCameraComponent.scaledPixelHeight / 2f,
                                          KMPlayerHandLenght);
        targetHandPoint = _playerCameraComponent.ScreenToWorldPoint(targetHandPoint);

        if (Input.GetButtonDown("Fire1"))
        {
            if(_leftHandAction.CheckHandUsing())
            {
                _leftHandAction.KMPlayerPutAction(targetHandPoint);
            }
            else
            {
                _leftHandAction.KMPlayerGrabAction(targetHandPoint);
            }
        }
        else if(Input.GetButtonDown("Fire2"))
        {
            if (_rightHandAction.CheckHandUsing())
            {
                _rightHandAction.KMPlayerPutAction(targetHandPoint);
            }
            else
            {
                _rightHandAction.KMPlayerGrabAction(targetHandPoint);
            }
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Cursor.visible = !Cursor.visible;
            Cursor.lockState = Cursor.visible ? CursorLockMode.None : CursorLockMode.Locked;
        }
    }
    #endregion

    #region Steam VR Player Input Handler
    private void SVRPositionInput()
    {

    }

    private void SVRRotationInput()
    {

    }

    private void SVRActionInput()
    {

    }
    #endregion

    // Check Inspector Properties
    private bool CheckComponentValue()
    {
        return _playerMovement == null
            || PlayerCamera == null
            || PlayerHandR == null
            || PlayerHandL == null;
    }
}

// Player Input Format
public struct PlayerInputValue
{
    public Vector3 PositionInput;
    public Vector3 RotationInput;
}