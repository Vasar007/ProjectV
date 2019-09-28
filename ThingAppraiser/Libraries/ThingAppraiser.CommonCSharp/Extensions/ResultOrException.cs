using System;
using System.Diagnostics.CodeAnalysis;

namespace ThingAppraiser.Extensions
{
    public sealed class ResultOrException<T>
    {
        [MaybeNull]
        private readonly T _result;
        
        private readonly Exception? _exception;

        public bool IsSuccess { get; }
        
        public T Result => IsSuccess && !(_result is null)
            ? _result
            : throw new InvalidOperationException($"{nameof(Result)} property was not active.");
        
        public Exception Exception => !IsSuccess && !(_exception is null)
            ? _exception
            : throw new InvalidOperationException($"{nameof(Exception)} property was not active.");


        public ResultOrException(T result)
        {
            if (result is null)
            {
                throw new ArgumentNullException(nameof(result));
            }

            IsSuccess = true;
            _result = result;
            _exception = null;
        }

        public ResultOrException(Exception ex)
        {
            IsSuccess = false;
            _result = default!;
            _exception = ex.ThrowIfNull(nameof(ex));
        }

        public T GetResultOrDefault([AllowNull] T defaultResult = default!)
        {
            return IsSuccess && !(_result is null)
                ? _result
                : defaultResult;
        }

        public Exception? GetExceptionOrDefault([AllowNull] Exception? defaultException = null)
        {
            return !IsSuccess && !(_exception is null)
                ? _exception
                : defaultException;
        }
    }
}
