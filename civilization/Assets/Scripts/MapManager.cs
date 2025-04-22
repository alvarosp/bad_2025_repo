using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using System.IO;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }
    public Tilemap map;
    public GameObject GOWarrior;
    public GameObject GOBarbarian;
    public GameObject GOCity;
    public GameObject GOBarbarianCamp;
    public GameObject GOHealthBar;
    public Tile tileDesert;
    public Tile tileMountain;
    public Tile tileOcean;
    public Tile tilePlains;
    public Tile tileSnow;
    public Tile tileWater;
    public Player player;
    public Player enemy;

    public Unit[,] mapUnits;
    private City[,] mapCities;
    public TerrainCiv[,] mapTerrains;
    private Dictionary<string, Tile> tiles = new();
    private readonly List<Unit> unitsList = new();
    private readonly List<City> citiesList = new();
    private readonly int height = 13;
    private readonly int width = 21;
    private readonly int healingValue = 10;
    private static int idCounter = 1;
    private UnitType warriorType;
    private UnitType barbarianType;

    private void Awake()
    {
        // If there is an instance, and it's not me, delete myself.
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        tiles["D"] = tileDesert;
        tiles["M"] = tileMountain;
        tiles["O"] = tileOcean;
        tiles["P"] = tilePlains;
        tiles["S"] = tileSnow;
        tiles["W"] = tileWater;
        mapTerrains = new TerrainCiv[width, height];
        LoadMapFromFile();
        mapCities = new City[width, height];
        mapUnits = new Unit[width, height];
        enemy = new Player(1, "Barbarian");
        player = new Player(2, "Player");
        DBManager.Instance.InsertPlayer(enemy);
        DBManager.Instance.InsertPlayer(player);
        List<UnitType> types = DBManager.Instance.GetUnitsTypes();
        warriorType = types[0];
        barbarianType = types[1];
        SpawnCity(GOCity, GOWarrior, warriorType, "Constantinopla", player, 18, 10);
        for (int i = 0; i < 2; i++)
        {
            SpawnUnitClosestTo(GOWarrior, warriorType, player, 18, 10);
        }
        SpawnCity(GOBarbarianCamp, GOBarbarian, barbarianType, "Camp 1", enemy, 11, 3);
        SpawnUnitAt(GOBarbarian, barbarianType, enemy, 11, 3);
        SpawnCity(GOBarbarianCamp, GOBarbarian, barbarianType, "Camp 2", enemy, 5, 5);
        SpawnUnitAt(GOBarbarian, barbarianType, enemy, 5, 5);
        Debug.Log($"{unitsList.Count} total units.");
    }

    private void LoadMapFromFile()
    {
        Dictionary<string, TerrainCiv> terrains = DBManager.Instance.GetTerrains();
        string mapString = GameManager.Instance.ReadFromFile(Path.Combine(Application.dataPath, "Sql", "map.txt"));
        string[] lines = mapString.Split('\n');
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                string letter = lines[i].Substring(j, 1);
                Tile selectedTile = tiles[letter];
                TerrainCiv terrain = terrains[letter];
                map.SetTile(new Vector3Int(i, j, 0), selectedTile);
                mapTerrains[i, j] = terrain;
            }
        }
        DBManager.Instance.SaveMap(mapTerrains);
    }

    /*
     * In case saving the map is necessary
     */
    private void TilesToString()
    {
        string mapData = "";
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                Tile tile = (Tile)map.GetTile(new Vector3Int(i, j, 0));
                switch (tile.name)
                {
                    case "Tile_Desert":
                        mapData += "D";
                        break;
                    case "Tile_Mountain":
                        mapData += "M";
                        break;
                    case "Tile_Ocean":
                        mapData += "O";
                        break;
                    case "Tile_Plains":
                        mapData += "P";
                        break;
                    case "Tile_Snow":
                        mapData += "S";
                        break;
                    case "Tile_Water":
                        mapData += "W";
                        break;
                }
            }
            mapData += "\n";
        }
        Debug.Log(mapData);
    }

    private void SpawnCity(GameObject cityGO, GameObject unitGO, UnitType unitType, string name, Player owner, int x, int y)
    {
        Vector3Int SpawnPositionInt = new Vector3Int(x, y, 0);
        Vector3 SpawnPositionWorld = map.CellToWorld(SpawnPositionInt);
        GameObject SpawnedCityGO = Instantiate(cityGO, SpawnPositionWorld, Quaternion.identity);
        City city = SpawnedCityGO.GetComponent<City>();
        city.Id = idCounter++;
        city.UnitType = unitType;
        city.Name = name;
        city.Owner = owner;
        city.Position = SpawnPositionInt;
        city.unitGO = unitGO;
        mapCities[x, y] = city;
        citiesList.Add(city);
        DBManager.Instance.InsertCity(city);
    }

    private void SpawnUnitAt(GameObject unitGO, UnitType type, Player owner, int x, int y)
    {
        Vector3Int SpawnPositionInt = new Vector3Int(x, y, 0);
        Vector3 SpawnPositionWorld = map.CellToWorld(SpawnPositionInt);
        GameObject SpawnedUnitGO = Instantiate(unitGO, SpawnPositionWorld, Quaternion.identity);
        Unit unit = SpawnedUnitGO.GetComponent<Unit>();
        unit.Id = idCounter++;
        unit.Type = type;
        unit.Owner = owner;
        unit.Hp = type.HpMax;
        unit.Movement = type.MovementMax;
        unit.Position = SpawnPositionInt;
        mapUnits[x, y] = unit;
        GameObject SpawnedHealthBarGO = Instantiate(GOHealthBar, SpawnPositionWorld, Quaternion.identity);
        SpawnedHealthBarGO.transform.SetParent(SpawnedUnitGO.transform);
        unit.Slider = SpawnedHealthBarGO.GetComponentInChildren<Slider>();
        unitsList.Add(unit);
        DBManager.Instance.InsertUnit(unit);
    }

    public bool SpawnUnitClosestTo(GameObject unitGO, UnitType type, Player owner, int x, int y)
    {
        if (mapUnits[x, y] == null){
            SpawnUnitAt(unitGO, type, owner, x, y);
            return true;
        } else
        {
            foreach (Vector3Int v3 in GetNeighbors(new Vector3Int(x,y,0)))
            {
                if (mapUnits[v3.x, v3.y] == null && mapTerrains[v3.x, v3.y].MovementCost >= 0 && !mapTerrains[v3.x, v3.y].IsNavegable)
                {
                    SpawnUnitAt(unitGO, type, owner, v3.x, v3.y);
                    return true;
                }
            }
        }
        return false;
    }

    public Vector3Int GetWorldToCellPosition(Vector3 position)
    {
        Vector3Int cellPosition = map.WorldToCell(position);
        cellPosition.z = 0;
        return cellPosition;
    }

    public bool TryToMoveUnitTo(Unit selectedUnit, Vector3Int cellPosDestination)
    {
        TerrainCiv destinationTerrain = mapTerrains[cellPosDestination.x, cellPosDestination.y];
        int destinationMovementCost = destinationTerrain.MovementCost;
        if (destinationMovementCost < 0 || destinationTerrain.IsNavegable) // Impassable terrain such as mountain or ocean
        {
            return false;
        }
        Vector3Int selectedUnitPos = map.WorldToCell(selectedUnit.gameObject.transform.position);
        if (selectedUnit.Movement >= destinationMovementCost && tile_distance(selectedUnitPos, cellPosDestination) == 1)
        {
            Unit otherUnit = mapUnits[cellPosDestination.x, cellPosDestination.y];
            if (otherUnit == null)
            {
                MoveUnitTo(selectedUnit, destinationMovementCost, selectedUnitPos, cellPosDestination);
            }
            else
            {
                if (selectedUnit.Owner == otherUnit.Owner)
                {
                    int originMovementCost = mapTerrains[selectedUnitPos.x, selectedUnitPos.y].MovementCost;
                    if (otherUnit.Movement >= originMovementCost)
                    {
                        SwapUnitsPosition(selectedUnit, otherUnit, destinationMovementCost, originMovementCost, cellPosDestination, selectedUnitPos);
                    }
                } else
                {
                    Debug.Log($"Combat between {selectedUnit} and {otherUnit}");
                    int difference = selectedUnit.Type.Combat - otherUnit.Type.Combat;
                    float random = UnityEngine.Random.Range(0.8f, 1.2f);
                    int damageToDefender = (int) (30 * Mathf.Exp(0.04f * difference) * random);
                    int damageToAttacker = (int) (30 * Mathf.Exp(0.04f * (-difference)) * random);
                    
                    bool selectedSurvived = selectedUnit.Damage(damageToAttacker);
                    bool otherSurvived = otherUnit.Damage(damageToDefender);
                    if (!selectedSurvived)
                    {
                        DestroyUnit(selectedUnit, selectedUnitPos);
                    }
                    if (!otherSurvived)
                    {
                        DestroyUnit(otherUnit, cellPosDestination);
                        if (selectedSurvived)
                        {
                            MoveUnitTo(selectedUnit, destinationMovementCost, selectedUnitPos, cellPosDestination);
                        }
                    }
                    selectedUnit.endMovement();
                }
            }
            return true;
        }
        return false;
    }

    private void DestroyUnit(Unit unit, Vector3Int pos)
    {
        mapUnits[pos.x, pos.y] = null;
        Destroy(unit.gameObject);
        unitsList.Remove(unit);
        DBManager.Instance.DeleteUnit(unit);
    }

    private void MoveUnitTo(Unit unit, int moveCost, Vector3Int originCell, Vector3Int destinationCell)
    {
        mapUnits[originCell.x, originCell.y] = null;
        mapUnits[destinationCell.x, destinationCell.y] = unit;
        unit.gameObject.transform.position = map.CellToWorld(destinationCell);
        unit.MoveTo(destinationCell, moveCost);
    }

    private void SwapUnitsPosition(Unit unit1, Unit unit2, int move1Cost, int move2Cost, Vector3Int pos1, Vector3Int pos2)
    {
        mapUnits[pos1.x, pos1.y] = unit1;
        mapUnits[pos2.x, pos2.y] = unit2;
        unit1.gameObject.transform.position = map.CellToWorld(pos1);
        unit2.gameObject.transform.position = map.CellToWorld(pos2);
        unit1.MoveTo(pos1, move1Cost);
        unit2.MoveTo(pos2, move2Cost);
    }

    public void AdvanceTurn()
    {
        foreach (Unit unit in unitsList)
        {
            if (unit.Owner == player)
            {
                unit.Heal(healingValue);
            }
            unit.ResetMovement();
        }
        foreach (City city in citiesList)
        {
            city.AdvanceTurn();
        }
    }

    public void EnemyAI()
    {
        for (int i = unitsList.Count - 1; i >= 0; i--)
        {
            Unit unit = unitsList[i];
            if (unit.Owner == enemy)
            {
                AI ai = new();
                List<Vector3Int> path = ai.CalculatePath(unit);
                int index = path.Count - 2;
                bool couldMove = true;
                if (path.Count < 2)
                {
                    Debug.Log(unit + " could not move");
                    couldMove = false;
                }
                while (couldMove && unit.Movement > 0) {
                    couldMove = TryToMoveUnitTo(unit, path[index]);
                    index--;
                    if (index < 0)
                    {
                        couldMove = false;
                    }
                }
            }
        }
    }

    public bool CheckVictoryCondition()
    {
        foreach (Unit unit in unitsList)
        {
            if (unit.Owner == enemy)
            {
                return false;
            }
        }
        return true;
    }

    public List<Vector3Int> GetValidNeighbors(Vector3Int hex)
    {
        List<Vector3Int> neighbors = GetNeighbors(hex);
        for (int i = neighbors.Count - 1; i >= 0; i--)
        {
            Vector3Int n = neighbors[i];
            if (n.x < 0 || n.x >= width || n.y < 0 || n.y >= height || mapTerrains[n.x, n.y].MovementCost < 0 || mapTerrains[n.x, n.y].IsNavegable)
            {
                neighbors.Remove(n);
            }
        }
        return ShuffleList(neighbors);
    }

    // Code based on https://www.redblobgames.com/grids/hexagons/
    private int tile_distance(Vector3Int h1, Vector3Int h2)
    {
        Cube a = oddr_to_cube(h1);
        Cube b = oddr_to_cube(h2);
        var vec = cube_subtract(a, b);
        return (Math.Abs(vec.q) + Math.Abs(vec.r) + Math.Abs(vec.s)) / 2;
    }

    private Cube cube_subtract(Cube a, Cube b)
    {
        return new Cube(a.q - b.q, a.r - b.r, a.s - b.s);
    }

    private Cube oddr_to_cube(Vector3Int hex)
    {
        int q = hex.x - (hex.y - (hex.y & 1)) / 2;
        int r = hex.y;
        return new Cube(q, r, -q - r);
    }

    private List<Vector3Int> GetNeighbors(Vector3Int hex)
    {
        List<Vector3Int> list = new()
        {
            new Vector3Int(hex.x - 1, hex.y, 0),
            new Vector3Int(hex.x + 1, hex.y, 0),
            new Vector3Int(hex.x, hex.y - 1, 0),
            new Vector3Int(hex.x, hex.y + 1, 0)
        };
        if (hex.y % 2 == 0)
        {
            list.Add(new Vector3Int(hex.x - 1, hex.y - 1, 0));
            list.Add(new Vector3Int(hex.x - 1, hex.y + 1, 0));
        } else
        {
            list.Add(new Vector3Int(hex.x + 1, hex.y - 1, 0));
            list.Add(new Vector3Int(hex.x + 1, hex.y + 1, 0));
        }
        return list;
    }

    struct Cube
    {
        public int q;
        public int r;
        public int s;

        public Cube(int q, int r, int s) : this()
        {
            this.q = q;
            this.r = r;
            this.s = s;
        }
    }

    // Knuth Shuffle Algorithm
    private List<Vector3Int> ShuffleList(List<Vector3Int> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int j = UnityEngine.Random.Range(0, i + 1);
            Vector3Int temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
        return list;
    }
}
