using System;

namespace Common
{
    public static class DateTimeConverterExtensions
    {
        public static DateTime ToMexico(this DateTime c) => TimeZoneInfo.Local.DisplayName != "Central Standard Time(Mexico)" ? TimeZoneInfo.ConvertTimeBySystemTimeZoneId(c, "Central Standard Time (Mexico)") : c;

        public static string ToUSA(this DateTime c)
        {
            return c.ToString("yyyy-MM-dd");
            /*   try{
      return  TimeZoneInfo.Local.DisplayName != "Central Standard Time(Mexico)" ? TimeZoneInfo.ConvertTimeBySystemTimeZoneId(c, "(UTC-06:00) hora estándar central") :c;
               }
               catch
               {
    return  TimeZoneInfo.Local.DisplayName != "Central Standard Time(Mexico)" ? TimeZoneInfo.ConvertTimeBySystemTimeZoneId(c, "(UTC-06:00) hora estándar central") :c;
               }*/
        }
        public static string ToUSA(this DateTime? c)
        {
            string valor = "";
            if (c != null)
                valor=c.Value.ToString("yyyy-MM-dd");
            return valor;
           
        }
    }
}