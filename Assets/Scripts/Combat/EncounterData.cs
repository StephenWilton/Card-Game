using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EncounterEnemySlot
{
    public EnemyData enemyData;
    [Range(0, 1)] public int row = 0;
    [Min(0)] public int column = 0;
}

[CreateAssetMenu(fileName = "New Encounter", menuName = "Encounters/Encounter Data")]
public class EncounterData : ScriptableObject
{
    public int gridColumns = 3;
    public List<EncounterEnemySlot> enemies = new List<EncounterEnemySlot>();
}
