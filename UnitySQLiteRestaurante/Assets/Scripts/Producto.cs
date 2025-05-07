using System;
using UnityEngine;

[Serializable]
public class Producto
{
    public int Id;
    public string Nombre;
    public float Precio;

    public Producto(int id, string nombre, float precio)
    {
        this.Id = id;
        this.Nombre = nombre;
        this.Precio = precio;
    }

    public Producto() { }

    public override string ToString()
    {
        return $"id: {Id}, nombre: {Nombre}, precio: {Precio}";
    }
}
