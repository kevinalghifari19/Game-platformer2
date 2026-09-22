using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private float followSpeed = 0.1f;
    [SerializeField] private Vector3 offset;
    
    // Tambahkan ini untuk zoom out
    [SerializeField] private float cameraZoom = 8f; // Semakin besar = semakin zoom out

    private void Start()
    {
        // Set orthographic size di awal
        Camera.main.orthographicSize = cameraZoom;
    }

    private void Update()
    {
        transform.position = Vector3.Lerp(
            transform.position,
            PlayerController.Instance.transform.position + offset,
            followSpeed
        );
    }
}