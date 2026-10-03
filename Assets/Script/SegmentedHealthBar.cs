using UnityEngine;
using UnityEngine.UI;

public class SegmentedHealthBar : MonoBehaviour
{
    public static SegmentedHealthBar Instance;

    [Header("Health Bar Settings")]
    [SerializeField] private Image healthBarImage;
    [SerializeField] private Sprite[] healthSprites;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateHealthBar();
    }

    public void UpdateHealthBar()
    {
        if (healthBarImage == null || PlayerController.Instance == null) return;

        int currentHealth = PlayerController.Instance.Health;
        int maxHealth = PlayerController.Instance.MaxHealth;

        float healthPercent = (float)currentHealth / maxHealth;

        int spriteIndex = Mathf.RoundToInt(healthPercent * (healthSprites.Length - 1));

        spriteIndex = Mathf.Clamp(spriteIndex, 0, healthSprites.Length - 1);

        healthBarImage.sprite = healthSprites[spriteIndex];
    }
}