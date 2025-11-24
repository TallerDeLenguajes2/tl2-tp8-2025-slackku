using tl2_tp8_2025_slackku.Models;
namespace tl2_tp8_2025_slackku.Interfaces;

public interface IPresupuestoRepository
{
    bool Crear(Presupuesto prod);
    public Presupuesto Obtener(int id);
    public bool Modificar(Presupuesto pModified);
    public bool Eliminar(int id);
    public List<Presupuesto> Listar();
    public bool AgregarDetalle(int idPresupuesto, int idProducto, int cantidad);
    public bool EliminarDetalle(int idPresupuesto, int idProducto);
}