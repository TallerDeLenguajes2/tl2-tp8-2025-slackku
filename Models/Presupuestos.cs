namespace Models
{
    public class Presupuesto
    {
        private int _idPresupuesto;
        private string _nombreDestinatario;
        private DateTime _fechaCreacion;
        private List<PresupuestoDetalle> _detalle;

        public int IdPresupuesto
        {
            get { return _idPresupuesto; }
            set { _idPresupuesto = value; }
        }

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
        public double MontoPresupuesto()
        {
            return Detalle.Sum(d => d.Cantidad * d.Producto.Precio);
        }
        public double MontoPresupuestoConIva() // IVA 21
        {
            double monto = MontoPresupuesto();
            return monto + (monto * 0.21);
        }
        public int CantidadProductos()
        {
            return Detalle.Sum(d => d.Cantidad);
        }
    }
}