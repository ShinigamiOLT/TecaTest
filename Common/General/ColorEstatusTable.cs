using System.Drawing;
using Common.Atribute;

namespace Common
{
    public class ColorEstatusTable
    {
        [NoVisible] public string ColorNotificacion { get; set; }
        [NoVisible] public object rowColor => new { Color = !string.IsNullOrEmpty(ColorNotificacion) ? ColorNotificacion : Color.Green.ToHexString() };
    }
}