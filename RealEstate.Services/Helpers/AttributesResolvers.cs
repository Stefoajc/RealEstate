using System.Collections.Generic;
using System.Linq;
using RealEstate.Model;

namespace RealEstate.Services.Helpers
{
    public static class AttributesResolvers
    {
        public const string BedroomsCountAttribute = "BedroomsCount";
        public const string BathroomsCountAttribute = "BathroomsCount";
        public const string RoomsCountAttribute = "RoomsCount";
        public const string AreaInSquareMetersAttribute = "Area";

        private static readonly Dictionary<string[], string> HelperPropertiesAttributes = new Dictionary<string[], string>
        {
            {new[] {"БРОЙ СПАЛНИ", "СПАЛНИ", "СПАЛНИ ПОМЕЩЕНИЯ", "СТАИ ЗА СПАНЕ"}, BedroomsCountAttribute},
            {new[] {"БРОЙ БАНИ", "БАНИ"}, BathroomsCountAttribute},
            {new[] {"БРОЙ СТАИ", "СТАИ", "ПОМЕЩЕНИЯ"}, RoomsCountAttribute}
        };


        private static readonly Dictionary<string,string> HelperPropertyAttributesReverse = new Dictionary<string, string>
        {
            {BedroomsCountAttribute, "Спални"},
            {BathroomsCountAttribute, "Бани"},
            {RoomsCountAttribute, "Стаи"},
            {AreaInSquareMetersAttribute, "Площ"},
        };

        public static string AttributeKeySaveResolver(string key)
        {
            foreach (var helperPropertiesAttribute in HelperPropertiesAttributes)
            {
                if (helperPropertiesAttribute.Key.Any(k => k == key.ToUpper()))
                {
                    return helperPropertiesAttribute.Value;
                }
            }

            return key;
        }

        public static string AttributeKeyReverseResolver(string key)
        {
            if (HelperPropertyAttributesReverse.ContainsKey(key))
            {
                return HelperPropertyAttributesReverse[key];
            }

            return key;
        }

        public static KeyValuePairs AttributesResolver(string key, string value)
        {
            
            foreach (var helperPropertiesAttribute in HelperPropertiesAttributes)
            {
                if (helperPropertiesAttribute.Key.Any(k => k == key.ToUpper()) && StringToIntNumber.Resolve(value) != -1)
                {
                    return new KeyValuePairs
                    {
                        Key = helperPropertiesAttribute.Value,
                        Value = StringToIntNumber.Resolve(value).ToString()
                    };
                }
            }


            var areaAttribute = new[] { "ПЛОЩ", "КВАДРАТУРА", "РАЗМЕР", "РАЗМЕРИ", "ГОЛЕМИНА", "КВАДРАТИ" };
            if (areaAttribute.Any(a => a == key.ToUpper()))
            {
                return new KeyValuePairs
                {
                    Key = AreaInSquareMetersAttribute,
                    Value = value
                };
            }


            return new KeyValuePairs
            {
                Key = key,
                Value = value
            };

        }
    }
}