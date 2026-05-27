using UnityEngine;

public class EnemyTargetView : MonoBehaviour
{
    public GridEnemy Enemy { get; private set; }

    public void Bind(GridEnemy enemy)
    {
        Enemy = enemy;
    }
}
