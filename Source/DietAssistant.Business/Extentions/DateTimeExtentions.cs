namespace DietAssistant.Business.Extentions
{
    internal static class DateTimeExtentions
    {
        public static Int32 ToAge(this DateTime dateOfBirth, DateTime date)
        {
            int age;
            age = date.Year - dateOfBirth.Year;

            if (age > 0)
                age -= Convert.ToInt32(date.Date < dateOfBirth.Date.AddYears(age));
            else
                age = 0;

            return age;
        }
    }
}
