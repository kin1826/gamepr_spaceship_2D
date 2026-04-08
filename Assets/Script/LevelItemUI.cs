using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelItemUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    private LevelData levelData;

    public void Setup(LevelData data)
    {
        levelData = data;
        levelText.text = "Level " + data.levelIndex;
    }

    public void OnClick()
    {
        MainMenu.instance.SelectLevel(levelData);
    }
}