using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Services.Helpers
{
    public static class StringToIntNumber
    {
        public static int Resolve(string ordinalNumber)
        {
            Dictionary<string[], int> helperPropertiesAttributes = new Dictionary<string[], int>
            {
                { new []{ "1","ЕДНО", "ЕДИН", "ЕДНА", "ПЪРВИ", "FIRST","ONE", "1ST", "1ВИ"}, 1 },
                { new []{ "2", "ДВЕ", "ВТОРИ", "2РИ", "TWO", "SECOND", "2ND" }, 2},
                { new []{ "3", "ТРИ", "ТРЕТИ", "3ТИ", "THREE", "THIRD", "3RD"},3 },
                { new []{ "4", "ЧЕТИРИ", "ЧЕТВЪРТИ", "4ТИ", "FOUR", "FOURTH", "4TH" }, 4 },
                { new []{ "5", "ПЕТ", "ПЕТИ", "5ТИ", "FIVE", "FIFTH", "5TH" }, 5 },
                { new []{ "6", "ШЕСТ", "ШЕСТИ", "6ТИ", "SIX", "SIXTH", "6TH" }, 6 },
                { new []{ "7", "СЕДЕМ", "СЕДМИ", "7МИ", "SEVEN", "SEVENTH", "7TH" }, 7 },
                { new []{ "8", "ОСЕМ", "ОСМИ", "8МИ", "EIGHT", "8TH" }, 8 },
                { new []{ "9", "ДЕВЕТ", "ДЕВЕТИ", "9ТИ", "NINE", "NINTH", "9TH" }, 9 },
                { new []{ "10", "ДЕСЕТ", "ДЕСЕТИ", "10ТИ", "TEN", "TENTH", "10TH" }, 10 },
            };

            foreach (var attribute in helperPropertiesAttributes)
            {
                if (attribute.Key.Any(k => k == ordinalNumber))
                {
                    return attribute.Value;
                }
            }

            return -1;
        }
    }
}
