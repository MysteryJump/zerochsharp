using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace ZerochSharp.Models
{
    [Serializable]
    public class BBSErrorException : InvalidOperationException
    {
        public BBSError BBSError { get; private set; }
        public BBSErrorException(int errorCode) : base(errorCode.ToString())
        {
            BBSError = BBSError.FindError(errorCode);
            
        }
        public BBSErrorException(string errorName) : base(errorName)
        {
            BBSError = BBSError.FindError(errorName);
        }
        public BBSErrorException(BBSErrorType type) : base(type.ToString())
        {
            BBSError = BBSError.FindError(type);
        }
    }
    // This enums support bbs-errors.yaml
    public enum BBSErrorType
    {
        BBSNoTitleError = 150,
        BBSNoContentError = 151,
        BBSNoNameError = 152,
        BBSThreadStoppedError = 200,
        BBSSameDatKeyError = 210,
        BBSInvalidThreadKeyError = 900,
        BBSNotFoundThreadError = 901,
        BBSNotFoundBoardError = 902,
        BBSInternalError = 999
    }
    [Serializable]
    public class BBSError
    {
        [YamlIgnore]
        [NonSerialized]
        private const string BBS_ERROR_PATH = "bbs-errors.yaml";
        // bbsErrors does not need sorted.
        [YamlIgnore]
        [NonSerialized]
        private static readonly List<BBSError> bbsErrors;
        static BBSError()
        {
            bbsErrors = new List<BBSError>();
        }
        [YamlMember(Alias = "error_code")]
        public int ErrorCode { get; set; }
        [YamlMember(Alias = "name")]
        public string Name { get; set; }
        [YamlMember(Alias = "error_message")]
        public string ErrorMessage { get; set; }
        [YamlMember(Alias = "response_code")]
        [JsonIgnore]
        public int ResponseCode { get; set; }
        public static async Task InitializeBBSErrors()
        {
            var data = await File.ReadAllTextAsync(BBS_ERROR_PATH);
            var deserializer = new Deserializer();
            var deserializedObj = deserializer.Deserialize<BBSError[]>(data);
            bbsErrors.AddRange(deserializedObj);
            // bbsErrors.Sort();
        }

        public static BBSError FindError(int errorCode)
        {
            return bbsErrors.First(x => x.ErrorCode == errorCode);
        }

        public static BBSError FindError(string errorName)
        {
            return bbsErrors.First(x => x.ErrorMessage == errorName);
        }

        public static BBSError FindError(BBSErrorType type)
        {
            return bbsErrors.First(x => x.ErrorCode == (int)type);
        }
    }

    public static class Extensions
    {
        /// <summary>
        /// Run binary search with predicate
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">Target list</param>
        /// <param name="target">Target object</param>
        /// <param name="predicate">Same as <see cref="IComparable"/></param>
        /// <exception cref="ArgumentException">Given list has not target value.</exception>
        /// <returns>Index of find item</returns>
        public static int BinarySearch<T1,T2>(this IList<T1> list, T2 target, Func<T1, T2, int> predicate)
        {
            var length = list.Count;
            var left = -1;
            var right = length;
            while (right - left > 0)
            {
                var median = left + (right - left) / 2;
                var eval = predicate(list[median], target);
                if (eval == 0)
                {
                    return median;
                }
                else if (eval < 0)
                {
                    left = median;
                }
                else
                {
                    right = median;
                }
            }
            throw new InvalidOperationException();
        }
    }
}
