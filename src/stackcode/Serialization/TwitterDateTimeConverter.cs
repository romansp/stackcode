using System.Globalization;
using Newtonsoft.Json.Converters;

namespace stackcode.Serialization
{
    public class TwitterDateTimeConverter : IsoDateTimeConverter
    {
        public TwitterDateTimeConverter()
        {
            DateTimeFormat = "ddd MMM dd HH:mm:ss K yyyy";
            DateTimeStyles = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;
            Culture = CultureInfo.InvariantCulture;
        }
    }
}