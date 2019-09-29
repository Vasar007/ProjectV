using System;
using System.Linq;
using ThingAppraiser.Extensions;

namespace ThingAppraiser.Exceptions
{
    public static class ExceptionsHelper
    {
        public static Exception UnwrapAggregateExceptionIfCan(AggregateException aggregateException)
        {
            aggregateException.ThrowIfNull(nameof(aggregateException));

            Exception resultException = aggregateException.InnerExceptions.Count == 1
                ? aggregateException.InnerExceptions.Single()
                : aggregateException;

            return resultException;
        }
    }
}
