using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    
    public PlayerVisual playerVisual;
    
    public GameObject gameOverPanel;
    public CanvasGroup gameOverPanelCanvas;
    public GameObject gamePausePanel;
    public CanvasGroup gamePausePanelCanvas;
    public GameObject gameWinPanel;
    public CanvasGroup winPanelCanvas;

    //Background
    public SpriteRenderer backgroundImage;
    public SpriteRenderer backgroundImage2;
    public float backgroundScrollSpeed = 2f;
    private float backgroundWidth;
    private const int BackgroundSortingOrder = -10;

    //Score
    public int score = 0;
    public bool isHighest = false;
    //Save
    public int currentLevel = 1;
    public ShipData shipData;
    public LevelData levelData;
    
    
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI scoreGameOverText;
    public TextMeshProUGUI scoreWinText;
    
    //Process
    public int currentProcess = 0;
    public int maxProcess;
    public Slider slider;
    [SerializeField] private GameObject markerPrefab;
    [SerializeField] private RectTransform markerParent;
    [SerializeField] private Image processFillImage;
    [SerializeField] private Sprite[] processStageSprites = new Sprite[5];
    
    //Energy
    public float maxEnergy = 100f;
    public float currentEnergy;
    public float drainRate = 20f; // hao mỗi giây
    public float regenRate = 10f; // hồi mỗi giây
    public Slider energySlider;
    

    void Awake()
    {
        GameData.Load();
        instance = this;
        CacheProcessFillImage();
    }

    void Start()
    {
        AudioManager.instance.PlayMusic(AudioManager.instance.gameMusic);
        UpdateUI();
        shipData = GameData.shipData;
        levelData = GameData.levelData;
        currentLevel = levelData.levelIndex; // sửa: level hiện tại đang chơi

        Debug.Log(markerPrefab);
        Debug.Log(markerParent);
        Debug.Log(shipData);


        //Set background
        // set sprite theo level
        backgroundImage.sprite = levelData.background;
        backgroundImage2.sprite = levelData.background;
        backgroundImage.sortingOrder = BackgroundSortingOrder;
        backgroundImage2.sortingOrder = BackgroundSortingOrder;
        // 🔥 QUAN TRỌNG: scale giống nhau
        Vector3 scale = new Vector3(3, 3, 1);
        backgroundImage.transform.localScale = scale;
        backgroundImage2.transform.localScale = scale;
        
        // lấy width (auto chuẩn theo sprite + scale)
        // backgroundImage.transform.localScale = Vector3.one;
        backgroundWidth = backgroundImage.bounds.size.x;
        Debug.Log("Background width: " + backgroundWidth);
        // đặt bg2 nối ngay sau bg1
        backgroundImage.transform.position = Vector3.zero;
        backgroundImage2.transform.position = new Vector3(backgroundWidth, 0, 0);
        
        

        maxProcess = levelData.maxProcess;

        SetUpSlider(maxProcess);
        
        // Init Energy
        currentEnergy = maxEnergy;
        if (energySlider != null)
        {
            energySlider.maxValue = maxEnergy;
            energySlider.value = currentEnergy;
        }
    }

    void Update()
    {
        MoveBackground();
    }

    void MoveBackground()
    {
        // move
        backgroundImage.transform.Translate(Vector2.left * backgroundScrollSpeed * Time.deltaTime);
        backgroundImage2.transform.Translate(Vector2.left * backgroundScrollSpeed * Time.deltaTime);

        // loop
        if (backgroundImage.transform.position.x <= -backgroundWidth)
        {
            backgroundImage.transform.position = backgroundImage2.transform.position + Vector3.right * backgroundWidth;
        }

        if (backgroundImage2.transform.position.x <= -backgroundWidth)
        {
            backgroundImage2.transform.position = backgroundImage.transform.position + Vector3.right * backgroundWidth;
        }
    }
    
    public void SetUpSlider(int maxProcessSlider)
    {
        maxProcess = levelData.maxProcess;

        // clear marker cũ
        foreach (Transform child in markerParent)
        {
            Destroy(child.gameObject);
        }

        foreach (float threshold in levelData.thresholds)
        {
            float percent = threshold / maxProcessSlider;

            Debug.Log("Spawn marker at: " + percent);

            GameObject marker = Instantiate(markerPrefab, markerParent);
            RectTransform rt = marker.GetComponent<RectTransform>();

            rt.anchorMin = new Vector2(percent, 0.5f);
            rt.anchorMax = new Vector2(percent, 0.5f);
            rt.anchoredPosition = Vector2.zero;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateUI();
    }

    void UpdateUI()
    {
        scoreText.text = "Score: " + score;

        float processRatio = maxProcess > 0 ? (float)currentProcess / maxProcess : 0f;
        slider.value = processRatio;
        UpdateProcessStageSprite(processRatio);
    }
    
    public void UpdateEnergy(bool isBoost)
    {
        if (isBoost)
        {
            currentEnergy -= drainRate * Time.deltaTime;
            if (currentEnergy <= 0)
            {
                currentEnergy = 0;
            }
        }
        else
        {
            currentEnergy += regenRate * Time.deltaTime;
        }

        // Giới hạn năng lượng trong khoảng [0, maxEnergy]
        currentEnergy = Mathf.Clamp(currentEnergy, 0, maxEnergy);
        
        if (energySlider != null)
        {
            energySlider.value = currentEnergy;
            
            if (currentEnergy < 20)
                energySlider.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.red;
            else
                energySlider.fillRect.GetComponent<UnityEngine.UI.Image>().color = Color.green;
        }
    }
    
    public bool CanBoost()
    {
        return currentEnergy > 0;
    }
    
    public void GameOver()
    {
        Debug.Log("Game Over");
        Time.timeScale = 0f; // pause game
        StartCoroutine(ShowWinPanelAfterDelay());
        
        AudioManager.instance.PlaySFX(AudioManager.instance.gameOverSound);
        
        SaveGame();
        if (isHighest)
        {
            scoreGameOverText.text = "New Highest Score: " + score;
        }
        else
        {
            scoreGameOverText.text = "Score: " + score;
        }
        
        gameOverPanel.SetActive(true);
        
        instance.AnimPanel(gameOverPanel, gameOverPanelCanvas);
        scoreText.enabled = false;
        
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f;
        Debug.Log("Restart");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void PauseGame()
    {
        gamePausePanel.SetActive(true);
        instance.AnimPanel(gamePausePanel, gamePausePanelCanvas);
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        gamePausePanel.SetActive(false);
        Time.timeScale = 1f;
    }

    private void SaveGame()
    {
        if (score > GameData.highScore)
        {
            isHighest = true;
            GameData.highScore = score;
            Debug.Log($"🆕 Highscore mới! {score}");
        }
        
        GameData.currentScore = score;
        // GameData.shipData = currentSkin;
        
        Debug.Log($"💾 SaveGame gọi với Score: {score}, HighScore: {GameData.highScore}");
        GameData.Save();
    }
    
    public void AddProcess(int amount)
    {
        currentProcess += amount;
        playerVisual.UpdateSprite(currentProcess, levelData.thresholds, shipData.levelSprites);
        Debug.Log(currentProcess + " -- " + maxProcess);

        if (currentProcess >= maxProcess)
        {
            currentProcess = maxProcess;
            instance.Win();
        }
            
        
        UpdateUI();
    }

    private void CacheProcessFillImage()
    {
        if (processFillImage != null || slider == null || slider.fillRect == null)
        {
            return;
        }

        processFillImage = slider.fillRect.GetComponent<Image>();
    }

    private void UpdateProcessStageSprite(float processRatio)
    {
        CacheProcessFillImage();

        if (processFillImage == null || processStageSprites == null || processStageSprites.Length == 0)
        {
            return;
        }

        int configuredStageCount = 0;
        for (int i = 0; i < processStageSprites.Length; i++)
        {
            if (processStageSprites[i] != null)
            {
                configuredStageCount++;
            }
        }

        if (configuredStageCount == 0)
        {
            return;
        }

        int stageIndex = Mathf.FloorToInt(Mathf.Clamp01(processRatio) * configuredStageCount);
        stageIndex = Mathf.Clamp(stageIndex, 0, configuredStageCount - 1);

        Sprite targetSprite = null;
        int currentConfiguredIndex = -1;
        for (int i = 0; i < processStageSprites.Length; i++)
        {
            if (processStageSprites[i] == null)
            {
                continue;
            }

            currentConfiguredIndex++;
            if (currentConfiguredIndex == stageIndex)
            {
                targetSprite = processStageSprites[i];
                break;
            }
        }

        if (targetSprite != null && processFillImage.sprite != targetSprite)
        {
            processFillImage.sprite = targetSprite;
            processFillImage.SetAllDirty();
        }
    }

    public void Win()
    {
        Time.timeScale = 0f;
        StartCoroutine(ShowWinPanelAfterDelay());
        
        AudioManager.instance.PlaySFX(AudioManager.instance.winSound);
        AudioManager.instance.PlaySFX(AudioManager.instance.winVoice);
        
        gameWinPanel.SetActive(true);
        instance.AnimPanel(gameWinPanel, winPanelCanvas);

        GameData.sceneLevel = currentLevel + 1; // lưu level cao nhất đã pass
        SaveGame();

        if (isHighest)
        {
            scoreWinText.text = "New Highest Score: " + score;
        }
        else
        {
            scoreWinText.text = "Score: " + score;
        }
    }
    public void NextLevel()
    {
        Time.timeScale = 1f;
        Debug.Log("Next Level");
        MainMenu.instance.GoToNextLevel();
    }

    private System.Collections.IEnumerator ShowWinPanelAfterDelay()
    {
        yield return new WaitForSecondsRealtime(1.5f);
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
}
