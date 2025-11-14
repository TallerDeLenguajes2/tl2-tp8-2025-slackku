namespace tl2_tp8_2025_slackku.Models;

public class ProductoDTO
{
    private string _descripcion;
    private double _precio;
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