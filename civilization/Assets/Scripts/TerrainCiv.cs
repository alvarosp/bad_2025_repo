
public class TerrainCiv
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Letter { get; private set; }
    public bool IsNavegable { get; private set; }
    public int MovementCost { get; private set; }
    public int DefenseModifier { get; private set; }

    public TerrainCiv(int id, string name, string letter, bool isNavegable, int movementCost, int defenseModifier)
    {
        Id = id;
        Name = name;
        Letter = letter;
        IsNavegable = isNavegable;
        MovementCost = movementCost;
        DefenseModifier = defenseModifier;
    }

    public override string ToString()
    {
        return $"id: {Id}, name: {Name}, letter: {Letter}, isNavegable: {IsNavegable}, movementCost: {MovementCost}, defenseModifier: {DefenseModifier}";
    }
}
