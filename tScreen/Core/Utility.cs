using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Web;
using Microsoft.AspNetCore.WebUtilities;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Core
{
    public static class UtilityExtensions
    {
        public static string Obscure(this string text, int previewLength, bool rightSide = false)
            => Utility.Obscure(previewLength, text, rightSide);

        public static string Checksum(this Stream stream, ChecksumAlgorithms algorithm)
            => Utility.Checksum(algorithm, stream);

        public static byte[] ToMd5(this string text)
            => Utility.ToMd5Hash(text);

        public static byte[] ToSha256(this string text)
            => Utility.ToSha256(text);

        public static string ToHex(this byte[] bytes)
            => Utility.ToHex(bytes);

        public static string ToBase64(this string text)
            => Utility.ToBase64(text);

        public static string ToBase64Url(this byte[] bytes)
            => Utility.ToBase64Url(bytes);

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> items, bool allCombinations = false)
        {
            var enumerable = items as T[] ?? items.ToArray();
            return Utility.GetPermutations(enumerable, enumerable.Length, allCombinations);
        }

        public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> items, int count, bool allCombinations = false)
            => Utility.GetPermutations(items, count, allCombinations);
    }

    public static class Utility
    {
        private static readonly JsonSerializerOptions SerializerOptions = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            ReferenceHandler = ReferenceHandler.IgnoreCycles,
            WriteIndented = false,
        };

        public static string Obscure(int previewLength, string value, bool rightSide = false)
        {
            var prev = rightSide
                ? value.Substring(value.Length - previewLength, previewLength)
                : value.Substring(0, previewLength);

            var mask = new string('*', value.Length - prev.Length);
            return rightSide ? $"{mask}{prev}" : $"{prev}{mask}";
        }

        public static string Checksum(ChecksumAlgorithms algorithm, Stream stream)
        {
            using HashAlgorithm hashAlgorithm = algorithm switch
            {
                ChecksumAlgorithms.Md5 => MD5.Create(),
                ChecksumAlgorithms.Sha256 => SHA256.Create(),
                _ => throw new ArgumentOutOfRangeException(nameof(algorithm), algorithm,
                    "Hashing Algorithm not supported")
            };

            var hash = hashAlgorithm.ComputeHash(stream);
            return BitConverter.ToString(hash).Replace("-", "");
        }

        public static byte[] ToMd5Hash(string text)
        {
            using var crypto = MD5.Create();
            return crypto.ComputeHash(Encoding.ASCII.GetBytes(text));
        }

        public static byte[] ToSha256(string text)
        {
            using var crypto = SHA256.Create();
            return crypto.ComputeHash(Encoding.ASCII.GetBytes(text));
        }

        public static string ToHex(byte[] bytes)
            => BitConverter.ToString(bytes).Replace("-", "").ToLower();

        public static string ToBase64(string text)
        {
            var textBytes = Encoding.UTF8.GetBytes(text);
            return Convert.ToBase64String(textBytes);
        }

        public static string ToBase64Url(byte[] bytes)
            => WebEncoders.Base64UrlEncode(bytes);

        public static string Base64Decode(this string encoded)
        {
            if (encoded == null) return null;

            if (encoded.Length == 0) return "";

            var encodedBytes = Convert.FromBase64String(encoded);
            return Encoding.UTF8.GetString(encodedBytes);
        }

        public static string SerializeObject(object data)
            => JsonSerializer.Serialize(data, SerializerOptions);

        public static T DeserializeObject<T>(string json, string rootElement = null) where T : class
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") != "Development")
                return string.IsNullOrWhiteSpace(rootElement)
                    ? JsonSerializer.Deserialize<T>(json, SerializerOptions)
                    : JsonDocument.Parse(json).RootElement.GetProperty(rootElement).Deserialize<T>();

            try
            {
                return string.IsNullOrWhiteSpace(rootElement)
                    ? JsonSerializer.Deserialize<T>(json, SerializerOptions)
                    : JsonDocument.Parse(json).RootElement.GetProperty(rootElement).Deserialize<T>();
            }
            catch (Exception e)
            {
                var element = string.IsNullOrWhiteSpace(rootElement) ? rootElement : "$";
                throw new Exception($"{e.Message} The following JSON at root element \"{element ?? "$"}\" " +
                                    $"cannot be deserialized -> {json}", e.InnerException);
            }
        }

        public static bool IsValidJson(string data)
        {
            data = data.Trim();

            if (!((data.StartsWith('{') && data.EndsWith('}'))
                  || data.StartsWith('[') && data.EndsWith(']')))
                return false;

            try
            {
                var _ = JsonDocument.Parse(data);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string BuildUrl(string url, Dictionary<string, string> parameters)
        {
            var temp = new UriBuilder(url) { Query = BuildUrlQueryParams(parameters) };
            return temp.ToString();
        }

        public static string BuildUrlQueryParams(Dictionary<string, string> parameters)
            => string.Join("&", parameters
                .Select(param =>
                    $"{HttpUtility.UrlEncode(param.Key)}={HttpUtility.UrlEncode(param.Value)}"));

        public static string GetShortGuid() => GetShortGuid(default);

        public static string GetShortGuid(Guid? guid)
        {
            var newGuid = guid ?? Guid.NewGuid();
            return Convert.ToBase64String(newGuid.ToByteArray())[0..^2]
                .Replace("+", "-")
                .Replace("/", "_");
        }

        public static Guid DecodeShortGuid(string encoded)
        {
            if (encoded is null)
                throw new ArgumentNullException(nameof(encoded));

            var base64String = $"{encoded.Replace("-", "+").Replace("_", "/")}==";
            var bytes = Convert.FromBase64String((base64String));
            return new Guid(bytes);
        }

        public static IEnumerable<(T item, int index)> Enumerated<T>(this IEnumerable<T> self)
            => self?.Select((item, index) => (item, index)) ?? Enumerable.Empty<(T, int)>();

        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(IEnumerable<T> items, int count,
            bool allCombinations)
        {
            var enumerable = items.ToArray();
            var itemsCount = enumerable.Length;

            if (count > itemsCount)
                throw new ArgumentOutOfRangeException(nameof(count),
                    $"Count ({count}) cannot exceed the length ({itemsCount}) of the items enumerable");

            return DoPermutations(enumerable, count, allCombinations);
        }

        private static IEnumerable<IEnumerable<T>> DoPermutations<T>(IEnumerable<T> items, int count, bool allCombinations)
        {
            var i = 0;
            var enumerable = items as T[] ?? items.ToArray();

            foreach (var item in enumerable)
            {
                if (count == 1)
                    yield return new T[] { item };
                else
                {
                    var func = allCombinations
                        ? enumerable.Except(new[] { item })
                        : enumerable.Skip(i + 1);

                    foreach (var result in DoPermutations<T>(func, count - 1, allCombinations))
                        yield return new T[] { item }.Concat(result);
                }

                ++i;
            }
        }
    }

    public enum ChecksumAlgorithms
    {
        Md5,
        Sha256
    }
}