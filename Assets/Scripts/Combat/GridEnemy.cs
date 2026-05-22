public class GridEnemy
{
    public Unit Unit { get; }
    public int Row { get; }
    public int Column { get; }
    public int AttackDamage { get; }

    public bool IsAlive => Unit != null && !Unit.IsDead;
    public string PositionName => Row == 0 ? $"Front {Column + 1}" : $"Back {Column + 1}";

    public GridEnemy(Unit unit, int row, int column, int attackDamage)
    {
        Unit = unit;
        Row = row;
        Column = column;
        AttackDamage = attackDamage;
    }
}
