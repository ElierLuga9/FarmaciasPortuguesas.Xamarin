using Newtonsoft.Json;

namespace ANFAPP.Logic.Models.In.Ecommerce
{
	public class EditBasketProductIn
	{
		[JsonProperty("NUMLINHA")]
		public int Id { get; set; }

		[JsonProperty("FARMID")]
		public string PharmacyId { get; set; }

		[JsonProperty("TIPOAQUISICAO", NullValueHandling=NullValueHandling.Ignore)]
		public int? AquisitionType { get; set; }

		[JsonProperty("QTY", NullValueHandling=NullValueHandling.Ignore)]
		public int? Quantity { get; set; }
	}
}
