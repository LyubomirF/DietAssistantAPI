namespace DietAssistant.Business.Contracts.Models.FoodCatalog.Responses
{
    public class FoodSearch
    {
        public List<Food> Foods { get; set; }

        public Int32 Page { get; set; }

        public Int32 PageSize { get; set; }

        public Int32 TotalFoods { get; set; }
    }
}
