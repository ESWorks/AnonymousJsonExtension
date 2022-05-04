using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace ObjectExtensions
{
    public static partial class ObjectExtensions
    {
        public static ValueTask<T> FromJsonAsync<T>(this Stream s, JsonSerializerOptions options = default, CancellationToken cancellationToken = default) => JsonSerializer.DeserializeAsync<T>(s, options, cancellationToken);
        public static ValueTask<T> FromJsonAsync<T>(this T entity, string s, JsonSerializerOptions options = default, CancellationToken cancellationToken = default) => JsonSerializer.DeserializeAsync<T>(new MemoryStream(Encoding.UTF8.GetBytes(s ?? "")), options, cancellationToken);
        public static async ValueTask<string> ToJsonNoNullsAsync<T>(this T entity, CancellationToken cancellationToken = default) => await ToJsonAsync(entity, JsonIgnoreCondition.WhenWritingNull, cancellationToken);
        public static async ValueTask<string> ToJsonAsync<T>(this T entity, JsonIgnoreCondition nullHandler = JsonIgnoreCondition.Never, CancellationToken cancellationToken = default)
        {
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, entity, new JsonSerializerOptions() { DefaultIgnoreCondition = nullHandler }, cancellationToken);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }
        public static async ValueTask<string> ToJsonAsync<T>(this T entity, JsonSerializerOptions options = default, CancellationToken cancellationToken = default) 
        {
            using (var stream = new MemoryStream())
            {
                await JsonSerializer.SerializeAsync(stream, entity, options, cancellationToken);
                stream.Position = 0;
                using var reader = new StreamReader(stream);
                return await reader.ReadToEndAsync();
            }
        }

        public static T FromJson<T>(this string s) => (T)JsonSerializer.Deserialize(s, typeof(T));
        public static T FromJson<T>(this T entity, string s) => (T)JsonSerializer.Deserialize(s, typeof(T));
        public static string ToJsonNoNulls<T>(this T entity) => ToJson(entity, new JsonSerializerOptions() { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
        public static string ToJson<T>(this T entity, JsonSerializerOptions options = default) => options is null ? JsonSerializer.Serialize(entity, typeof(T), options) : JsonSerializer.Serialize(entity);
    }

}
