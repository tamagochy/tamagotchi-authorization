﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace Tamagotchi.Authorization.Helpers
{
    public class ApiResult<T> where T : class
    {
        public ApiResult()
        { }

        public ApiResult(T data)
        {
            Data = data;
        }

        public ApiResult(Error error)
        {
            Errors = new List<Error> { error };
        }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public T Data { get; set; }
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public List<Error> Errors { get; set; }
    }

    public class Error
    {
        public string Attr { get; set; }
        public string Code { get; set; }
    }

    public class Error<T> : Error where T : class
    {
        public T AdditionalData { get; set; }
    }
}
