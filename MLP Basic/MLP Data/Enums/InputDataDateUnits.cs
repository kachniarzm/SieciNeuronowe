using System;
using System.ComponentModel;
using System.Reflection;

namespace MLP_Data.Enums
{
    public enum InputDataDateUnits
    {
        Undefined,
        Day,
        Week,
        [Description("Two Weeks")]
        TwoWeeks,
        Month,
        [Description("Three Months")]
        ThreeMonths,
        Year
    }

    public static class InputDataDateUnitsExtension
    {
        // returns approximate days number
        public static int GetDaysNumber(this InputDataDateUnits dateUnit)
        {
            switch (dateUnit)
            {
                case InputDataDateUnits.Day:            return 1;
                case InputDataDateUnits.Week:           return 5;
                case InputDataDateUnits.TwoWeeks:       return 10;
                case InputDataDateUnits.Month:          return 20;
                case InputDataDateUnits.ThreeMonths:    return 60;
                case InputDataDateUnits.Year:           return 260;
                default: throw new ArgumentException("GetDaysNumber undefined enum");
            }
        }

        public static string GetDescription(this Enum en)
        {
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);
                if (attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }
            return en.ToString();
        }
    }
}
