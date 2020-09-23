using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    private float MoveSpeed = 1.0f;

    private CharacterController _characterController = null;
    private PlayerInputValue _targetValue = new PlayerInputValue();
    private float _rotationHorizontalValue = 0f;

    public void SetTargetMovement(PlayerInputValue playerInputValue)
    {
        _targetValue = playerInputValue;
    }

    private void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }

    // Apply Movement by Current Input
    private void Update()
    {
        _rotationHorizontalValue += _targetValue.RotationInput.y * Time.deltaTime;

        Vector3 nextMovement = transform.rotation * _targetValue.PositionInput * MoveSpeed;
        Vector3 nextRotation = new Vector3(0, _rotationHorizontalValue, 0);

        _characterController.SimpleMove(nextMovement);
        transform.rotation = Quaternion.Euler(nextRotation);
    }
}