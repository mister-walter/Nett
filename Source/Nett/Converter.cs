﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Nett
{
    internal static class Converter
    {
        private static readonly Dictionary<Type, Dictionary<Type, Func<object, object>>> ConvertFunctions = new Dictionary<Type, Dictionary<Type, Func<object, object>>>()
        {
            { typeof(long), new Dictionary<Type, Func<object, object>>()
                {
                    { typeof(int), (l) => (int)(long)l },
                }
            },
            { typeof(TomlArray), new Dictionary<Type, Func<object, object>>()
                {
                    { typeof(int[]), (a) => ((TomlArray)a).To<int>().ToArray() }
                }
            }
        };

        public static TRes Convert<TRes>(object src)
        {
            if(typeof(TRes) == src.GetType())
            {
                return (TRes)src;
            }

            Dictionary<Type, Func<object, object>> convertersForSourceType;
            if(ConvertFunctions.TryGetValue(src.GetType(), out convertersForSourceType))
            {
                Func<object, object> convertFunc;
                if(convertersForSourceType.TryGetValue(typeof(TRes), out convertFunc))
                {
                    return (TRes)convertFunc(src);
                }
            }

            throw new Exception(string.Format("Cannot convert from type '{0}' to type '{1}'.", src.GetType().Name, typeof(TRes).Name));
        }

        public static object Convert(Type tRes, object src)
        {
            if (tRes == src.GetType())
            {
                return src;
            }

            Dictionary<Type, Func<object, object>> convertersForSourceType;
            if (ConvertFunctions.TryGetValue(src.GetType(), out convertersForSourceType))
            {
                Func<object, object> convertFunc;
                if (convertersForSourceType.TryGetValue(tRes, out convertFunc))
                {
                    return convertFunc(src);
                }
            }

            throw new Exception(string.Format("Cannot convert from type '{0}' to type '{1}'.", src.GetType().Name, tRes.Name));
        }
    }
}
