using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SQLite;

namespace ANFAPP.Logic.Database.Models
{
	[Table("Medicines")]
	public class Medicine : DosageBase
	{

        [PrimaryKey]
		public int Id { get; set; }
        public string CNP { get; set; }
		public string Name { get; set; }
        public string Dosage { get; set; }
        public string Qty { get; set; }
		public string Notes { get; set; }
        
        public bool WarningFlag { get; set; }
		public bool AutoGeneratedSchedule { get; set; }
		public string PharmaceuticalUnit { get; set; }

        [Ignore]
        public int ScheduledCount { get; set; }

		[Ignore]
		public bool HasScheduleSentByPharmacy { get; set; }

        [Ignore]
        public string Description { 
            get {
				int qty = 0;
				if (!string.IsNullOrWhiteSpace (Qty)) {
					int.TryParse (Qty, out qty);
				}

				StringBuilder sb = new StringBuilder ();
				if (!string.IsNullOrEmpty (Dosage)) {
					sb.AppendFormat ("{0} ", Dosage);
				}
				if (qty > 0) {
					if (sb.Length > 0) {
						sb.AppendFormat ("- ");
					}
					sb.AppendFormat ("{0} ", Qty);
				}
				if (!string.IsNullOrEmpty (PharmaceuticalUnit)) {
					sb.AppendFormat ("{0}", PharmaceuticalUnit);
				}

				return sb.ToString ();
            }    
        }

		[Ignore]
		public string PharmaceuticalUnitDescription
		{
			get { return !string.IsNullOrEmpty(PharmaceuticalUnit) ? PharmaceuticalUnit : "NA"; }
		}

        [Ignore]
        public string QuantityDescription { 
            get {
				if (string.IsNullOrEmpty(Qty) && string.IsNullOrEmpty(PharmaceuticalUnit)) return "NA";

				int qty = 0;
				if (!string.IsNullOrWhiteSpace (Qty)) {
					int.TryParse (Qty, out qty);
				}

				if (qty == 0)
					return PharmaceuticalUnit;

                return string.Format ("{0} {1}", Qty, PharmaceuticalUnit);
            }    
        }


		[Ignore]
		public string DosageDescription
		{
			get { return !string.IsNullOrEmpty(Dosage) ? Dosage : "NA"; }
		}

        [Ignore]
		public string FullDescription
		{
			get
			{
				int qty = 0;
				if (!string.IsNullOrWhiteSpace (Qty)) {
					int.TryParse (Qty, out qty);
				}

				StringBuilder sb = new StringBuilder ();
				if (!string.IsNullOrEmpty (Name)) {
					sb.AppendFormat ("{0}", Name);
				}
				if (!string.IsNullOrEmpty (Dosage)) {
					if (sb.Length > 0) sb.Append(" - ");
					sb.AppendFormat ("{0}", Dosage);
				}
				if (qty > 0) {
					if (sb.Length > 0) sb.AppendFormat (" - ");
					sb.AppendFormat ("{0} ", Qty);
				}
				if (!string.IsNullOrEmpty (PharmaceuticalUnit)) {
					sb.AppendFormat ("{0}", PharmaceuticalUnit);
				}

				return sb.ToString ();
			}
		}
	}
}
