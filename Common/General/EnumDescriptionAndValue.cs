using System.Collections.Generic;

namespace Common
{
    public class EnumDescriptionAndValue
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Description { get; set; }
        public int Order { get; set; }
        public List<EnumDescriptionAndValue> Hijos { get; set; }

        public EnumDescriptionAndValue()
        {
            Hijos = new List<EnumDescriptionAndValue>();
        }
    }

}
