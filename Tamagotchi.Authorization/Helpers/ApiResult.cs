using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Tamagotchi.Authorization.JsonModels;

namespace Tamagotchi.Authorization.Helpers
{
    public class ApiResult<T> where T : class
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; }
        public ApiResult()
        {
        }
        public ApiResult(T data)
        {
            Data = data; 
        }
        public ApiResult(List<Error> error)
        {
            Errors = error;
        }

    }

    public class Error
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Attr { get; set; }
        public string Code { get; set; }
    }

    public class Error<T> : Error where T : class
    {
        public T AdditionalData { get; set; }
    }
}
