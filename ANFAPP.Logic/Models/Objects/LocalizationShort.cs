using System;
using System.Collections;

namespace ANFAPP.Logic
{
	public class LocalizationShort
	{
		public string Id { get; set; }
		public string Description { get; set; }
		public string DescriptionLong { get; set; }
		public string ImageName { get; set; }

		public LocalizationShort () {}

		public LocalizationShort (string id, string description, string descriptionLong) {
			this.Id = id;
			this.Description = description;
			this.DescriptionLong = description;
			//dummy object
			this.ImageName = "annotation_list";
		}

		public char ShortByDescrition
		{
			get
			{
				if (string.IsNullOrWhiteSpace(Description) || Description.Length == 0)
					return '?';

				return Description[0];
			}
		}
	}
}

