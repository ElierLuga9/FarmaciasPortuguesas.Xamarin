using ANFAPP.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ANFAPP.Logic.Database.Models;

namespace ANFAPP.Resources
{
    public static class ConverterResources
    {
		public static DoubleToCardImageConvert DoubleToCardImageConvert = new DoubleToCardImageConvert();
        public static ZeroToFalseConverter ZeroToFalse = new ZeroToFalseConverter();
        public static NullableConverter Nullable = new NullableConverter();
        public static ZeroToNullConverter ZeroToNull = new ZeroToNullConverter();
        public static InverseBoolConverter InverseBool = new InverseBoolConverter();
        public static NotNullableConverter NotNullable = new NotNullableConverter();
        public static BoolToLanguageConverter BoolToLanguage = new BoolToLanguageConverter();
        public static StringToMiniDateConverter StringToMiniDate = new StringToMiniDateConverter();
        public static StringToShortDateConverter StringToShortDate = new StringToShortDateConverter();
        public static DocumentTypeColorConverter DocumentTypeColor = new DocumentTypeColorConverter();
        public static TableOrderBackgroundConverter TableOrderBackground = new TableOrderBackgroundConverter();
        public static MedicineListPlanTextColorConverter MedicineListTextColor = new MedicineListPlanTextColorConverter();

        public static CheckboxImageConverter CheckboxImage = new CheckboxImageConverter();
		public static CheckboxImageConverter CheckboxImageDoTomas = new CheckboxImageConverter(true);
        public static BiometricGaugeImageConverter BiometricGaugeImage = new BiometricGaugeImageConverter();

        public static MaleSelectedImageConverter MaleSelectedImage = new MaleSelectedImageConverter();
        public static FemaleSelectedImageConverter FemaleSelectedImage = new FemaleSelectedImageConverter();

		public static PharmacyToXAMLConverter PharmacyPinType = new PharmacyToXAMLConverter();
		public static PharmacyIsInServiceConverter PharmacyInService = new PharmacyIsInServiceConverter();

		public static UppercaseConverter Uppercase = new UppercaseConverter();

        public static CNPLabelConverter CNPEMLabel = new CNPLabelConverter();

        public static EmptyOrNullConverter EmptyOrNull = new EmptyOrNullConverter();
        public static HasElementsConverter HasElements = new HasElementsConverter();

		public static CatalogCategoryColorConverter CatalogCategoryColor = new CatalogCategoryColorConverter();

        public static ZeroToNoneConverter ZeroToNone = new ZeroToNoneConverter();
		public static TitlecaseConverter Capitalize = new TitlecaseConverter();

       	public static PointsToStringConverter PointsString = new PointsToStringConverter();

		public static UtcToLocalDateTimeConverter SystemToPresentationDate = new UtcToLocalDateTimeConverter();
    }
}
