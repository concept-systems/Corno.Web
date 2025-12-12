using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO.Ports;
using System.Linq;

namespace Corno.Web.Extensions
{
    public static class ConverterExtensions
    {
        public static object ToObject(this object obj)
        {
            return obj;
        }

        public static byte ToByte(this object obj)
        {
            byte byteValue = 0;
            if (obj != null)
                byte.TryParse(obj.ToString(), out byteValue);

            return byteValue;
        }

        public static int ToUInt(this object obj)
        {
            var intValue = 0;
            if (obj != null)
                int.TryParse(obj.ToString(), out intValue);

            return intValue;
        }

        public static ushort ToUShort(this object obj)
        {
            ushort intValue = 0;
            if (obj != null)
                ushort.TryParse(obj.ToString(), out intValue);

            return intValue;
        }


        public static int ToInt(this object obj)
        {
            var intValue = 0;
            if (obj != null)
                int.TryParse(obj.ToString(), out intValue);

            return intValue;
        }

        public static long ToLong(this object obj)
        {
            long intValue = 0;
            if (obj != null)
                long.TryParse(obj.ToString(), out intValue);

            return intValue;
        }

        public static double ToDouble(this object obj)
        {
            double doubleValue = 0;
            if (obj == null) return doubleValue;

            double.TryParse(obj.ToString(), out doubleValue);
            var strValue = doubleValue.ToString("F3");
            return Convert.ToDouble(strValue);
        }

        public static double ToDouble(this object obj, int decimalPoints)
        {
            double doubleValue = 0;
            if (obj == null) return doubleValue;

            double.TryParse(obj.ToString(), out doubleValue);
            var strValue = doubleValue.ToString("F" + decimalPoints);
            return Convert.ToDouble(strValue);
        }

        public static decimal ToDecimal(this object obj)
        {
            decimal value = 0;
            if (obj != null)
                decimal.TryParse(obj.ToString(), out value);
            return value;
        }

        public static decimal ToDecimalInteger(this object obj)
        {
            decimal value = 0;
            if (obj != null)
                decimal.TryParse(obj.ToString(), NumberStyles.Integer,
                    CultureInfo.CurrentCulture.NumberFormat, out value);
            return value;
        }

        public static string ToString(this object obj)
        {
            var value = "";
            if (obj != null)
                value = Convert.ToString(obj);

            return value;
        }

        public static bool ToBoolean(this object obj)
        {
            if (obj == null)
                return false;

            bool.TryParse(obj.ToString(), out var value);
            return value;
        }

        public static DateTime ToDateTime(this object obj)
        {
            if (obj == null)
                return new DateTime(1900, 1, 1);

            DateTime.TryParse(obj.ToString(), out var value);
            if (value.Year == 1)
                value = Convert.ToDateTime(obj, CultureInfo.InvariantCulture);
            return value;
        }

        public static Parity ToParity(this object obj)
        {
            var parity = Parity.None;
            if (obj != null)
                //Parity.TryParse(obj.ToString(), out parity);
                Enum.TryParse(obj.ToString(), out parity);

            return parity;
        }

        public static StopBits ToStopBits(this object obj)
        {
            var stopBits = obj.ToDouble();
            switch (stopBits)
            {
                case 1:
                    return StopBits.One;
                case 2:
                    return StopBits.Two;
                case 1.5:
                    return StopBits.OnePointFive;
                default:
                    return StopBits.None;
            }
        }

        public static string FormatDate(this DateTime dateTime)
        {
            var date = dateTime.Day + //dd
                       "/" + dateTime.Month + //mm
                       "/" + dateTime.Year; //yyyy

            return date;
        }

        public static List<TEntity> Trim<TEntity>(this List<TEntity> collection)
        {
            if (null == collection) return null;

            //collection = collection;

            var type = typeof(TEntity);

            var properties = TypeDescriptor.GetProperties(type).Cast<PropertyDescriptor>()
                .Where(p => p.PropertyType == typeof(string)).ToList();

            foreach (var entity in collection)
            {
                foreach (var property in properties)
                {
                    var value = (string)property.GetValue(entity);

                    if (string.IsNullOrEmpty(value)) continue;

                    value = value.Trim();
                    property.SetValue(entity, value);
                }
            }

            return collection;
        }
    }
}