using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelItemUI : MonoBehaviour
{
    public TextMeshProUGUI levelText;
    public Image imgLock;
    private LevelData levelData;

    public void Setup(LevelData data)
    {
        levelData = data;
        levelText.text = "Level " + data.levelIndex;

        if (data.levelIndex <= GameData.sceneLevel)
        {
            GetComponent<Image>().color = Color.green;
            GetComponent<Button>().interactable = true;
            imgLock.gameObject.SetActive(false);
        } else
        {
            GetComponent<Image>().color = Color.red;
            GetComponent<Button>().interactable = false;
            imgLock.gameObject.SetActive(true);
        }
    }

    public void OnClick()
    {
        MainMenu.instance.SelectLevel(levelData);
    }
}