using tl2_tp8_2025_slackku.Models;
namespace tl2_tp8_2025_slackku.Interfaces;

public interface IProductoRepository
{
    bool Crear(Producto prod);
    public Producto Obtener(int id);
    public bool Modificar(Producto pModified);
    public bool Eliminar(int id);
    public List<Producto> Listar();
}