using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class Result
    {
        public bool isSuccess { get;}
        public IReadOnlyList<Error> Errors { get; }

        protected Result(bool isSuccess, IReadOnlyList<Error> errors)
        {
            this.isSuccess = isSuccess;
            Errors = errors;
        }
        public static Result Ok() => new Result(true, Array.Empty<Error>());
        public static Result Fail(Error errors) => new Result(false, new[] { errors });
        public static Result Fail(IReadOnlyList<Error> errors) => new Result(false,errors);

    }
    public class Result<T> : Result
    {
        private readonly T Value;

        public T data => isSuccess ? Value : throw new InvalidOperationException("Cannot access the value of a failed result");
        private Result(T value) : base(true, Array.Empty<Error>())
        {
            Value = value;
        }
        private Result(Error error) : base(false, new[] { error })
        {
            Value = default!;
        }
        private Result(IReadOnlyList<Error> error) : base(false,error)
        {
            Value = default!;
        }
      

        public static Result<T> Ok(T value) => new Result<T>(value);
        public static new Result<T> Fail(Error error) => new Result<T>(error);
        public static new Result<T> Fail(IReadOnlyList <Error> error) => new Result<T>(error);
    }

}
