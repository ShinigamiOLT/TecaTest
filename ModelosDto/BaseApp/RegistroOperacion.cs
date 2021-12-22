namespace ModelosDto
{
    public class RegistroOperacion
    {
        public bool  Success { get; set; }
        public decimal SaldoInicial { get; set; }
        public decimal MontoTransaccion { get; set; }
        public decimal SaldoFinal { get; set; }
        public string Message { get; set; }
    }
}