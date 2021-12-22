using System;

namespace ModelosDto
{
    public class MyTokenDto
    {
        public string MyKey { get; set; }
        public string Code { get; set; }

        public string UserId { get; set; }
        public DateTime DateExpired { get; set; }
    }
}

