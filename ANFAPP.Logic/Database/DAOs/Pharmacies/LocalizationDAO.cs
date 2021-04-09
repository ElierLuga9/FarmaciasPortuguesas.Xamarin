using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Logic.Database.DAOs
{
	public class LocalizationDAO : DAO<Localization>
	{
		public Task<List<LocalizationShort>> GetAllDistritosDistinct()
		{
			return Task.Run<List<LocalizationShort>>(() =>
				{
					var db = GetDatabaseInstance();

					List<LocalizationShort> shortList = new List<LocalizationShort>();
					List<Localization> locations = db.Table<Localization>().ToList();

					var locsDisctinct = locations
						.Select(i => new {i.IdDistrito, i.DescDistrito})
						.Distinct()
						.OrderBy(i => i.DescDistrito)
						.ToArray();

					foreach (var temp in locsDisctinct) {
						shortList.Add(new LocalizationShort {
							Id = temp.IdDistrito,
							Description = temp.DescDistrito,
							ImageName = "annotation_list"
						});
					}
							
					return shortList;
				});
		}


		public Task<List<LocalizationShort>> GetConcelhosById(string id)
		{
			return Task.Run<List<LocalizationShort>>(() =>
				{
					var db = GetDatabaseInstance();

					List<LocalizationShort> shortList = new List<LocalizationShort>();
					List<Localization> locations = db.Table<Localization>()
						.Where (p => p.IdDistrito == id)
						.ToList();

					var locsDisctinct = locations
						.Select(i => new {i.IdConcelho, i.DescConcelho})
						.Distinct()
						.OrderBy(i => i.DescConcelho)
						.ToArray();

					foreach (var temp in locsDisctinct) {
						shortList.Add(new LocalizationShort {
							Id = temp.IdConcelho,
							Description = temp.DescConcelho,
							ImageName = "annotation_list"
						});
					}

					return shortList;
				});
		}


		public Task<List<LocalizationShort>> GetFreguesiasById(string idDistrict, string idCounty)
		{
			return Task.Run<List<LocalizationShort>>(() =>
				{
					var db = GetDatabaseInstance();

					List<LocalizationShort> shortList = new List<LocalizationShort>();
					List<Localization> locations = db.Table<Localization>()
						.Where(p => (p.IdDistrito == idDistrict) && (p.IdConcelho == idCounty))
						.ToList();

					var locsDisctinct = locations
						.Select(i => new {i.IdFreguesia, i.DescAbreviadoFreguesia, i.DescCompletaFreguesia})
						.Distinct()
						.OrderBy(i => i.DescCompletaFreguesia)
						.ToArray();

					//warning: descriptions are switched
					foreach (var temp in locsDisctinct) {
						shortList.Add(new LocalizationShort {
							Id = temp.IdFreguesia,
							Description = temp.DescCompletaFreguesia,
							DescriptionLong = temp.DescAbreviadoFreguesia,
							ImageName = "annotation_list"
						});
					}

					return shortList;
				});
		}


		public Task<List<Pharmacy>> GetFavourits()
		{
			return Task.Run<List<Pharmacy>>(() =>
				{
					var db = GetDatabaseInstance();

					List<Pharmacy> pharmacies = db.Table<Pharmacy>()
						.Where(p => p.IsFavourite == true)
						.OrderBy(p => p.Name)
						.ToList();

                    foreach (Pharmacy pharm in pharmacies)
                    {
                        ANFAPP.Logic.Helper.PharmacyHelper.SetPharmStatusTxt(pharm);
                    }

                    return pharmacies;
				});
		}

		public Task<List<Pharmacy>> GetRecent()
		{
			return Task.Run<List<Pharmacy>>(() =>
				{
					var db = GetDatabaseInstance();

					DateTime dt = new DateTime(2000,01,01);

					List<Pharmacy> pharmacies = db.Table<Pharmacy>()
						.Where(p => p.LastVisitedOn != dt)
						.OrderBy(p => p.LastVisitedOn)
						.ToList();

					var orderP = (pharmacies
						.OrderByDescending(i => i.LastVisitedOn)
						.ToList())
						.Take(5).ToList();

                    foreach(Pharmacy pharm in orderP)
                    {
                        ANFAPP.Logic.Helper.PharmacyHelper.SetPharmStatusTxt(pharm);
                    }

                    return orderP;
				});
		}

		public Task<string> GetDistrictContaining(string search) {
			return Task.Run<string>(() =>
				{
					var db = GetDatabaseInstance();

					List<Localization> localizations =  db.Table<Localization>()
						.Where(p => p.DescDistrito.Contains(search))
						.ToList();

					if (localizations.Count > 0) {
						var distLoc = localizations
							.Select(i => new {i.IdDistrito})
							.Distinct()
							.ToList();

						//District eq 02 or District eq 03...
						string locs = "(District eq ";
						for (int i=0; i<distLoc.Count; i++) {
							locs = locs + distLoc[i].IdDistrito;
							if (i != distLoc.Count-1) {
								locs = locs + " or District eq ";
							} else {
								locs = locs + ")";
							}
						}
						return locs;
					} else 
						return null;

				});
		}

		public Task<string> GetCountyContaining(string search) {
			return Task.Run<string>(() =>
				{
					var db = GetDatabaseInstance();

					List<Localization> localizations =  db.Table<Localization>()
						.Where(p => p.DescConcelho.Contains(search))
						.ToList();

					if (localizations.Count > 0) {
						var distLoc = localizations
							.Select(i => new {i.IdConcelho})
							.Distinct()
							.ToList();

						//District eq 02 or District eq 03...
						string locs = "(County eq ";
						for (int i=0; i<distLoc.Count; i++) {
							locs = locs + distLoc[i].IdConcelho;
							if (i != distLoc.Count-1) {
								locs = locs + " or County eq ";
							} else {
								locs = locs + ")";
							}
						}
						return locs;
					} else 
						return null;
					
				});
		}

		public Task<string> GetParishContaining(string search) {
			return Task.Run<string>(() =>
				{
					var db = GetDatabaseInstance();

					List<Localization> localizations =  db.Table<Localization>()
						.Where(p => p.DescCompletaFreguesia.Contains(search))
						.ToList();

					if (localizations.Count > 0) {
						var distLoc = localizations
							.Select(i => new {i.IdFreguesia})
							.Distinct()
							.ToList();

						//District eq 02 or District eq 03...
						string locs = "(Parish eq '";
						for (int i=0; i<distLoc.Count; i++) {
							locs = locs + distLoc[i].IdFreguesia;
							if (i != distLoc.Count-1) {
								locs = locs + "' or Parish eq '";
							} else {
								locs = locs + "')";
							}
						}
						return locs;
					} else 
						return null;
				});
		}

		public Task<string> GetLocalsWithString(string search) {
			return Task.Run<string>(() =>
				{
					// Yet another reason why the LazyLoad needs to be replaced.
					var upper = search.ToUpper();
					// Escape "'" for ODATA (see http://stackoverflow.com/a/4483742).
					var escaped = upper.Replace("'", "''");

					// This should be implemented on the server side: http://issue.innovagency.com/view.php?id=20964
					// The PCL doesn't have Normalize. Awesome!
					// escaped = escaped.Normalize(NormalizationForm.FormD);
					// And because the server doesn't return results with accents when we send the normalized expression... we need to send both!! Super Awesome!!!
					string unescapedString = null;
					if (escaped.Contains("À") || escaped.Contains("Á") || escaped.Contains("Ã") || escaped.Contains("Â") || escaped.Contains("É") 
						|| escaped.Contains("Ê") || escaped.Contains("Í") || escaped.Contains("Ó") || escaped.Contains("Ô") || escaped.Contains("Ú")) {
						unescapedString = escaped;

						escaped = escaped.Replace("À", "A");
						escaped = escaped.Replace("Á", "A");
						escaped = escaped.Replace("Ã", "A");
						escaped = escaped.Replace("Â", "A");
						escaped = escaped.Replace("É", "E");
						escaped = escaped.Replace("Ê", "E");
						escaped = escaped.Replace("Í", "I");
						escaped = escaped.Replace("Ó", "O");
						escaped = escaped.Replace("Ô", "O");
						escaped = escaped.Replace("Ú", "U");
					}


					var db = GetDatabaseInstance();

					List<Localization> localizations =  db.Table<Localization>()
						//.Where(p => (p.DescDistrito + " " + p.DescConcelho + " " + p.DescCompletaFreguesia).ToString().Contains(search))
						.ToList();

					List<string> validLocs = new List<string>();
					string query;
					if (localizations.Count > 0) {

						for (int i=0; i<localizations.Count; i++) {
							Localization locTemp = localizations[i];

							/*
							if (locTemp.DescCompletaFreguesia.Contains(search)) {
								query = "(District eq {0} and County eq {1} and Parish eq '{2}')";
								query = query.Replace("{0}", locTemp.IdDistrito);
								query = query.Replace("{1}", locTemp.IdConcelho);
								query = query.Replace("{2}", locTemp.IdFreguesia);
								validLocs.Add(query);
							} else */
							
							/// County search will be done using the pharmacy "Locale"
							//if (locTemp.DescConcelho.Contains(search)) {
							//	query = "(District eq {0} and County eq {1})";
							//	query = query.Replace("{0}", locTemp.IdDistrito);
							//	query = query.Replace("{1}", locTemp.IdConcelho);
							//	validLocs.Add(query);
							//} else 
							if (locTemp.DescDistrito.Contains(upper)) {
								query = "(District eq {0})";
								query = query.Replace("{0}", locTemp.IdDistrito);
								validLocs.Add(query);
							}

						}

						var distinctLocs = validLocs.Distinct().ToList();
						string myQuery = "";
						for (int j=0; j<distinctLocs.Count; j++) {
							myQuery = myQuery + distinctLocs[j];

							if (j != distinctLocs.Count -1) {
								myQuery = myQuery + " or ";
							}
						}

						// Add name query
						string searchByName = "contains(Name,'" + escaped + "')";
						if (!string.IsNullOrWhiteSpace(myQuery)) {
							searchByName = " or " + searchByName;
						}

						if (!string.IsNullOrEmpty(unescapedString)) {
							// XXX: Work around for the search service lack of search
							searchByName += " or contains(Name,'" + unescapedString + "')";
						}

						myQuery = myQuery + searchByName;

						// Add locale query
						string searchByLocale = "contains(Locale,'" + escaped + "')";
						if (!string.IsNullOrWhiteSpace(myQuery))
						{
							searchByLocale = " or " + searchByLocale;
						}
						myQuery = myQuery + searchByLocale;

						return myQuery;

					} else 
						return null;
				});
		}

		[Obsolete]
		public Task<string> GetQueryForCounty(string search) {
			return Task.Run<string>(() =>
				{
					var db = GetDatabaseInstance();

					List<Localization> localizations =  db.Table<Localization>()
						//.Where(p => (p.DescDistrito + " " + p.DescConcelho + " " + p.DescCompletaFreguesia).ToString().Contains(search))
						.ToList();

					List<string> validLocs = new List<string>();
					string query;
					if (localizations.Count > 0) {

						for (int i=0; i<localizations.Count; i++) {
							Localization locTemp = localizations[i];

							if (locTemp.DescConcelho.Contains(search)) {
								query = "(District eq {0} and County eq {1})";
								query = query.Replace("{0}", locTemp.IdDistrito);
								query = query.Replace("{1}", locTemp.IdConcelho);
								validLocs.Add(query);
							}

						}

						var distinctLocs = validLocs.Distinct().ToList();
						string myQuery = "";
						for (int j=0; j<distinctLocs.Count; j++) {
							myQuery = myQuery + distinctLocs[j];

							if (j != distinctLocs.Count -1) {
								myQuery = myQuery + " or ";
							}
						}

						return myQuery;

					} else 
						return null;
				});
		}

		public Task<Tuple<string, string, string>> GetIdsForDescriptions(string districtDescription, string countyDescription, string parishDescription)
		{
			return Task.Run<Tuple<string, string, string>>(() =>
				{
					Tuple<string,string,string> t = null;

					var db = GetDatabaseInstance();

					var d1 = districtDescription.ToUpperInvariant();
					var d2 = countyDescription.ToUpperInvariant();
					var d3 = parishDescription.ToUpperInvariant();

					List<Localization> localizations = db.Table<Localization>()
						.Where(p => 
							p.DescDistrito.Contains(d1) &&
							p.DescConcelho.Contains(d2) &&
							p.DescAbreviadoFreguesia.Contains(d3))
						.ToList();

					if (localizations.Count > 0)
					{
						Localization location = localizations.Last();
						t = new Tuple<string, string, string>(location.IdDistrito, location.IdConcelho, location.IdFreguesia);
					}

					return t;
				});
		}
	}
}

