using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DBManager : MonoBehaviour
{
    private string dbUri = "URI=file:mydb.sqlite";
    private string SQL_CREATE_PRODUCTS = "CREATE TABLE IF NOT EXISTS Products" +
        "(Id INTEGER UNIQUE NOT NULL PRIMARY KEY" +
        ",Nombre TEXT NOT NULL" +
        ",Cantidad INTEGER NOT NULL" +
        ",Precio NUMERIC NOT NULL" +
        ",TipoId INTEGER NOT NULL REFERENCES Tipos);";
    private string SQL_CREATE_TIPOS = "CREATE TABLE IF NOT EXISTS Tipos" +
        "(Id INTEGER UNIQUE NOT NULL PRIMARY KEY" +
        ",Nombre TEXT NOT NULL);";
    private string[] TIPOS = { "Lacteo", "Fruta", "Vegetal", "Cereal", "Carne", "Bebida", "Pescado" };
    private string[] PRODUCTOS = { "Queso", "Manzanas", "Judías", "Pan", "Chuletas", "Agua", "Lubina", "Muesli" };

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IDbConnection dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        IDbCommand dbCommand1 = dbConnection.CreateCommand();
        dbCommand1.CommandText = SQL_CREATE_PRODUCTS + SQL_CREATE_TIPOS;
        dbCommand1.ExecuteReader();
        IDbCommand dbCommand2 = dbConnection.CreateCommand();
        string command = "INSERT INTO Tipos (Nombre) VALUES ";
        for (int i = 0; i < TIPOS.Length; i++)
        {
            command += $"('{TIPOS[i]}'),";
        }
        command = command.Remove(command.Length - 1, 1);
        command += ';';
        Debug.Log(command);
        dbCommand2.CommandText = command;
        dbCommand2.ExecuteNonQuery();
        dbConnection.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
