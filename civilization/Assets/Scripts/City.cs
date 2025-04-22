using UnityEngine;

public class City : MonoBehaviour
{
    [SerializeField] public int productionPerTurn;
    [SerializeField] public int nextUnitProduction;
    
    public GameObject unitGO;
    public Vector3Int Position { get; set; }
    public Player Owner { get; set; }
    public string Name { get; set; }
    public int Id { get; set; }
    public UnitType UnitType { get; set; }

    private int production;
    
    void Start()
    {
        production = 0;
    }
    public void AdvanceTurn()
    {
        production += productionPerTurn;
        if (production >= nextUnitProduction)
        {
            if (MapManager.Instance.SpawnUnitClosestTo(unitGO, UnitType, Owner, Position.x, Position.y))
            {
                production -= nextUnitProduction;
            }
        }
    }
}
