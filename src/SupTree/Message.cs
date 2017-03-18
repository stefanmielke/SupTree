using System;
using Newtonsoft.Json;

namespace SupTree
{
    public class Message
    {
        /// <summary>
        /// SYSTEM, Object, XML, JSON, etc...
        /// </summary>
        public string Format { get; set; }

        /// <summary>
        /// Filtering for formats
        /// </summary>
        public string Tag { get; set; }

        public Result Status { get; set; }

        public DateTime Time { get; set; }

        public string Body { get; set; }

        public void SetBody<T>(T body)
        {
            Body = JsonConvert.SerializeObject(body);
        }

        public T GetBody<T>() where T : class
        {
            return JsonConvert.DeserializeObject<T>(Body);
        }
    }
}
