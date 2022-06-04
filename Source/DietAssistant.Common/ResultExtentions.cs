namespace DietAssistant.Common
{
    public static class Result
    {
        public static Result<TModel> Create<TModel>(
            TModel data)
        {
            return new Result<TModel>(data, EvaluationTypes.Success, new List<string> { });
        }

        public static Result<TModel> Create<TModel>(
            EvaluationTypes evalution,
            string message) 
        {
            return new Result<TModel>(default, evalution, new List<string> { message });
        }

        public static Result<TModel> Create<TModel>(
            EvaluationTypes evalution,
            List<string> messages)
        {
            return new Result<TModel>(default, evalution, messages);
        }
    }
}
