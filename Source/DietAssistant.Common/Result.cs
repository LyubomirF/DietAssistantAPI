namespace DietAssistant.Common
{
    public class Result<TModel> 
    {
        public Result(TModel data, EvaluationTypes evaluation, IEnumerable<string> errors)
        {
            Data = data;
            EvaluationResult = evaluation;
            Errors = errors;
        }

        public TModel Data { get; private set; }

        public EvaluationTypes EvaluationResult { get; private set; }

        public IEnumerable<string> Errors { get; private set; }

        public bool IsSuccessful() 
            => EvaluationResult == EvaluationTypes.Success;

        public bool IsFailure()
            => !IsSuccessful();
    }
}
