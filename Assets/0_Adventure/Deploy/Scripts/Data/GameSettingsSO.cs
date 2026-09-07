using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Data/GameSettings")]
public class GameSettingsSO : ScriptableObject
{
    public float initialWaitTime = 10f;
    public int startingGold = 3000;
    public float monsterSpawnDelay = 1f;
    public float monsterBaseHp = 200f;
    public int monsterHpIncreaseStep = 10;
    public float monsterHpIncreasePercent = 0.1f;
}
