using System;
using System.Globalization;
using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.Out.Ecommerce
{
	// Workaround to mandatory fields that are not yet sent by the web service.
	public class NullToTypeConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return true;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			object retVal;
			try {
				retVal = serializer.Deserialize(reader, objectType);
			}  catch (Exception) {
				retVal = Activator.CreateInstance(objectType);
			}
			return retVal;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}
	}	
}
