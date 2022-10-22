namespace DietAssistant.Business.Helpers
{
    internal static class UnitConvert
    {
        public static Double ToKgs(Double weightInPounds) => Math.Round(weightInPounds / 2.205, 2);
        public static Double ToPounds(Double weightInKg) => Math.Round(2.205 * weightInKg, 2);
        public static Double ToCentimeters(Double heightInInches) => Math.Round(2.54 * heightInInches, 2);
    }
}
