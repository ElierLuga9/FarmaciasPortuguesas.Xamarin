using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ANFAPP.Logic.Models
{
    public class ServiceError
    {
		class ParseIntConverter : JsonConverter
		{
			public override bool CanConvert(Type objectType)
			{
				if (objectType == typeof(int?))
					return true;
				if (objectType == typeof(int))
					return true;
				if (objectType == typeof(string))
					return true;

				return false;
			}

			public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
			{
				object retVal = null;

				JToken token = JToken.Load (reader);

				if (token.Type == JTokenType.String) {
					int result;
					if (int.TryParse (token.ToObject<string> (), out result)) {
						retVal = result;
					}
				} else {
					retVal = serializer.Deserialize(reader, objectType);
				}

				return retVal;
			}

			public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
			{
				throw new NotImplementedException();
			}
		}

        public ANFError Error { get; set; }

        public class ANFError
        {
			[JsonConverter(typeof(ParseIntConverter))]
            public int? Code { get; set; }
            
			public string Message { get; set; }

			public ANFInnerError InnerError { get; set; }

			[JsonIgnore]
			public string Reason {
				get { 
					if (null != InnerError && !string.IsNullOrWhiteSpace (InnerError.Message))
						return InnerError.Message;

					return Message;
				}
			}
        }

		public class ANFInnerError 
		{
			public string Message { get; set; }
			public string Type { get; set; }
			public string Stacktrace { get; set; }
		}
    }
}
