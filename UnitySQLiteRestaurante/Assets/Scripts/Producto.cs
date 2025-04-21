using UnityEngine;

public class Producto
{
    public int Id { get; private set; }
    public string Nombre { get; private set; }
    public float Precio { get; private set; }

    public Producto(int id, string nombre, float precio)
    {
        this.Id = id;
        this.Nombre = nombre;
        this.Precio = precio;
    }

    public override string ToString()
    {
        return $"id: {Id}, nombre: {Nombre}, precio: {Precio}";
    }
}
