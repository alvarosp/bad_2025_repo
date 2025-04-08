using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System.Globalization;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using NUnit.Framework;

public class DBManager : MonoBehaviour
{
    private string dbUri = "URI=file:mydb.sqlite";
    private string SQL_CREATE_PRODUCTS = "CREATE TABLE IF NOT EXISTS Productos" +
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
    private int[] PRODUCTOS_TIPOS = { 0, 1, 2, 3, 4, 5, 6, 3 };
    IDbConnection dbConnection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        dbConnection = new SqliteConnection(dbUri);
        dbConnection.Open();
        createTables();
        populateDB();
        searchByPriceMin(4f);
        dbConnection.Close();
    }

    private void searchByPriceMin(float price)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string command = "SELECT * FROM Productos WHERE Precio >= " + price;
        dbCommand.CommandText = command;
        IDataReader reader = dbCommand.ExecuteReader();
        List<Producto> productos = new();
        while (reader.Read())
        {
            productos.Add(new Producto(reader.GetInt32(0), reader.GetString(1), 
                reader.GetInt32(2), reader.GetFloat(3), reader.GetInt32(4)));
        }
        Debug.Log(listToString(productos));
    }

    private string listToString(List<Producto> list)
    {
        string data = "";
        foreach (var item in list)
        {
            data += item.ToString() + "\n";
        }
        return data;
    }

    private void populateDB()
    {
        int numeroElementos = getNumberOfElements("Tipos");
        if (numeroElementos != 0)
        {
            return;
        }
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

        IDbCommand dbCommand3 = dbConnection.CreateCommand();
        command = "INSERT INTO Productos (Nombre, Cantidad, Precio, TipoId) VALUES ";
        for (int i = 0; i < PRODUCTOS.Length; i++)
        {
            // Especificamos que se ignore el Locale que tengamos configurado para que se use el punto como separador decimal
            string precio = Random.Range(0.5f, 8f).ToString("F3", CultureInfo.InvariantCulture);
            command += $"('{PRODUCTOS[i]}',{Random.Range(0, 5)},{precio},{PRODUCTOS_TIPOS[i]}),";
        }
        command = command.Remove(command.Length - 1, 1);
        command += ';';
        Debug.Log(command);
        dbCommand3.CommandText = command;
        dbCommand3.ExecuteNonQuery();
    }

    private int getNumberOfElements(string tableName)
    {
        IDbCommand dbCommand4 = dbConnection.CreateCommand();
        string command = "SELECT count(*) FROM " + tableName;
        dbCommand4.CommandText = command;
        IDataReader reader = dbCommand4.ExecuteReader();
        reader.Read();
        return reader.GetInt32(0);
    }

    private void createTables()
    {
        IDbCommand dbCommand1 = dbConnection.CreateCommand();
        dbCommand1.CommandText = SQL_CREATE_PRODUCTS + SQL_CREATE_TIPOS;
        dbCommand1.ExecuteReader();
    }
}
