namespace Common
{
    public class Response<T> where T : class, new()
    {
        public bool Success { get; set; }
        public string Msg => EstableceMsg();

        public int Code { get; set; }

        private string Msj { get; set; }

        public T Data { get; set; }

        public void Error(string cad)
        {
            Success = false;
            Code = -1;
            Msj += cad;
        }

        public void Custom(string cad, int cod = -2)
        {
            Success = cod == 0;
            Code = cod;
            Msj = cad;
        }

        private string EstableceMsg()
        {
            switch (Code)
            {
                case -1:
                    Msj += " .::Error::. ";
                    break;

                case 0:
                    Msj = "Acceso correcto";
                    break;

                case 1:
                    Msj = "Contraseña o email incorrecto";
                    break;

                case 2:
                    Msj = "Correo no confirmado";
                    break;

                case 3:
                    Msj = "Cuenta bloqueado";
                    break;

                case 4:
                    Msj = "Error al crear esta cuenta";
                    break;

                case 5:
                    Msj = "Correo verificado";
                    break;

                case 6:
                    Msj = "Token no valido o expirado";
                    break;

                case 7:
                    Msj = "Contraseña reseteado";
                    break;

                case 8:
                    Msj = "Correo enviado para resetear contraseña";
                    break;

                case 9:
                    Msj = "Avatar Actualizado";
                    break;
            }

            return Msj;
        }

        public Response()
        {
            Data = new T();

            Success = false;
            Code = 0;
            Msj = "";
        }
    }
}