using System.ComponentModel.DataAnnotations;
namespace tl2_tp8_2025_slackku.Models;

public class PresupuestoDetalle
{
    private Producto _producto;
    private int _cantidad;

    public Producto Producto
    {
        get { return _producto; }
        set { _producto = value; }
    }

    [Required(ErrorMessage = "La cantidad es obligatoria.")]
    [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
    public int Cantidad
    {
        get { return _cantidad; }
        set { _cantidad = value; }
    }

}