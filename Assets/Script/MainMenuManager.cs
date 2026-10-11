using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject panelMainMenu;
    public GameObject panelLevelSelect;

    [Header("Level Buttons")]
    public Button[] levelButtons;

    void Start()
    {
        BukaMainMenu();

        // Mengambil level tertinggi yang terbuka (Default: Level 1)
        int levelReached = PlayerPrefs.GetInt("LevelReached", 1);

        for (int i = 0; i < levelButtons.Length; i++)
        {
            if (i + 1 > levelReached)
            {
                levelButtons[i].interactable = false; // Kunci tombol
            }
        }
    }

    public void BukaLevelSelect()
    {
        panelLevelSelect.SetActive(true);
    }

    public void BukaMainMenu()
    {
        panelMainMenu.SetActive(true);
        panelLevelSelect.SetActive(false);
    }

    public void PilihLevel(string namaSceneLevel)
    {
        SceneManager.LoadScene(namaSceneLevel);
    }
}