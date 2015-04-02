using BlockHelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResultAndExceptionHelp
{
    public class Result
    {
        public bool Success { get;private set; }
        public string Error {get;private set;}
        public bool Failure { get { return !Success; } private set { } }

        protected Result(bool success, string error) { Success = success; Error = error; }
        public static Result<T> Fail<T>(string message) { return new Result<T>(default(T), false, message); }
        public static Result<T> Ok<T>(T value) { return new Result<T>(value, true); }

        public static Result Combine(params Result[] paras)
        {
            return Combine(paras, 0, paras.Length, new Result(true, string.Empty));
        }

        protected static Result Combine(Result[] paras,int index, int length, Result resultAcc)
        {
            return resultAcc.Success && length > index ? Combine(paras, index + 1, length, paras[index]) : resultAcc;

        }

        public Result OnSuccess(Func<Result,Result> actOnSuccess)
        {
            return this.Success ? actOnSuccess(this) : this;
        }

        public Result WhatEver(Func<Result, Result> actOnBoth)
        {
            actOnBoth(this);
            return this;
        }

        public Result OnFailure(Func<Result, Result> actOnFail)
        {
            return this.Failure ? 
                BlockMethod.CodeBlockHelp<Result>(()=>
                {
                    actOnFail(this);
                    return this;
                })
                : this;
        }

    }

    public class Result<T> : Result
    {
        public T Value { get; set; }

        protected internal Result(T value, bool success, string error = default(string))
            : base(success, error)
        {
            Value = value;
        }
    }
}
