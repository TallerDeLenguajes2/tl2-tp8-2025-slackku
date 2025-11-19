namespace tl2_tp8_2025_slackku.Models;

public class PresupuestoDTO
{
    private string _nombreDestinatario;
    private DateTime _fechaCreacion;
    private List<PresupuestoDetalle> _detalle;

    public string NombreDestinatario
    {
        get { return _nombreDestinatario; }
        set { _nombreDestinatario = value; }
    }
    public DateTime FechaCreacion
    {
        get { return _fechaCreacion; }
        set { _fechaCreacion = value; }
    }
    public List<PresupuestoDetalle> Detalle
    {
        get { return _detalle; }
        set { _detalle = value; }
    }
}