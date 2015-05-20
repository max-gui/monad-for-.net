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
        public bool Success { get; private set; }
        public string ErrorMessage { get; private set; }
        public bool Failure { get { return !Success; } private set { } }

        protected Result(bool success, string error) { Success = success; ErrorMessage = error; }
        public static Result<T> Fail<T>(string message, T value = default(T)) { return new Result<T>(value, false, message); }
        public static Result<T> Ok<T>(T value) { return new Result<T>(value, true); }

        public static Result Combine(params Result[] paras)
        {
            return Combine(paras, 0, paras.Length, new Result(true, string.Empty));
        }

        public static Result<TRes> Any<TRes>(params Task<Result<TRes>>[] paras)
        {
            return Any(paras, 0, paras.Length, new Result(true, string.Empty));
        }

        protected static Result<TRes> Any<TRes>(Task<Result<TRes>>[] paras, int index, int length, Result resultAcc)
        {
            var resFlag = false;
            var tIndex = -1;
            var ret = Result.Fail<TRes>(string.Empty);
            while (!resFlag && paras.Length > 0)
            {
                tIndex = Task.WaitAny(paras);

                resFlag = tIndex > -1 ?
                    paras[tIndex].Result.Success
                    : BlockHelp.BlockMethod.CodeBlockHelp(() =>
                    {
                        ret = Result.Fail<TRes>(paras[tIndex].Result.ErrorMessage + ret.ErrorMessage);
                        return false;
                    });

                paras = paras.Skip(tIndex).ToArray();
            }

            ret = resFlag ?
                paras[tIndex].Result :
                ret;

            return ret;
        }

        protected static Result Combine(Result[] paras, int index, int length, Result resultAcc)
        {
            return resultAcc.Success && length > index ? Combine(paras, index + 1, length, paras[index]) : resultAcc;

        }

        public Result OnSuccess(Func<Result, Result> actOnSuccess)
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
                BlockMethod.CodeBlockHelp<Result>(() =>
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

        protected internal Result(T value, bool success, string errorMessage = default(string))
            : base(success, errorMessage)
        {
            Value = value;
        }
    }

    public static class ResultHelp
    {
        public static Result<T> ResultIsOk<T>(this T value)
        {
            return Result.Ok(value);
        }

        public static Result<T> ResultIsFailed<T>(this T value, string errorMessage)
        {
            return Result.Fail(errorMessage, value);
        }
    }
}
