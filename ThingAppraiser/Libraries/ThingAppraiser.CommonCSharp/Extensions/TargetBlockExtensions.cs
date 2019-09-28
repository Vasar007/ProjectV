using System.Collections.Generic;
using System.Threading.Tasks.Dataflow;

namespace ThingAppraiser.Extensions
{
    public static class TargetBlockExtensions
    {
        public static void MarkAsCompleted<T>(this IEnumerable<ITargetBlock<T>> queues)
        {
            foreach (ITargetBlock<T> rawDataQueue in queues)
            {
                rawDataQueue.Complete();
            }
        }
    }
}
