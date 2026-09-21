using UnityEngine;

public class InfiniteBackground : MonoBehaviour
{
    [SerializeField] private float parallaxSpeed = 0.1f;
    
    private Transform cameraTransform;
    private SpriteRenderer spriteRenderer;
    private float spriteWidth;
    private float spriteHeight;
    private Vector3 startPosition;

    void Start()
    {
        cameraTransform = Camera.main.transform;
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            spriteWidth = spriteRenderer.bounds.size.x;
            spriteHeight = spriteRenderer.bounds.size.y;
        }
        
        // Set posisi awal di center camera
        startPosition = new Vector3(
            cameraTransform.position.x,
            cameraTransform.position.y,
            transform.position.z
        );
        transform.position = startPosition;
    }

    void LateUpdate()
    {
        // Hitung posisi berdasarkan parallax
        float targetX = cameraTransform.position.x * parallaxSpeed;
        float targetY = cameraTransform.position.y * parallaxSpeed;
        
        transform.position = new Vector3(
            startPosition.x + targetX,
            startPosition.y + targetY,
            transform.position.z
        );
        
        // Infinite tiling - reset jika terlalu jauh
        if (spriteWidth > 0 && spriteHeight > 0)
        {
            float cameraX = cameraTransform.position.x;
            float cameraY = cameraTransform.position.y;
            float backgroundX = transform.position.x;
            float backgroundY = transform.position.y;
            
            // Horizontal tiling
            if (backgroundX < cameraX - spriteWidth)
            {
                transform.position = new Vector3(
                    transform.position.x + spriteWidth,
                    transform.position.y,
                    transform.position.z
                );
            }
            else if (backgroundX > cameraX + spriteWidth)
            {
                transform.position = new Vector3(
                    transform.position.x - spriteWidth,
                    transform.position.y,
                    transform.position.z
                );
            }
            
            // Vertical tiling
            if (backgroundY < cameraY - spriteHeight)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y + spriteHeight,
                    transform.position.z
                );
            }
            else if (backgroundY > cameraY + spriteHeight)
            {
                transform.position = new Vector3(
                    transform.position.x,
                    transform.position.y - spriteHeight,
                    transform.position.z
                );
            }
        }
    }
}