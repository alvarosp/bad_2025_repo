using UnityEngine;
using Mono.Data.Sqlite;
using System.Data;

public class DBManager : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        IDbConnection dbConnection = new SqliteConnection("URI=file:mydb.sqlite");
        dbConnection.Open();
        dbConnection.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
