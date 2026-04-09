using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class MainMenu : MonoBehaviour
{
    public static MainMenu instance;

    public LevelData currentLevel;
    public LevelData[] allLevels;

    public GameObject guidePanel;
    public CanvasGroup guidePanelCanvas;
    public GameObject selectShipPanel;
    public CanvasGroup selectShipPanelCanvas;
    public GameObject selectLevelPanel;
    public CanvasGroup selectLevelPanelCanvas;
    
    public TextMeshProUGUI highScoreText;
    
    public Image selectShipImage;
    
    public int highScore = 0;

    // public GameData data;

    private void Awake()
    {
        instance = this;

        allLevels = Resources.LoadAll<LevelData>("Levels");
        System.Array.Sort(allLevels, (a, b) => a.levelIndex.CompareTo(b.levelIndex));
    }

    void Start()
    {
        
        LoadGame();
        AudioManager.instance.PlayMusic(AudioManager.instance.menuMusic);
    }

    public void PlayGame()
    {
        selectLevelPanel.SetActive(true);
        AnimPanel(selectLevelPanel, selectLevelPanelCanvas);
        // SceneManager.LoadScene("GameScene"); // tên scene game
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ToggleSound()
    {
        AudioListener.volume = AudioListener.volume == 1 ? 0 : 1;
    }

    public void OpenGuide()
    {
        guidePanel.SetActive(true);
        AnimPanel(guidePanel, guidePanelCanvas);
    }

    public void CloseGuide()
    {
        guidePanel.SetActive(false);
    }
    
    private void LoadGame()
    {
        GameData.Load();
        selectShipImage.sprite = GameData.shipData.levelSprites[4];

        highScore = GameData.highScore;
        highScoreText.text = "Highest point: " + highScore;
    }
    
    public void AnimPanel(GameObject panel, CanvasGroup canvasGroup)
    {
        // reset trạng thái ban đầu
        panel.transform.localScale = Vector3.zero;
        canvasGroup.alpha = 0;

        // scale pop (nảy nảy)
        panel.transform
            .DOScale(Vector3.one, 0.4f)
            .SetEase(Ease.OutBack)
            .SetUpdate(true); // quan trọng khi timeScale = 0

        // fade in
        canvasGroup
            .DOFade(1, 0.3f)
            .SetUpdate(true);
    }

    public void OpenSelectShip()
    {
        selectShipPanel.SetActive(true);
        AnimPanel(selectShipPanel, selectShipPanelCanvas);
    }
    public void CloseSelectShip()
    {
        selectShipPanel.SetActive(false);
        SelectShip();
    }

    public void LoadLevel(LevelData level)
    {
        currentLevel = level;

        Debug.Log("Load level: " + level.levelIndex);

        // setup game theo level
        // SetUpSlider();
        // đổi background
    }

    public void SelectShip()
    {
        ShipData ship = GameData.shipData;

        selectShipImage.sprite = ship.levelSprites[
            ship.levelSprites.Length - 1
        ];

    }
    public void CloseSelectLevel()
    {
        selectLevelPanel.SetActive(false);
    }
    public void SelectLevel(LevelData level)
    {
        currentLevel = level;

        // lưu lại để scene sau dùng
        GameData.levelData = level;
        GameData.level = level.levelIndex;
        GameData.Save();

        SceneManager.LoadScene("GameScene");
    }

    public void GoToNextLevel()
    {
        if (currentLevel == null)
        {
            Debug.LogError("currentLevel NULL");
            return;
        }

        int currentIndex = currentLevel.levelIndex - 1;
        int nextIndex = currentIndex + 1;

        if (nextIndex < allLevels.Length)
        {
            LevelData nextLevel = allLevels[nextIndex];
            SelectLevel(nextLevel);
        }
        else
        {
            Debug.Log("Hết level, quay về menu");
            SceneManager.LoadScene("MainMenu");
        }
    }
}