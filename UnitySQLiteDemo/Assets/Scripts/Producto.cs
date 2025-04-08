using UnityEngine;

public class Producto
{
    public int id { get; private set; }
    public string nombre { get; private set; }
    public int cantidad { get; private set; }
    public float precio { get; private set; }
    public int tipo { get; private set; }

    public Producto(int id, string nombre, int cantidad, float precio, int tipo)
    {
        this.id = id;
        this.nombre = nombre;
        this.cantidad = cantidad;
        this.precio = precio;
        this.tipo = tipo;
    }

    public override string ToString()
    {
        return $"id: {id}, nombre: {nombre}, cantidad: {cantidad}, precio: {precio}, tipo: {tipo}";
    }
}
