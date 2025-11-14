namespace tl2_tp8_2025_slackku.Models;

public class PresupuestoDetalle
{
    private Producto _producto;
    private int _cantidad;
    private int _idPresupuesto;

    public Producto Producto
    {
        get { return _producto; }
        set { _producto = value; }
    }

    public int Cantidad
    {
        get { return _cantidad; }
        set { _cantidad = value; }
    }

    public int IdPresupuesto
    {
        get { return _idPresupuesto; }
        set { _idPresupuesto = value; }
    }
}