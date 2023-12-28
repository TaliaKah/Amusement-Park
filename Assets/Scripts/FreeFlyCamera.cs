using UnityEngine;

public class FreeFlyCamera : MonoBehaviour
{
    public float speed = 5.0f;
    public float sensitivity = 2.0f;
    public float rotationLimit = 80.0f;

    public float minX = -100.0f;
    public float maxX = 100.0f;
    public float minY = 1.0f;
    public float maxY = 50.0f;
    public float minZ = -100.0f;
    public float maxZ = 100.0f;

    private float currentRotationX = 0.0f;

    void Update()
    {
        // Mouvement de la caméra avec les touches
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 direction = new Vector3(horizontal, 0.0f, vertical).normalized;

        // Translation de la caméra en fonction des axes locaux
        Vector3 localDirection = transform.TransformDirection(direction);
        Vector3 newPosition = transform.position + localDirection * speed * Time.deltaTime;
        newPosition.x = Mathf.Clamp(newPosition.x, minX, maxX);
        newPosition.y = Mathf.Clamp(newPosition.y, minY, maxY);
        newPosition.z = Mathf.Clamp(newPosition.z, minZ, maxZ);

        transform.position = newPosition;

        // Rotation de la caméra avec la souris
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        // Rotation de la caméra uniquement sur l'axe Y
        float rotationX = -mouseY * sensitivity;
        currentRotationX += rotationX;
        currentRotationX = Mathf.Clamp(currentRotationX, -rotationLimit, rotationLimit);

        // Appliquer la rotation à la caméra en fonction des axes locaux
        transform.Rotate(Vector3.up * mouseX * sensitivity, Space.World);
        transform.localRotation = Quaternion.Euler(currentRotationX, transform.localRotation.eulerAngles.y, 0.0f);
    }
}
