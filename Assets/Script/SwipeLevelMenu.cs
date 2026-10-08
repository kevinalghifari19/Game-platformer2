using UnityEngine;
using UnityEngine.UI;

public class SwipeLevelMenu : MonoBehaviour
{
    [Header("UI References")]
    public Scrollbar scrollbar;
    
    [Header("Settings")]
    public float snapSpeed = 0.1f;
    
    private float[] pos;
    private int currentLevelIndex = 0; // Menyimpan indeks level yang sedang aktif/dipilih

    void Start()
    {
        // Menghitung posisi snap untuk setiap tombol level
        pos = new float[transform.childCount];
        float distance = 1f / (pos.Length - 1f);

        for (int i = 0; i < pos.Length; i++)
        {
            pos[i] = distance * i;
        }
    }

    void Update()
    {
        // Berpindah secara halus (Lerp) ke posisi level yang dipilih
        if (pos != null && pos.Length > 0)
        {
            scrollbar.value = Mathf.Lerp(scrollbar.value, pos[currentLevelIndex], snapSpeed);
        }

        // Animasi Pembesaran: Perbesar level yang dipilih, perkeceil sisanya
        for (int i = 0; i < transform.childCount; i++)
        {
            if (i == currentLevelIndex)
            {
                // Level Aktif (Di Tengah) -> Diperbesar (1.3x)
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(1.3f, 1.3f), 0.1f);
            }
            else
            {
                // Level Lain -> Diperkecil (0.8x)
                transform.GetChild(i).localScale = Vector2.Lerp(transform.GetChild(i).localScale, new Vector2(0.8f, 0.8f), 0.1f);
            }
        }
    }

    // Fungsi untuk Tombol Panah Kanan ( > )
    public void NextLevel()
    {
        if (currentLevelIndex < transform.childCount - 1)
        {
            currentLevelIndex++;
        }
    }

    // Fungsi untuk Tombol Panah Kiri ( < )
    public void PrevLevel()
    {
        if (currentLevelIndex > 0)
        {
            currentLevelIndex--;
        }
    }
}