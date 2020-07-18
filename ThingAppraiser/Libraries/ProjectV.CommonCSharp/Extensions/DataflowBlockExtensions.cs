using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks.Dataflow;

namespace ProjectV.Extensions
{
    public static class DataflowBlockExtensions
    {
        public static void MarkAsCompletedSafe(this IEnumerable<IDataflowBlock> queues)
        {
            // Do not throw exceptions in this method!
            try
            {
                foreach (IDataflowBlock targetBlock in queues)
                {
                    MarkBlockAsCompletedSafe(targetBlock);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    $"[{nameof(MarkAsCompletedSafe)}] Exception: {ex.ToString()}"
                );
            }
        }

        public static void MarkAsFaultedSafe(this IEnumerable<IDataflowBlock> queues,
            Exception exception)
        {
            // Do not throw exceptions in this method!
            try
            {
                foreach (IDataflowBlock targetBlock in queues)
                {
                    MarkBlockAsFaultedSafe(targetBlock, exception);
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    $"[{nameof(MarkAsFaultedSafe)}] Exception: {ex.ToString()}"
                );
            }
        }

        private static void MarkBlockAsCompletedSafe(IDataflowBlock dataflowBlock)
        {
            try
            {
                dataflowBlock.Complete();
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    $"[{nameof(MarkBlockAsCompletedSafe)}] Exception: {ex.ToString()}"
                );
            }
        }

        private static void MarkBlockAsFaultedSafe(IDataflowBlock dataflowBlock,
            Exception exception)
        {
            try
            {
                dataflowBlock.Fault(exception);
            }
            catch (Exception ex)
            {
                Trace.TraceError(
                    $"[{nameof(MarkBlockAsFaultedSafe)}] Exception: {ex.ToString()}"
                );
            }
        }
    }
}
