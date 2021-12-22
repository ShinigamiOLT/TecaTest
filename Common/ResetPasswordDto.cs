namespace Common
{
    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public Plantilla Plantilla { get; set; }

        public ResetPasswordDto()
        {
            Plantilla = new Plantilla();
        }
    }
}