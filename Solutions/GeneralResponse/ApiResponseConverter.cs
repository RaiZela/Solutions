using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Solutions.GeneralResponse;

public static class ApiResponseConverter<S, R>
{
    public static ApiResponse<R> ConvertServiceResult(ApiResponse<S> obj, string sourceType = null)
    {
        if (obj.StatusCode != 200)
        {
            R r = default;
            return new ApiResponse<R>(obj.StatusCode, obj.Success, r, obj.Message);
        }

        else if (obj.StatusCode == 200 && obj.Result == null)
            return ApiResponse<R>.ApiNotFoundResponse($"{sourceType} not found!");

        else
        {
            R r = Activator.CreateInstance<R>();
            return new ApiResponse<R>(obj.StatusCode, obj.Success, r, obj.Message);
        }
    }
}