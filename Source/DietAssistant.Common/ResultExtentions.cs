namespace DietAssistant.Common
{
    public static class Result
    {
        public static Result<TModel> Create<TModel>(TModel data)
            => new Result<TModel>(data, EvaluationTypes.Success, null);

        public static Result<TModel> CreateWithError<TModel>(EvaluationTypes evalution, string message)
            => new Result<TModel>(default, evalution, new List<string> { message });

        public static Result<TModel> CreateWithErrors<TModel>(EvaluationTypes evalution, List<string> messages)
            => new Result<TModel>(default, evalution, messages);
       
    }
}
