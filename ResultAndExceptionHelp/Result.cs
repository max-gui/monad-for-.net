using BlockHelp;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public static Result Fail(string message) { return new Result(false, message); }
        public static Result Ok() { return new Result(true, string.Empty); }

        public Result<T> RapperWithValue<T>() { return this.Success ? Result.Ok<T>(default(T)) : Result.Fail<T>(this.ErrorMessage); }

        public static Result Juge(Func<bool> jugeAct, string errorMessage)
        {
            return Juge(jugeAct(), errorMessage);
        }

        public static Result Juge(bool jugeText, string errorMessage)
        {
            return jugeText ?
                new Result(true, string.Empty) :
                new Result(false, errorMessage);
        }

        public static Result Combine(params Result[] paras)
        {
            return Combine(paras, 0, paras.Length, new Result(true, string.Empty));
        }
        protected static Result Combine(Result[] paras, int index, int length, Result resultAcc)
        {
            return resultAcc.Success && length > index ? Combine(paras, index + 1, length, paras[index]) : resultAcc;

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


        public Result OnSuccess(Func<Result, Result> actOnSuccess)
        {
            return this.Success ? BlockHelp.BlockMethod.CodeBlockHelp(() =>
            {
                try
                {
                    return actOnSuccess(this);
                }
                catch (Exception e)
                {
                    return Result.Fail(e.Message);
                }
            }) : this;
        }

        public Result<T> OnSuccessWithValue<T>(Func<Result, Result<T>> actOnSuccess)
        {
            return this.Success ? BlockHelp.BlockMethod.CodeBlockHelp(() =>
            {
                try
                {
                    return actOnSuccess(this);
                }
                catch (Exception e)
                {
                    return Result.Fail<T>(e.Message);
                }
            }) : Result.Fail<T>(this.ErrorMessage);

            //actOnSuccess(this) : Result.Fail<T>(this.ErrorMessage);// this;
        }

        public Result WhatEver(Func<Result, Result> actOnBoth)
        {
            try
            {
                return actOnBoth(this);
            }
            catch (Exception e)
            {
                return Result.Fail(e.Message);
            }
        }

        public Result<T> WhatEverWithValue<T>(Func<Result, Result<T>> actOnBoth)
        {
            try
            {
                return actOnBoth(this);
            }
            catch (Exception e)
            {
                return Result.Fail<T>(e.Message);
            }
        }

        public Result OnFailure(Func<Result, Result> actOnFail)
        {
            return this.Failure ?
                BlockMethod.CodeBlockHelp<Result>(() =>
                {
                    try
                    {
                        return actOnFail(this);
                    }
                    catch (Exception e)
                    {
                        return Result.Fail(e.Message);
                    }
                })
                : this;
        }

        public Result<T> OnFailureWithValue<T>(Func<Result, Result<T>> actOnFail)
        {
            return this.Failure ?
                BlockMethod.CodeBlockHelp<Result<T>>(() =>
                {
                    try
                    {
                        return actOnFail(this);
                    }
                    catch (Exception e)
                    {
                        return Result.Fail<T>(e.Message);
                    }
                })
                : this.RapperWithValue<T>(); ;
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

        public Result<T> OnSuccessWithValue(Func<Result<T>, Result<T>> actOnSuccess)
        {
            return this.Success ? BlockHelp.BlockMethod.CodeBlockHelp(() =>
            {
                try
                {
                    return actOnSuccess(this);
                }
                catch (Exception e)
                {
                    return Result.Fail<T>(e.Message);
                }
            }) : Result.Fail<T>(this.ErrorMessage);

            //actOnSuccess(this) : Result.Fail<T>(this.ErrorMessage);// this;
        }

        public Result<T> WhatEverWithValue(Func<Result<T>, Result<T>> actOnBoth)
        {
            try
            {
                return actOnBoth(this);
            }
            catch (Exception e)
            {
                return Result.Fail<T>(e.Message);
            }
        }

        public Result<T> OnFailureWithValue(Func<Result<T>, Result<T>> actOnFail)
        {
            return this.Failure ?
                BlockMethod.CodeBlockHelp<Result<T>>(() =>
                {
                    try
                    {
                        return actOnFail(this);
                    }
                    catch (Exception e)
                    {
                        return Result.Fail<T>(e.Message);
                    }
                })
                : this;
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

    public interface IReposite<T, KeyType>
    {
        Result<List<T>> GetAll();
        Result<T> GetById(KeyType id);
        Result Add(T value);
        Result Del(KeyType id);
        Result Update(T Value);
    }
}
