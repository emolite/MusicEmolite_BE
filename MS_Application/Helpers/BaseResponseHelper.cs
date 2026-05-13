// <copyright file=BaseResponseHelper.cs company= AMF>
// Copyright (c) AMF. All rights reserved.
// </copyright>

namespace MS_Application.Helpers;

using MS_Application.Constants;
using MS_Application.DataTransferObjects.Base;
using static MS_Application.Constants.GlobalConstants;

public static class BaseResponseHelper
{
    public static BaseResponse<T> Set<T>(
        this BaseResponse<T> response,
        string message,
        T data = default,
        string code = ResponseStatusCode.Status200,
        string type = ResponseType.Success)
    {
        response ??= new BaseResponse<T>();

        response.Type = type.ToString();
        response.Message = message;
        response.Code = code;
        response.Data = data;

        return response;
    }

    public static BaseResponse<T> Set<T>(
        this BaseResponse<T> response,
        string message,
        string code = ResponseStatusCode.Status200,
        string type = ResponseType.Success)
    {
        response ??= new BaseResponse<T>();

        response.Type = type.ToString();
        response.Message = message;
        response.Code = code;

        return response;
    }

    public static BaseResponse<T> Success<T>(this BaseResponse<T> response, T data, string message, string code = ResponseStatusCode.Status200) => response.Set(message, data, code, ResponseType.Success);

    public static BaseResponse<T> Success<T>(this BaseResponse<T> response, string message, string code = ResponseStatusCode.Status200) => response.Set(message, code, ResponseType.Success);

    public static BaseResponse<T> Fail<T>(this BaseResponse<T> response, T data, string message, string code = ResponseStatusCode.Status400) => response.Set(message, data, code, ResponseType.Error);

    public static BaseResponse<T> Fail<T>(this BaseResponse<T> response, string message, string code = ResponseStatusCode.Status400) => response.Set(message, code, ResponseType.Error);
}
