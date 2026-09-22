using UnityEngine;

public class RetroFloating : MonoBehaviour
{
    [Header("Bobbing Settings")]
    [Tooltip("Jarak Naik Turun Logo")]
    public float floatAmplitude = 10f;

    [Tooltip("Kecepatan Naik Turun Logo")]
    public float floatSpeed = 3f;

    [Header("Retro Low FPS")]
    [Tooltip("Target Frame Rate untuk efek patah-patah retro (contoh: 6, 8, atau 12 FPS)")]
    public float targetFPS = 8f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private float timer;

    private void Start()
    {
        //ambil komponen RectTransform dari Logo Realize
        rectTransform = GetComponent<RectTransform>();
        
        if (rectTransform != null)
        {
            startPosition = rectTransform.anchoredPosition;
        }
    }

    private void Update()
    {
        if (rectTransform == null) return;
        
        //Hitung jeda antar frame sesuai target FPS
        float frameInterval = 1f / targetFPS;

        //Tambahkan timer berdasarkan delta time
        timer += Time.deltaTime;

        //cuman perbarui posisi pas timer melewati jeda frame
        if (timer >= frameInterval)
        {
            // atur ulang timer biar konsisten ritmenya
            timer %= frameInterval;

            //Hitung posisi naik-turun menggunakan gelombang Sinus
            float newY = startPosition.y + (Mathf.Sin(Time.time * floatSpeed) * floatAmplitude);

            //bulatkan nilai ke angka bulat
            newY = Mathf.Round(newY);

            //Terapkan posisi baru ke RectTransform Logo
            rectTransform.anchoredPosition = new Vector2(startPosition.x, newY);
        }
    }
}
