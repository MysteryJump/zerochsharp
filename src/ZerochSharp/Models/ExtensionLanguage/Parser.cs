using System;
using System.Collections.Generic;
using System.Linq;

namespace ZerochSharp.Models.ExtensionLanguage
{
    class Parser
    {
        private List<Atomic> atomicList;
        public Atomic ParsedAtomic { get; private set; }
        public Parser(IEnumerable<Atomic> atomics)
        {
            atomicList = new List<Atomic>(atomics);
        }
        public void Parse()
        {
            var copied = atomicList.ToList();
            while (atomicList.Count > 1)
            {
                var max = -1;
                var ind = 0;
                var maxInd = 0;
                OperatorAtomic atomic = null;
                var bLeftInd = 0;
                var bRightInd = 0;
                var atom = atomicList.FirstOrDefault(x =>
                {
                    if (x is BracketAtomic braAtom)
                    {
                        return true;
                    }
                    bLeftInd++;
                    return false;
                }) as BracketAtomic;

                if (atom != null)
                {
                    var right = atomicList.FirstOrDefault(x =>
                    {
                        if (x is BracketAtomic rightItem
                            && rightItem.BracketType == BracketType.Right
                            && atom.BracketLevel == rightItem.BracketLevel)
                        {
                            return true;
                        }
                        bRightInd++;
                        return false;
                    });
                    if (atom.BracketType == BracketType.Left && right != null)
                    {
                        var parsed = BracketParse(bLeftInd + 1, bRightInd - 1);
                        atomicList.RemoveRange(bLeftInd, bRightInd - bLeftInd + 1);
                        atomicList.Insert(bLeftInd, parsed);
                        continue;
                    }
                    else
                    {
                        throw new InvalidOperationException("invalid bracket");
                    }
                }
                foreach (var item in atomicList)
                {
                    if (item is OperatorAtomic opeatom)
                    {
                        int priority = (int)opeatom.OpType;
                        if (opeatom.IsBinded)
                        {
                            priority = -1;
                        }
                        if (max < priority)
                        {
                            max = priority;
                            maxInd = ind;
                            atomic = opeatom;
                        }
                    }
                    ind++;
                }

                if (atomic != null)
                {
                    try
                    {
                        var rightInd = maxInd + 1;
                        if (atomic.Term == NTerm.Binomial)
                        {
                            var leftInd = maxInd - 1;
                            atomic.Bind(atomicList[rightInd], atomicList[leftInd]);
                            atomicList.RemoveAt(rightInd);
                            atomicList.RemoveAt(leftInd);
                        }
                        else if (atomic.Term == NTerm.Unary)
                        {
                            atomic.Bind(atomicList[rightInd]);
                            atomicList.RemoveAt(rightInd);
                        }
                    }
                    catch (IndexOutOfRangeException)
                    {
                        throw new InvalidOperationException("invalid expression");
                    }
                }
            }
            ParsedAtomic = atomicList.First();
            atomicList = copied;
        }

        private Atomic BracketParse(int leftInd, int rightInd)
        {
            var bracketInner = new List<Atomic>(atomicList.Skip(leftInd).Take(rightInd - leftInd + 1).ToList());
            var parser = new Parser(bracketInner);
            parser.Parse();
            return parser.ParsedAtomic;
        }
    }
}
