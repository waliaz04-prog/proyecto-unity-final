using UnityEngine;

public class Playerlook : MonoBehaviour
{
    public float mouseSensitivityX = 100f;
    public float mouseSensitivityY = 100f;

    private Transform payaso;
    private float xRotation = 0f;

    [Header("Evitar atravesar paredes (FPS)")]
    public float checkDistance = 0.3f;
    public float cameraOffset = 0.1f;
    public LayerMask collisionMask;

    private Vector3 defaultLocalPos;
    private PlayerMove playerMove;

    private void Start()
    {
        payaso = transform.parent;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        defaultLocalPos = transform.localPosition;
        playerMove = GetComponentInParent<PlayerMove>();
    }

    void Update()
    {
        if (Time.timeScale == 0) return;
        if (playerMove != null && playerMove.estaMuerto) return;

        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivityX * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivityY * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        payaso.Rotate(Vector3.up * mouseX);

        CheckCameraCollision();
    }

    void CheckCameraCollision()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, checkDistance, collisionMask))
            transform.localPosition = defaultLocalPos - Vector3.forward * cameraOffset;
        else
            transform.localPosition = Vector3.Lerp(transform.localPosition, defaultLocalPos, Time.deltaTime * 10f);
    }
}
