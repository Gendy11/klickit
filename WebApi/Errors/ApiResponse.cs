﻿namespace WebApi.Errors
{
    public class ApiResponse 
    {
        public ApiResponse(int statusCode,string message=null)
        {
            StatusCode = statusCode;
            Message = message ?? GetDefaultMessageForStatusCode(statusCode); 
        }
        public int StatusCode { get; set; }
        public string Message { get; set; }
        private string GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "You have made a bad request",
                401 => "You are not Authorized",
                403 => "You are forbidden from doing this",
                404 => "Resource not found",
                500 => "Server error",
                _ => null
            };
        }
    }

}