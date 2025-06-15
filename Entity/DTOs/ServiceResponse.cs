using System.Collections.Generic;

namespace Entity.DTOs
{
    public class ServiceResponse<T>
    {
        public ServiceResponse()
        {
            Errors = new List<string>();
        }
        
        public bool IsSuccess { get; set; }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        
        // Success - for single data
        public static ServiceResponse<T> Success(T data)
        {
            return new ServiceResponse<T> 
            { 
                IsSuccess = true, 
                Data = data 
            };
        }
        
        // Success - for void methods (no data)
        public static ServiceResponse<T> SuccessNoData()
        {
            return new ServiceResponse<T> 
            { 
                IsSuccess = true
            };
        }
        
        // Failure - single error message
        public static ServiceResponse<T> Failure(string error)
        {
            return new ServiceResponse<T> 
            { 
                IsSuccess = false, 
                Errors = new List<string> { error } 
            };
        }
        
        // Failure - multiple error messages
        public static ServiceResponse<T> Failure(List<string> errors)
        {
            return new ServiceResponse<T> 
            { 
                IsSuccess = false, 
                Errors = errors 
            };
        }
    }
} 