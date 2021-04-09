using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ANFAPP.Logic.Models.Out
{
    public class PointsMovementOut
    {

        /// <summary>
        /// List of values.
        /// </summary>
        public List<PointsMovement> Value { get; set; }
    }

    /// <summary>
    /// Inner Class
    /// </summary>
    public class PointsMovement
    {

        public int Id { get; set; }
        public int PointsBalance { get; set; }
        public int Points { get; set; }
        public double Value { get; set; }
        public string Type { get; set; }
        public DateTime Date { get; set; }

		public Card Card { get; set; }

        [JsonIgnore]
        public int Order { get; set; }
        [JsonIgnore]
        public string MovementType { get; set; }
        [JsonIgnore]
        public string FormatedPoints
        {
            get
            {
                return Points >= 0 ? "+" + Points : Points + string.Empty;
            }
        }
    }

	public class Card
	{
		public string Number { get; set; }
		public string Name { get; set; }
	}
}
