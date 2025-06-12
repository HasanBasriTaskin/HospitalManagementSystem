using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entity.DTOs
{
    public class Response<T>
        where T : class
    {
        public Response()
        {
            Errors = new List<string>();
        }
        public T Data { get; set; }
        public List<string> Errors { get; set; }
        public int StatusCode { get; set; }
        public bool IsSuccessful
        {
            get { return (!Errors.Any()); }
        }
        public static Response<T> Success(T data, int statusCode)
        {
            return new Response<T> { Data = data, StatusCode = statusCode };
        }

        public static Response<T> Fail(int statusCode, List<string> errors)
        {
            return new Response<T> { StatusCode = statusCode, Errors = errors };
        }
    }
}
