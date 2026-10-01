using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget;

    [Header("Mouse")]
    [SerializeField] private float mouseSensitivity = 3f;

    [Header("Vertical Limits")]
    [SerializeField] private float minPitch = -30f;
    [SerializeField] private float maxPitch = 70f;

    private float yaw;
    private float pitch;

    public bool isDisabled = false;

    private void Start()
    {
        Vector3 angles = cameraTarget.eulerAngles;

        yaw = angles.y;
        pitch = angles.x;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        if (isDisabled) return;
        HandleCameraRotation();
    }

    private void HandleCameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        yaw += mouseX * mouseSensitivity;
        pitch -= mouseY * mouseSensitivity;

        pitch = Mathf.Clamp(
            pitch,
            minPitch,
            maxPitch
        );

        cameraTarget.rotation =
            Quaternion.Euler(pitch, yaw, 0f);
    }

    public void SetDisabled(bool isDisabled)
    {
        this.isDisabled = isDisabled;
        Cursor.lockState = this.isDisabled ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = this.isDisabled;
    }
}
