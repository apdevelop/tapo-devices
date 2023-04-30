using System;
using System.ComponentModel;
using System.Linq;

namespace TapoDevices
{
    static class EnumExtensionMethods
    {
        public static string GetDescription(this Enum genericEnum)
        {
            var genericEnumType = genericEnum.GetType();
            var memberInfo = genericEnumType.GetMember(genericEnum.ToString());
            if ((memberInfo != null) && (memberInfo.Length > 0))
            {
                var attribs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if ((attribs != null) && (attribs.Count() > 0))
                {
                    return ((DescriptionAttribute)attribs.ElementAt(0)).Description;
                }
            }

            return genericEnum.ToString();
        }
    }
}
