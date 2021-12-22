using System;

namespace ModelosDto
{
    public class ArchivoCreateDto
    {
        public string Nombre { get; set; }
        public string Rutas { get; set; }
        public int NumPages { get; set; }
        public string PublicFile { get; set; }
        public DateTime? FechaUpload { get; set; }
    }
}