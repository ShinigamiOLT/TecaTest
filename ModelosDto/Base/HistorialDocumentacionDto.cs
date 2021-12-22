using System;

namespace ModelosDto
{
    public class HistorialDocumentacionDto
    {
         public string Fecha { get; set; }
        public string Log { get; set; }

        public override string ToString()
        {
            return $"{Fecha} {Log}";
        }
    }
}