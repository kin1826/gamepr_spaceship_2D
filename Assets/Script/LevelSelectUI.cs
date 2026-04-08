using UnityEngine;

public class LevelSelectUI : MonoBehaviour
{
    public Transform content;
    public GameObject levelItemPrefab;

    void Start()
    {
        LoadLevels();
    }

    void LoadLevels()
    {
        LevelData[] levels = Resources.LoadAll<LevelData>("Levels");

        foreach (LevelData level in levels)
        {
            GameObject obj = Instantiate(levelItemPrefab, content);

            LevelItemUI item = obj.GetComponent<LevelItemUI>();
            item.Setup(level);
        }
    }
}