using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.IO;
using System.Collections.Generic;

public class DBManager : MonoBehaviour
{
    private string dbBaseUri = "URI=file:";
    private string dbName = "game.sqlite";
    private IDbConnection dbConnection;
    public static DBManager Instance { get; private set; }
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

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /* 
         * https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-persistentDataPath.html
        */
        string path = Path.Combine(Application.persistentDataPath, dbName);
        string dbUri = dbBaseUri + path;
        DeleteDB(path);
        Debug.Log("DB en " + path);
        dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        CreateTables();
        PopulateDB();
    }

    private void DeleteDB(string path)
    {
        File.Delete(path);
        Debug.Log("DB Deleted");
    }

    private void OnDestroy()
    {
        dbConnection.Close();
    }

    private void CreateTables()
    {
        /* 
         * https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-dataPath.html
        */
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = GameManager.Instance.ReadFromFile(Path.Combine(Application.dataPath, "Sql", "ddl.sql"));
        dbCommand.ExecuteReader();
    }

    private void PopulateDB()
    {
        if (GetNumberOfElementsFromTable("UnitsTypes") != 0)
        {
            return;
        }
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = GameManager.Instance.ReadFromFile(Path.Combine(Application.dataPath, "Sql", "dml.sql"));
        dbCommand.ExecuteNonQuery();
        Debug.Log("Added initial data to DB");
    }

    private int GetNumberOfElementsFromTable(string tableName)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string command = "SELECT count(*) FROM " + tableName;
        dbCommand.CommandText = command;
        IDataReader reader = dbCommand.ExecuteReader();
        reader.Read();
        return reader.GetInt32(0);
    }

    public Dictionary<string, TerrainCiv> GetTerrains()
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string command = "SELECT * FROM Terrains;";
        dbCommand.CommandText = command;
        IDataReader reader = dbCommand.ExecuteReader();
        Dictionary<string, TerrainCiv> terrains = new();
        while (reader.Read())
        {
            string letter = reader.GetString(2);
            TerrainCiv t = new TerrainCiv(reader.GetInt32(0), reader.GetString(1), letter,
                 reader.GetBoolean(3), reader.GetInt32(4), reader.GetInt32(5));
            terrains.Add(letter, t);
        }
        return terrains;
    }

    public List<UnitType> GetUnitsTypes()
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = "SELECT * FROM UnitsTypes;";
        IDataReader reader = dbCommand.ExecuteReader();
        List<UnitType> unitsTypes = new();
        while (reader.Read())
        {
            UnitType t = new UnitType(reader.GetInt32(0), reader.GetString(1), reader.GetInt32(2),
                 reader.GetInt32(3), reader.GetInt32(4), reader.GetInt32(5));
            unitsTypes.Add(t);
        }
        return unitsTypes;
    }

    public void InsertUnit(Unit unit)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"INSERT INTO Units VALUES ({unit.Id},'{unit.Type.Name}',{unit.Movement},{unit.Hp},{unit.Type.Id},{unit.Owner.Id},{unit.Position.x},{unit.Position.y});";
        dbCommand.ExecuteNonQuery();
    }

    public void UpdateUnitHp(Unit unit)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"UPDATE Units SET Hp={unit.Hp},MovementLeft={unit.Movement} WHERE Id={unit.Id};";
        dbCommand.ExecuteNonQuery();
    }

    public void UpdateUnitPosition(Unit unit)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"UPDATE Units SET X={unit.Position.x},Y={unit.Position.y},MovementLeft={unit.Movement} WHERE Id={unit.Id};";
        dbCommand.ExecuteNonQuery();
    }

    public void UpdateUnitMovement(Unit unit)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"UPDATE Units SET MovementLeft={unit.Movement} WHERE Id={unit.Id};";
        dbCommand.ExecuteNonQuery();
    }

    public void DeleteUnit(Unit unit)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"DELETE FROM Units WHERE Id={unit.Id};";
        dbCommand.ExecuteNonQuery();
    }

    public void InsertPlayer(Player player)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"INSERT INTO Players VALUES ({player.Id},'{player.Name}');";
        dbCommand.ExecuteNonQuery();
    }

    public void InsertCity(City city)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = $"INSERT INTO Cities VALUES ({city.Id},'{city.Name}',{city.Owner.Id},{city.Position.x},{city.Position.y});";
        dbCommand.ExecuteNonQuery();
    }

    public void SaveMap(TerrainCiv[,] terrains)
    {
        string command = "INSERT INTO Map VALUES ";
        int width = terrains.GetLength(0);
        int height = terrains.GetLength(1);
        for (int i = 0; i < width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                int terrainId = terrains[i, j].Id;
                command += $"({i},{j},{terrainId}),";
            }
        }
        command = command.Remove(command.Length - 1, 1);
        command += ';';
        IDbCommand dbCommand = dbConnection.CreateCommand();
        dbCommand.CommandText = command;
        dbCommand.ExecuteNonQuery();
    }
}
