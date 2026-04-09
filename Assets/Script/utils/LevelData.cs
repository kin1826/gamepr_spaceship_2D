using UnityEngine;

[CreateAssetMenu(menuName = "Game/Level")]
public class LevelData : ScriptableObject
{
    public int levelIndex;
    public bool isSuccess;

    public int maxProcess;
    public int[] thresholds;

    public Sprite background;
}