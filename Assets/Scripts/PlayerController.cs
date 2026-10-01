using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] public float moveSpeed = 5.6f;
    [SerializeField] public float rotationSpeed = 5.0f;
    [SerializeField] public Rigidbody rb;
    [SerializeField] public GameObject thirdPersonCamera;
    [SerializeField] public CameraController cameraController;
    [SerializeField] public InteractController interactController;

    [SerializeField] public Animator animator;

    public StateId currentStateId;
    Vector3 moveDirection;

    public void SwitchState(StateId designatedStateId, bool canOverride = false)
    {
        if (currentStateId == designatedStateId && canOverride == false) return;

        OnStateExited();
        currentStateId = designatedStateId;
        OnStateEntered();

    }

    private void OnDestroy()
    {
        cameraController.SetDisabled(true);
    }

    public void OnStateEntered()
    {
        switch (currentStateId) {
            case StateId.None:
                animator.PlayInFixedTime("Idle");
                cameraController.SetDisabled(true);
                break;
            case StateId.Idle:
                animator.PlayInFixedTime("Idle");
                break;
            case StateId.Walking:
                animator.PlayInFixedTime("Walk");
                break;
        }
    }

    public void OnStateExited()
    {
        switch (currentStateId)
        {
            case StateId.None:
                cameraController.SetDisabled(false);
                break;
            case StateId.Idle:
                break;
            case StateId.Walking:
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        GetMovementInput();

        switch (currentStateId)
        {
            case StateId.None:
                break;
            case StateId.Idle:
                if (moveDirection != Vector3.zero) SwitchState(StateId.Walking);
                break;
            case StateId.Walking:
                if (moveDirection == Vector3.zero) SwitchState(StateId.Idle);
                break;
        }
    }

    private void FixedUpdate()
    {
        switch (currentStateId)
        {
            case StateId.None:
                return;
            case StateId.Idle:
                break;
            case StateId.Walking:
                break;
        }

        Move();
    }

    public void GetMovementInput()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        moveDirection = new Vector3(horizontalInput, 0f, verticalInput);

        Vector3 cameraForward = thirdPersonCamera.transform.forward;
        Vector3 cameraRight = thirdPersonCamera.transform.right;

        cameraForward.y = 0f;
        cameraRight.y = 0f;

        cameraForward.Normalize();
        cameraRight.Normalize();

        moveDirection = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

        if (moveDirection.magnitude > 1f)
        {
            moveDirection.Normalize();
        }
    }

    public void Move()
    {
        rb.linearVelocity = new Vector3(moveDirection.x * moveSpeed, rb.linearVelocity.y, moveDirection.z * moveSpeed);

        if (moveDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
        }
    }

    public void OnInteract()
    {
        interactController.currentInteractable.Interact(this);
    }
}
