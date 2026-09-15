using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    // Singleton Pattern
    public static HeartController Instance;

    [Header("Heart UI Settings")]
    [SerializeField] private Transform heartsParent;
    [SerializeField] private GameObject heartContainerPrefab;

    private void Awake()
    {
        // Setup Singleton
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
        CreateHearts();
    }

    private void CreateHearts()
    {
        // Hapus semua anak yang ada sebelumnya (untuk mencegah duplikasi)
        foreach (Transform child in heartsParent)
        {
            Destroy(child.gameObject);
        }

        // Buat hati sebanyak Max Health player
        int maxHealth = PlayerController.Instance.MaxHealth;
        
        for (int i = 0; i < maxHealth; i++)
        {
            // Instantiate prefab heart container
            GameObject heart = Instantiate(heartContainerPrefab, heartsParent);
            
            // Set nama untuk debugging
            heart.name = $"Heart_{i}";
        }

        // Update UI setelah semua hati dibuat
        UpdateHeartsUI();
    }

    public void UpdateHeartsUI()
    {
        int currentHealth = PlayerController.Instance.Health;
        int maxHealth = PlayerController.Instance.MaxHealth;

        // Loop melalui semua anak di heartsParent
        for (int i = 0; i < heartsParent.childCount; i++)
        {
            Transform heartTransform = heartsParent.GetChild(i);
            
            // Cari child bernama "HeartFill" di dalam setiap heart container
            Transform heartFill = heartTransform.Find("HeartFill");
            
            if (heartFill != null)
            {
                // Jika index hati kurang dari current health, nyalakan fill
                if (i < currentHealth)
                {
                    heartFill.gameObject.SetActive(true);
                }
                else
                {
                    heartFill.gameObject.SetActive(false);
                }
            }
        }
    }
}