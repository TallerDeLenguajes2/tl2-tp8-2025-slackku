using System.ComponentModel.DataAnnotations;
using tl2_tp8_2025_slackku.Models;

namespace tl2_tp8_2025_slackku.ViewModels;

public class PresupuestoViewModel
{
    public int IdPresupuesto { get; set; }

    // Validación: Requerido
    [Display(Name = "Nombre")]
    [Required(ErrorMessage = "El nombre o email es obligatorio.")]
    public string NombreDestinatario { get; set; }

    // Validación: Requerido y tipo de dato
    [Display(Name = "Fecha de Creación")]
    [Required(ErrorMessage = "La fecha es obligatoria.")]
    [DataType(DataType.Date)]
    public DateTime FechaCreacion { get; set; }
}