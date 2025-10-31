namespace tl2_tp8_2025_slackku.Models;
public class Producto
{
    private int _idProducto;
    private string _descripcion;
    private double _precio;

    public int IdProducto
    {
        get { return _idProducto; }
        set { _idProducto = value; }
    }

    public string Descripcion
    {
        get { return _descripcion; }
        set { _descripcion = value; }
    }

    public double Precio
    {
        get { return _precio; }
        set { _precio = value; }
    }

}