using System;

namespace Common.Atribute
{
    public class MyOrdenAttribute : Attribute
    {
        public int Orden { get; }

        public MyOrdenAttribute(int orden)
        {
            Orden = orden;
        }
    }
}