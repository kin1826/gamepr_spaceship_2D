using UnityEngine;

public class PlayerVisual : MonoBehaviour
{
    public PlayerShield shield;
    
    private SpriteRenderer sr;

    private int currentIndex = -1;
    private ShipData ship;
    private LevelData levelData;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        // lấy tàu đã chọn
        ship = GameData.shipData;
        levelData = GameData.levelData;

        if (ship == null)
        {
            Debug.LogError("Chưa có ShipData!");
            return;
        }

        // bắt đầu LV1
        SetSprite(0);
    }

    public void UpdateSprite(int process, int[] thresholds, Sprite[] sprites)
    {
        int index = 0;

        Debug.Log("Process: " + process + " | Thresholds: " + string.Join(", ", thresholds) + " | Sprites: " + sprites.Length);
        Debug.Log("Current Sprite Index: " + currentIndex);

        for (int i = 0; i < thresholds.Length; i++)
        {
            if (process >= thresholds[i])
                index = i + 1;
        }

        index = Mathf.Clamp(index, 0, sprites.Length - 1);

        sr.sprite = sprites[index];

        // Cập nhật level hiện tại
        // GameManager.instance.currentLevel = index + 1;
        SetSprite(index);
    }

    void SetSprite(int index)
    {
        index = Mathf.Clamp(index, 0, ship.levelSprites.Length - 1);
        if (index == currentIndex) return;

        currentIndex = index;

        sr.sprite = ship.levelSprites[index];
        if (index != 0)
            shield.ActivateShield();

        GameManager.instance.currentLevel = index + 1; // Cập nhật level hiện tại dựa trên index của sprite (index 0 = level 1, index 1 = level 2, ...)
    }
}