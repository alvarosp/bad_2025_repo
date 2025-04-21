using System;
using System.Collections.Generic;

public class Pedido
{
    public int Id { get; private set; }

    public int Mesa {  get; private set; }
    public DateTime Fecha { get; private set; }
    private List<ProductoCantidad> productos;

    public Pedido(int id, int mesa, DateTime fecha)
    {
        Id = id;
        Mesa = mesa;
        Fecha = fecha;
        productos = new List<ProductoCantidad>();
    }

    public void AddProducto(Producto producto, int cantidad)
    {
        productos.Add(new ProductoCantidad(producto, cantidad));
    }

    public string GetProductosSQL()
    {
        string sql = "INSERT INTO 'Pedidos-Productos' VALUES ";
        foreach (ProductoCantidad p in productos)
        {
            sql += $"({Id},{p.producto.Id},{p.cantidad}),";
        }
        sql = sql.Remove(sql.Length - 1, 1);
        sql += ';';
        return sql;
    }

    public struct ProductoCantidad
    {
        public Producto producto;
        public int cantidad;

        public ProductoCantidad(Producto producto, int cantidad)
        {
            this.producto = producto;
            this.cantidad = cantidad;
        }
    }
}
