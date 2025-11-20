using System.ComponentModel.DataAnnotations;

namespace tl2_tp8_2025_slackku.ViewModels;

public class ProductoViewModel
{
    public int IdProducto { get; set; }

    // Validación: Máximo 250 caracteres. Es opcional por defecto si no tiene [Required]
    [Display(Name = "Descripción del Producto")]
    [StringLength(250, ErrorMessage = "La descripción no puede superar los 250 caracteres.")]
    public string Descripcion { get; set; }

    // Validación: Requerido y debe ser positivo
    [Display(Name = "Precio Unitario")]
    [Required(ErrorMessage = "El precio es obligatorio.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser un valor positivo.")]
    public double Precio { get; set; }
}