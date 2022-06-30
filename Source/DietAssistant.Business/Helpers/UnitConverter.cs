namespace DietAssistant.Business.Helpers
{
    internal static class UnitConverter
    {
        public static Double ToPounds(this Double kgs)
            => Math.Round(2.205 * kgs, 2);

        public static Double ToKgs(this Double pounds)
            => Math.Round(pounds / 2.205, 2);

        public static Double ToInches(this Double centimeters)
            => Math.Round(centimeters / 2.54, 2);

        public static Double ToCentimeters(this Double inches)
            => Math.Round(2.54 * inches, 2);
    }
}
