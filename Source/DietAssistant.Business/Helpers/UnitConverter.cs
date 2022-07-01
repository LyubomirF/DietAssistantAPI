namespace DietAssistant.Business.Helpers
{
    public class UnitConverter
    {
        public static Double ToPounds(Double kgs)
            => Math.Round(2.205 * kgs, 2);

        public static Double ToKgs(Double pounds)
            => Math.Round(pounds / 2.205, 2);

        public static Double ToInches(Double centimeters)
            => Math.Round(centimeters / 2.54, 2);

        public static Double ToCentimeters(Double inches)
            => Math.Round(2.54 * inches, 2);
    }
}
