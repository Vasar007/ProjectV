using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DataAccessLayer.Orm
{
    internal sealed class JobMapper : IMapper<JobInfo, JobDbInfo>
    {
        public JobMapper()
        {
        }

        #region IMapper<JobInfo, JobDbInfo> Implementation

        public JobInfo Map(JobDbInfo jobDbInfo)
        {
            return new JobInfo(
                id: JobId.Wrap(jobDbInfo.Id),
                name: jobDbInfo.Name,
                state: jobDbInfo.State,
                result: jobDbInfo.Result,
                config: jobDbInfo.Config
            );
        }

        public JobDbInfo Map(JobInfo jobInfo)
        {
            return new JobDbInfo(
                id: jobInfo.Id.Value,
                name: jobInfo.Name,
                state: jobInfo.State,
                result: jobInfo.Result,
                config: jobInfo.Config
            );
        }

        #endregion
    }
}
