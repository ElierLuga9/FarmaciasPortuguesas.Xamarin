
using Newtonsoft.Json;
namespace ANFAPP.Logic.Models.Out
{
    public class WSTomasOut
    {

		#region Properties

		public int StatusCode { get; set; }
        public string ErrorMessage { get; set; }

		#endregion

		// Backwards compatible
		[JsonIgnore]
		public bool OK
		{
			get { return StatusCode == 200; }
		}

    }
}
