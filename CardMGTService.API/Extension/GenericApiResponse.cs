using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CardMGTService.API.Extension
{
    public class GenericApiResponse<T>
    {

        public string message { get; set; }
        public string response { get; set; }
        public string responsedata { get; set; }
        public T data { get; set; }
    }
}