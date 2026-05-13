

namespace MS_Application.Helpers;

using MS_Application.DataTransferObjects.Base;
using MS_Application.Constants;
using static MS_Application.Constants.GlobalConstants;

public static class BaseTableResponseHelper
{
    public static BaseTableResponse<T> Set<T>(
        this BaseTableResponse<T> response,
        string message,
        List<T> data = default,
        string type = ResponseType.Success,
        string code = ResponseStatusCode.Status200)
    {
        response ??= new BaseTableResponse<T>();

        response.Type = type.ToString();
        response.Message = message;
        response.Code = code;
        response.Data = data;

        return response;
    }

    public static BaseTableResponse<T> Set<T>(
        this BaseTableResponse<T> response,
        string message,
        string code = ResponseStatusCode.Status200,
        string type = ResponseType.Success)
    {
        response ??= new BaseTableResponse<T>();

        response.Type = type.ToString();
        response.Message = message;
        response.Code = code;

        return response;
    }

    public static BaseTableResponse<T> Success<T>(this BaseTableResponse<T> response, List<T> data, string message, string code = ResponseStatusCode.Status200) => response.Set(message, data, code, ResponseType.Success);

    public static BaseTableResponse<T> Success<T>(this BaseTableResponse<T> response, string message, string code = ResponseStatusCode.Status200) => response.Set(message, code, ResponseType.Success);

    public static BaseTableResponse<T> Fail<T>(this BaseTableResponse<T> response, List<T> data, string message, string code = ResponseStatusCode.Status400) => response.Set(message, data, code, ResponseType.Error);

    public static BaseTableResponse<T> Fail<T>(this BaseTableResponse<T> response, string message, string code = ResponseStatusCode.Status400) => response.Set(message, code, ResponseType.Error);
}
