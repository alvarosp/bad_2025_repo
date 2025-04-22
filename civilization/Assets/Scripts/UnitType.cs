

public class UnitType
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int HpMax { get; set; }
    public int MovementMax {  get; set; }
    public int Combat {  get; set; }
    public int SightDistance { get; set; }

    public UnitType(int id, string name, int hpMax, int movementMax, int combat, int sightDistance)
    {
        Id = id;
        Name = name;
        HpMax = hpMax;
        MovementMax = movementMax;
        Combat = combat;
        SightDistance = sightDistance;
    }
}

