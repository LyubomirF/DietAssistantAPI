﻿using DietAssistant.Common;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace DietAssistant.WebAPI.Extentions
{
    public static class ResultExtentions
    {
        public static IActionResult ToActionResult<T>(this Result<T> result, ControllerBase controller)
            => result.EvaluationResult switch
            {
                EvaluationTypes.Success => controller.Ok(result),
                EvaluationTypes.InvalidParameters => controller.BadRequest(result),
                EvaluationTypes.NotFound => controller.NotFound(result),
                EvaluationTypes.Failed => controller.StatusCode((int)HttpStatusCode.InternalServerError, result),
                _ => throw new NotSupportedException("Action is not supported")
            };


        public static async Task<IActionResult> ToActionResult<T>(this Task<Result<T>> result, ControllerBase controller)
        {
            return ToActionResult<T>(await result, controller);
        }
    }
}
