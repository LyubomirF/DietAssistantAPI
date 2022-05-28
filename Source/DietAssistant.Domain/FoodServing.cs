#pragma warning disable CS8618

namespace DietAssistant.Domain
{
    public class FoodServing
    {
        public Int32 FoodServingId { get; set; }

        public Food Food { get; set; }

        public Int32 ServingSizeId { get; set; }

        public ServingSize ServingSize { get; set; }

        public Double NumberOfServings { get; set; }
    }
}
