using System;

namespace ANFAPP.Logic.Models.Wrappers
{
	public class PrescriptionItem
	{
		public PrescriptionItem() : base()
		{
			Quantity = 1;
		}

		public int? Code { get; set; }
	
		public int Quantity { get; set; }

		public string Type { get; set; }

		public int ActivePrincipleId { get; set; }

		public string Name { get; set; }

		public string Dosage { get; set; }

		public int? Pack { get; set; }

		public string PackName { get; set; }

		public string FF { get; set; }

		public string PackDescription
		{
			get { 
				if (null != PackName) {
					return Dosage + " - " + PackName; 
				} else {
					return Dosage + " - " + Pack + " " + FF; 
				}
			}
		}

		private string _CNPPrinciple;
		public string CNPPrinciple
		{
			get { return _CNPPrinciple; }
			set
			{
				_CNPPrinciple = (value != null && value.Length > 140) ? value.Substring(0, 140) : value;
			}
		}


		public string CodeLabel
		{
			get { return Type + ":" + Code; }
		}

		public string Presentation { get; set; }
	}
}

