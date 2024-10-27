using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 0.125f;
    public float rotationSpeed = 5.0f;
    public float height = 3.0f;
    public Vector3 focusOffset = new Vector3(0, 1.5f, 0);
    public float shakeMagnitude = 0.1f; // U“®‚Ì‹­‚³
    public bool isShaking = false; // ƒJƒƒ‰‚ªU“®’†‚©‚Ç‚¤‚©‚Ìƒtƒ‰ƒO

    private Vector3 offset;
    private float currentX = 0.0f;
    private float currentY = 0.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    void Start()
    {
        offset = transform.position - player.position;
        offset.y = height;
    }

    void LateUpdate()
    {
        currentX += Input.GetAxis("Mouse X") * rotationSpeed;
        currentY = Mathf.Clamp(currentY, yMinLimit, yMaxLimit);

        Quaternion rotation = Quaternion.Euler(currentY, currentX, 0);
        Vector3 rotatedOffset = rotation * offset;
        Vector3 targetPosition = player.position + rotatedOffset;

        // Target pull‚ª‰Ÿ‚³‚ê‚Ä‚¢‚éŠÔAƒJƒƒ‰‚ªU“®
        if (Input.GetButton("Target pull"))
        {
            isShaking = true;
        }
        else
        {
            isShaking = false;
        }

        if (isShaking)
        {
            // ƒJƒƒ‰‚ÌU“®ˆ—
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;
            transform.position = Vector3.Lerp(transform.position, targetPosition + shakeOffset, smoothSpeed);
        }
        else
        {
            transform.position = Vector3.Lerp(transform.position, targetPosition, smoothSpeed);
        }

        Vector3 focusPosition = player.position + focusOffset;
        transform.LookAt(focusPosition);
    }

    void Update()
    {
        offset.y = height;
    }
}
