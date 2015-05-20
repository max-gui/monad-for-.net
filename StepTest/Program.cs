using ResultAndExceptionHelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StepTest
{
    class Program
    {
        static void Main(string[] args)
        {
            var seed = 15;
            var first = Step1.Create(seed);
            var second = Step2.Create((seed + seed).ToString());
            var third = Step3.Create(decimal.Parse((seed + "." + seed)));


            AllSuccessDemo(seed, first, second, third);

            var seed2 = 16;
            
            FailOnStep1Demo(seed, first, second, third, seed2);

            FailOnStep2Demo(seed, first, second, third, seed2);

            FailOnStep3(seed, first, second, third, seed2);
                
        }

        private static void FailOnStep3(int seed, Result<Step1> first, Result<Step2> second, Result<Step3> third, int seed2)
        {
            var end4 = Result.Combine(first, second, third).
                OnSuccess(res => first.Value.Work(seed)).
                OnSuccess(res => second.Value.Work((seed + seed).ToString())).
                OnSuccess(res => third.Value.Work(decimal.Parse((seed2 + "." + seed)))).
                OnSuccess(res => { Console.WriteLine("ok"); return "ok".ResultIsOk();}).//Result.Ok<string>("ok"); }).
                OnFailure(res => { Console.WriteLine("failed"); Console.WriteLine(res.ErrorMessage); return "failed".ResultIsFailed("failed");}).//Result.Fail<string>("failed"); }).
                WhatEver(res =>
                {
                    Console.WriteLine("in the end"); Console.WriteLine(res.ErrorMessage);
                    Console.WriteLine("!------------------------------------------------------------------!");
                    return "ok".ResultIsOk();//Result.Ok<string>("ok");
                });
        }

        private static void FailOnStep2Demo(int seed, Result<Step1> first, Result<Step2> second, Result<Step3> third, int seed2)
        {
            var end3 = Result.Combine(first, second, third).
                OnSuccess(res => first.Value.Work(seed)).
                OnSuccess(res => second.Value.Work((seed + seed2).ToString())).
                OnSuccess(res => third.Value.Work(decimal.Parse((seed + "." + seed)))).
                OnSuccess(res => { Console.WriteLine("ok"); return "ok".ResultIsOk();}).// Result.Ok<string>("ok"); }).
                OnFailure(res => { Console.WriteLine("failed"); Console.WriteLine(res.ErrorMessage); return "failed".ResultIsFailed("failed"); }).//Result.Fail<string>("failed"); }).
                WhatEver(res =>
                {
                    Console.WriteLine("in the end"); Console.WriteLine(res.ErrorMessage);
                    Console.WriteLine("!------------------------------------------------------------------!");
                    return "ok".ResultIsOk();// Result.Ok<string>("ok");
                });
        }

        private static void FailOnStep1Demo(int seed, Result<Step1> first, Result<Step2> second, Result<Step3> third, int seed2)
        {
            var end2 = Result.Combine(first, second, third).
                OnSuccess(res => first.Value.Work(seed2)).
                OnSuccess(res => second.Value.Work((seed + seed).ToString())).
                OnSuccess(res => third.Value.Work(decimal.Parse((seed + "." + seed)))).
                OnSuccess(res => { Console.WriteLine("ok"); return "ok".ResultIsOk(); }).//Result.Ok<string>("ok"); }).
                OnFailure(res => { Console.WriteLine("failed"); Console.WriteLine(res.ErrorMessage); return "failed".ResultIsFailed("failed");}).// Result.Fail<string>("failed"); }).
                WhatEver(res =>
                {
                    Console.WriteLine("in the end"); Console.WriteLine(res.ErrorMessage);
                    Console.WriteLine("!------------------------------------------------------------------!");
                    return "ok".ResultIsOk();// Result.Ok<string>("ok");
                });
        }

        private static void AllSuccessDemo(int seed, Result<Step1> first, Result<Step2> second, Result<Step3> third)
        {
            var end1 = Result.Combine(first, second, third).
                OnSuccess(res => first.Value.Work(seed)).
                OnSuccess(res => second.Value.Work((seed + seed).ToString())).
                OnSuccess(res => third.Value.Work(decimal.Parse((seed + "." + seed)))).
                OnSuccess(res => { Console.WriteLine("ok"); return "ok".ResultIsOk();}).// }).
                OnFailure(res => { Console.WriteLine("failed"); Console.WriteLine(res.ErrorMessage); return "failed".ResultIsFailed("failed"); }).
                WhatEver(res =>
                {
                    Console.WriteLine("in the end");
                    Console.WriteLine(res.ErrorMessage); Console.WriteLine("!------------------------------------------------------------------!");
                    return "ok".ResultIsOk();// Result.Ok<string>("ok");
                });
        }
    }

    class Step1
    {
        public int Para { get; private set; }

        protected Step1() { }
        public static Result<Step1> Create(int para)
        {
            var resTmp =new Step1 { Para = para };
            return 
                para > 10 && para < 20 ?
                resTmp.ResultIsOk():
                resTmp.ResultIsFailed("para can't less than 11 or greater than 19");
        }

        public Result<string> Work(int inPara)
        {
            var resTmp = (Para + inPara).ToString();
            Console.WriteLine("in step1");
            return Para.Equals(inPara) ? 
                resTmp.ResultIsOk():
                resTmp.ResultIsFailed("step1 need same inPara as para");
        }
    }

    class Step2
    {
        public string Para { get; private set; }
        protected Step2() { }
        public static Result<Step2> Create(string para)
        {
            var res = 0;
            var parseTmp = int.TryParse(para, out res);
            var resTmp = new Step2 { Para = para };
            return parseTmp ?
                resTmp.ResultIsOk():
                resTmp.ResultIsFailed("para should be int string");
        }
        public Result<decimal> Work(string inPara)
        {
            var resTmp = decimal.Parse((Para + "." + inPara));
            Console.WriteLine("in step2");
            return Para.Equals(inPara) ?
                resTmp.ResultIsOk():
                resTmp.ResultIsFailed("step2 need same inPara as para");
                //Result.Ok<decimal>(decimal.Parse((Para + "." + inPara))) : Result.Fail<decimal>("step2 need same inPara as para");
        }
    }

    class Step3
    {
        public decimal Para { get; private set; }
        protected Step3() { }
        public static Result<Step3> Create(decimal para)
        {
            
            var res = para.ToString().Split('.');
            var resTmp = new Step3 { Para = para };

            return res.Length == 2 && res.First().Equals(res.Last()) ?
                resTmp.ResultIsOk():
                resTmp.ResultIsFailed("para should be a decimal with same integer part and decimal part");
        }
        public Result<int> Work(decimal inPara)
        {
            var resTmp = int.Parse(Para.ToString().Split('.').First() + int.Parse(inPara.ToString().Split('.').Last()));
            Console.WriteLine("in step3");
            return Para.Equals(inPara) ?
                resTmp.ResultIsOk():
                resTmp.ResultIsFailed("step3 need same inPara as para");
                //: Result.Fail<int>("step3 need same inPara as para");
        }
    }
}
