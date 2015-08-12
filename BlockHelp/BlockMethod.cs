using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlockHelp
{
    public class BlockMethod
    {
        public static T CodeBlockHelp<T>(Func<T> f)
        {
            return f();
        }

        public static bool IfElseHelp(Func<bool> flag, Action trueBlock, Action falseBlock)
        {
            return
                flag() ?
                CodeBlockHelp<bool>(() => { trueBlock(); return true; }) :
                CodeBlockHelp<bool>(() => { falseBlock(); return false; });
        }
        public static bool IfElseHelp(Func<bool> flag, Action trueBlock)
        {
            return
                flag() ?
                CodeBlockHelp<bool>(() => { trueBlock(); return true; }) :
                false;
        }

        public delegate Func<A, R> Recursive<A, R>(Recursive<A, R> r);

        public static Func<A, R> Y<A, R>(Func<Func<A, R>, Func<A, R>> f)
        {
            Recursive<A, R> rec = r => a => f(r(r))(a);
            return rec(rec);
        }
    }
}
