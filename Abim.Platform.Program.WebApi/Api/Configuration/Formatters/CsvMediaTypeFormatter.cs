using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Web;

namespace Abim.Platform.Program.WebApi.Formatters
{
    /// <summary>
    /// CsvMediaTypeFormatter Class
    /// </summary>
    public class CsvMediaTypeFormatter : BufferedMediaTypeFormatter
    {
        /// <summary>
        /// The selector
        /// </summary>
        public Func<object, HttpRequestMessage, object> Selector;

        /// <summary>
        /// The filename
        /// </summary>
        public string Filename = "export.csv";

        /// <summary>
        /// The special chars
        /// </summary>
        private static readonly char[] SpecialChars = { ',', '\n', '\r', '"' };

        /// <summary>
        /// The request
        /// </summary>
        private readonly HttpRequestMessage Request;

        /// <summary>
        /// The fields
        /// </summary>
        private readonly string Fields;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        private CsvMediaTypeFormatter(HttpRequestMessage request) : this()
        {
            if(request == null) return;
            Request = request;
            Fields = HttpUtility.ParseQueryString(Request.RequestUri.Query)["fields"] ?? "*";
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CsvMediaTypeFormatter"/> class.
        /// </summary>
        public CsvMediaTypeFormatter()
        {
            SupportedMediaTypes.Add(new MediaTypeHeaderValue("text/csv"));
            this.AddQueryStringMapping("accept", "text-csv", "text/csv");
        }

        /// <summary>
        /// Returns a specialized instance of the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> that can format a response for the given parameters.
        /// </summary>
        /// <param name="type">The type to format.</param>
        /// <param name="request">The request.</param>
        /// <param name="mediaType">The media type.</param>
        /// <returns>
        /// Returns <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" />.
        /// </returns>
        public override MediaTypeFormatter GetPerRequestFormatterInstance(Type type, HttpRequestMessage request, MediaTypeHeaderValue mediaType)
        {
            return new CsvMediaTypeFormatter(request)
            {
                Selector = Selector,
                Filename = Filename
            };
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can serializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to serialize.</param>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can serialize the type; otherwise, false.
        /// </returns>
        /// <exception cref="System.ArgumentNullException"></exception>
        public override bool CanWriteType(Type type)
        {
            if(type == null)
                throw new ArgumentNullException(); //nameof(type));
            
            return true;
        }

        /// <summary>
        /// Queries whether this <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserializean object of the specified type.
        /// </summary>
        /// <param name="type">The type to deserialize.</param>
        /// <returns>
        /// true if the <see cref="T:System.Net.Http.Formatting.MediaTypeFormatter" /> can deserialize the type; otherwise, false.
        /// </returns>
        public override bool CanReadType(Type type)
        {
            return false;
        }

        /// <summary>
        /// Writes synchronously to the buffered stream.
        /// </summary>
        /// <param name="type">The type of the object to serialize.</param>
        /// <param name="value">The object value to write. Can be null.</param>
        /// <param name="writeStream">The stream to which to write.</param>
        /// <param name="content">The <see cref="T:System.Net.Http.HttpContent" />, if available. Can be null.</param>
        public override void WriteToStream(Type type, object value, Stream writeStream, HttpContent content)
        {
            using (var writer = new StreamWriter(writeStream))
            {
                var obj = ApplyFunc(value, Request);
                var objType = obj.GetType().GetGenericArguments().First();
                var dt = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj), (typeof(DataTable)));
                var cols = GetColumnNames(objType);
                var columnNames = cols.Select(column => "\"" + column.Value.Replace("\"", "\"\"") + "\"").ToArray();

                writer.WriteLine(string.Join(",", columnNames));

                foreach (var fields in from DataRow row in dt.Rows select cols.Select(column => Escape(row[column.Key])).ToList())

                    writer.WriteLine(string.Join(",", fields));
            }
        }

        /// <summary>
        /// Sets the default headers for content that will be formatted using this formatter. This method is called from the <see cref="T:System.Net.Http.ObjectContent" /> constructor. This implementation sets the Content-Type header to the value of mediaType if it is not null. If it is null it sets the Content-Type to the default media type of this formatter. If the Content-Type does not specify a charset it will set it using this formatters configured <see cref="T:System.Text.Encoding" />.
        /// </summary>
        /// <param name="type">The type of the object being serialized. See <see cref="T:System.Net.Http.ObjectContent" />.</param>
        /// <param name="headers">The content headers that should be configured.</param>
        /// <param name="mediaType">The authoritative media type. Can be null.</param>
        public override void SetDefaultContentHeaders(Type type, HttpContentHeaders headers, MediaTypeHeaderValue mediaType)
        {
            base.SetDefaultContentHeaders(type, headers, mediaType);
            headers.Add("Content-Disposition", "attachment; filename=" + Filename);
        }

        /// <summary>
        /// Gets the column names.
        /// </summary>
        /// <param name="t">The t.</param>
        /// <returns></returns>
        private Dictionary<string, string> GetColumnNames(Type t)
        {
            var columnNames = new Dictionary<string, string>();
            var type = typeof(CsvColumnAttribute);

            foreach (var propertyInfo in t.GetProperties().Where(propertyInfo => Fields.IndexOf(propertyInfo.Name, StringComparison.OrdinalIgnoreCase) >= 0 || Fields == "*"))
            {

                if(Attribute.IsDefined(propertyInfo, type))
                {

                    var attributeInstance = Attribute.GetCustomAttribute(propertyInfo, type);

                    if(attributeInstance != null)
                    {
                        foreach (var info in type.GetProperties().Where(info => info.CanRead && string.Compare(info.Name, "name", StringComparison.InvariantCultureIgnoreCase) == 0))

                            columnNames.Add(propertyInfo.Name, info.GetValue(attributeInstance, null).ToString());
                    }
                    else
                        columnNames.Add(propertyInfo.Name, propertyInfo.Name);

                }
                else
                    columnNames.Add(propertyInfo.Name, propertyInfo.Name);

            }

            return columnNames;

        }

        /// <summary>
        /// Applies the function.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="request">The request.</param>
        /// <returns></returns>
        private object ApplyFunc(object value, HttpRequestMessage request)
        {
            return Selector != null ? Selector(value, request) : value;
        }

        /// <summary>
        /// Escapes the specified o.
        /// </summary>
        /// <param name="o">The o.</param>
        /// <returns></returns>
        private static string Escape(object o)
        {
            var field = o.ToString();
            
            //TODO: fix (pasted from example code)
            //return field.IndexOfAny(SpecialChars) != -1 ? $"\"{field.Replace("\"", "\"\"")}\"" : field;
            return null;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public class CsvColumnAttribute : Attribute
    {
        public string Name { get; set; }
    }
}
