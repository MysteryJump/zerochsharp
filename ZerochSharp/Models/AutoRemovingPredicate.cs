using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet;

namespace ZerochSharp.Models
{
    public class AutoRemovingPredicate
    {
        private List<ArchivingPredicate> predicates;
        private AutoRemovingPredicate()
        {
            predicates = new List<ArchivingPredicate>();
        }
        public async Task<AutoRemovingPredicate> Build(string path)
        {
            var predicate = new AutoRemovingPredicate();
            var file = File.ReadAllTextAsync(path);
            var deserializer = new YamlDotNet.Serialization.Deserializer();
            predicates.AddRange(deserializer.Deserialize<ArchivingPredicate[]>(await file));
            return predicate;
        }
        public IEnumerable<Thread> FilterRemoveThread(IEnumerable<Thread> threads)
        {
            return threads.Where(x => !IsDelete(x));
        }

        private bool IsDelete(Thread thread)
        {
            foreach (var item in predicates)
            {
                var targetNum = 0L;
                targetNum = item.Target switch
                {
                    ArchivingPredicate.TargetType.Count => thread.ResponseCount,
                    ArchivingPredicate.TargetType.CreatedAt =>
                            targetNum = new DateTimeOffset(thread.Created, new TimeSpan(+9, 0, 0)).ToUnixTimeSeconds(),
                    ArchivingPredicate.TargetType.ModifiedAt =>
                            targetNum = new DateTimeOffset(thread.Modified, new TimeSpan(+9, 0, 0)).ToUnixTimeSeconds(),
                    ArchivingPredicate.TargetType.Influence =>
                            targetNum = (long)thread.Influence,
                    _ => throw new InvalidOperationException("doesn't match expression")
                };
                
            }

            return false;
        }
        private class ArchivingPredicate
        {
            public EvaluationType EvalType { get; set; }
            public TargetType Target { get; set; }
            public int ComparisonNumber { get; set; }

            public enum EvaluationType
            {
                Less,
                LessOrEqual,
                Greater,
                GreaterOrEqual,
                Equal
            }

            public enum TargetType
            {
                Count,
                CreatedAt,
                ModifiedAt,
                Influence
            }
        }
    }

}
