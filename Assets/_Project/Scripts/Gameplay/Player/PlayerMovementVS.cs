using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovementVS : MonoBehaviour
{
    [Header("Input System")]
    public InputActionAsset inputActions;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;

    private InputAction moveAction;
    private Rigidbody rb;
    private Vector3 moveDirection;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveAction = inputActions.FindAction("Player/Move");
        moveAction.Enable();
    }

    private void OnDisable()
    {
        moveAction.Disable();
    }

    private void Update()
    {
        Vector2 input = moveAction.ReadValue<Vector2>();

        moveDirection = new Vector3(input.x, 0f,input.y).normalized;
    }

    private void FixedUpdate()
    {
        rb.MovePosition(rb.position + moveDirection * moveSpeed * Time.fixedDeltaTime);
    }
}
