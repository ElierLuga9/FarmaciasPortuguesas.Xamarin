using System;
using Newtonsoft.Json;

namespace ANFAPP.Logic.StaticData.Models
{
	public class CatalogCategory
	{
		[JsonProperty("cid")]
		public string Id { get; set; }
		[JsonProperty("name")]
		public string Name { get; set; }
		[JsonProperty("fid")]
		public UInt64 FamilyId { get; set; }
	}
}
