using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Globalization;
using System.Xml.Serialization;

public class DBManager : MonoBehaviour
{
    private string dbBaseUri = "URI=file:";
    IDbConnection dbConnection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        /* 
         * https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-persistentDataPath.html
        */
        string path = dbBaseUri + Path.Combine(Application.persistentDataPath, "restaurante.db");
        Debug.Log("DB en " + path);
        dbConnection = new SqliteConnection(path);
        dbConnection.Open();
        createTables();
        populateDB();
        Dictionary<int,Producto> productos = GetAllProducts();
        int numPedidos = getNumberOfElements("Pedidos");
        // Date and Time Function in SQLite: https://www.sqlite.org/lang_datefunc.html
        Pedido pedido1 = new Pedido(++numPedidos, 1, new DateTime(2025,04,01,12,30,0));
        pedido1.AddProducto(productos[1], 2);
        pedido1.AddProducto(productos[2], 1);
        pedido1.AddProducto(productos[3], 4);
        InsertPedido(pedido1);
        dbConnection.Close();
        SaveProductoXMLToFile(productos[1]);
        SavePedidoXMLToFile(pedido1);
        SaveProductoJSONToFile(productos[1]);
        SavePedidoJSONToFile(pedido1);
    }

    private void InsertPedido(Pedido pedido)
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string fecha = pedido.Fecha.ToUniversalTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        string hora = pedido.Fecha.ToUniversalTime().ToString("HH:mm", CultureInfo.InvariantCulture);
        dbCommand.CommandText = $"INSERT INTO Pedidos VALUES ({pedido.Id},{pedido.Mesa},'{fecha}','{hora}');";
        dbCommand.ExecuteNonQuery();
        dbCommand.CommandText = pedido.GetProductosSQL();
        dbCommand.ExecuteNonQuery();
    }

    private Dictionary<int,Producto> GetAllProducts()
    {
        IDbCommand dbCommand = dbConnection.CreateCommand();
        string command = "SELECT * FROM Productos;";
        dbCommand.CommandText = command;
        IDataReader reader = dbCommand.ExecuteReader();
        Dictionary<int, Producto> productos = new();
        while (reader.Read())
        {
            int id = reader.GetInt32(0);
            Producto p = new Producto(id, reader.GetString(1), reader.GetFloat(2));
            productos.Add(id, p);
        }
        return productos;
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
        dbCommand1.CommandText = readFromFile(Path.Combine("Sql", "ddl.sql"));
        dbCommand1.ExecuteReader();
    }

    private void populateDB()
    {
        int numeroElementos = getNumberOfElements("Productos");
        if (numeroElementos != 0)
        {
            return;
        }
        IDbCommand dbCommand2 = dbConnection.CreateCommand();
        dbCommand2.CommandText = readFromFile(Path.Combine("Sql", "dml.sql"));
        dbCommand2.ExecuteNonQuery();
    }

    private string readFromFile(string fileName)
    {
        /* https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-dataPath.html
        */
        string path = Path.Combine(Application.dataPath, fileName);
        StreamReader sr = new StreamReader(path);
        string fileContents = sr.ReadToEnd();
        sr.Close();
        return fileContents;
    }

    public static bool WriteToFile(string filename, string data)
    {
        string fullPath = Path.Combine(Application.persistentDataPath, filename);
        try
        {
            File.WriteAllText(fullPath, data);
            Debug.Log("Fichero guardado correctamente en: " + fullPath);
            return true;
        }
        catch (Exception e)
        {
            Debug.Log("Error al guardar el fichero en: " + fullPath + " con el error " + e);
            return false;
        }
    }

    private void SaveProductoXMLToFile(Producto producto)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Producto));
        String path = Path.Combine(Application.persistentDataPath, "producto.xml");
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, producto);
        }
    }

    private void SavePedidoXMLToFile(Pedido pedido)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(Pedido));
        String path = Path.Combine(Application.persistentDataPath, "pedido.xml");
        using (FileStream stream = new FileStream(path, FileMode.Create))
        {
            serializer.Serialize(stream, pedido);
        }
    }

    private void SaveProductoJSONToFile(Producto producto)
    {
        WriteToFile("producto.json", JsonUtility.ToJson(producto));
    }

    private void SavePedidoJSONToFile(Pedido pedido)
    {
        WriteToFile("pedido.json", JsonUtility.ToJson(pedido));
    }
}
