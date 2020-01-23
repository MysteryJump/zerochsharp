using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using YamlDotNet;
using ZerochSharp.Models.ExtensionLanguage;

namespace ZerochSharp.Models
{
    public class AutoRemovingPredicatePool
    {
        public static Dictionary<string, AutoRemovingPredicate> Predications { get; set; }
        static AutoRemovingPredicatePool()
        {
            Predications = new Dictionary<string, AutoRemovingPredicate>();
        }
    }
    public class AutoRemovingPredicate
    {
        private List<ArchivingPredicate> predicates;
        private AutoRemovingPredicate()
        {
        }
        public static async Task<AutoRemovingPredicate> Build(string path)
        {
            var predicate = new AutoRemovingPredicate();
            var file = await File.ReadAllLinesAsync(path);
            var predicates = new List<ArchivingPredicate>();
            foreach (var line in file)
            {
                var exp = new ArchivingPredicate(line);
                predicates.Add(exp);
            }
            predicate.predicates = predicates;
            return predicate;
        }
        public static AutoRemovingPredicate Build(Board board)
        {
            var splitted = board.AutoRemovingPredicate?.Split(';', StringSplitOptions.RemoveEmptyEntries);
            var predicates = new List<ArchivingPredicate>();
            foreach (var line in splitted ?? new string[] { })
            {
                predicates.Add(new ArchivingPredicate(line));
            }
            var pred = new AutoRemovingPredicate();
            pred.predicates = predicates;
            return pred;
        }
        public IEnumerable<Thread> FilterRemoveThread(IEnumerable<Thread> threads)
        {
            return threads.Where(x => IsDelete(x));
        }

        private bool IsDelete(Thread thread)
        {
            var table = BuildConstantTable(thread);
            foreach (var pred in predicates)
            {
                var ret = pred.Evaluate(table);
                if (ret)
                {
                    return true;
                }
            }
            return false;
        }

        private Dictionary<string, long> BuildConstantTable(Thread th)
        {
            return new Dictionary<string, long>()
            {
                { "CreatedAt", new DateTimeOffset(th.Created, new TimeSpan(+9, 0, 0)).ToUnixTimeSeconds() },
                { "ModifiedAt", new DateTimeOffset(th.Modified, new TimeSpan(+9, 0, 0)).ToUnixTimeSeconds() },
                { "Influence", (long)th.Influence },
                { "Count", th.ResponseCount }
            };
        }
    }
    class ArchivingPredicate
    {
        public string Expression { get; }
        public ArchivingPredicate(string expression)
        {
            Expression = expression;
        }
        public bool Evaluate(Dictionary<string, long> constantTable)
        {
            return ExtensionLanguage.ExtensionLanguage.EvaluatePredicate(Expression, constantTable);
        }
    }
}
