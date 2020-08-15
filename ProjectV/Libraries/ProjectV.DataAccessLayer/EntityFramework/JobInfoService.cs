using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Exceptions;
using AutoMapper;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DataAccessLayer.EntityFramework
{
    public sealed class JobInfoService : IJobInfoService
    {
        private readonly ProjectVDbContext _context;

        private readonly IMapper _mapper;


        public JobInfoService(
            ProjectVDbContext context,
            IMapper mapper)
        {
            _context = context.ThrowIfNull(nameof(context));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        #region ITaskRepository Implementation

        public async Task AddAsync(JobInfo jobInfo)
        {
            var taskDbModel = _mapper.Map<JobDbInfo>(jobInfo);
            await _context.GetJobDbSet().AddAsync(taskDbModel);

            await _context.SaveChangesAsync();
        }

        public async Task<JobInfo?> FindByIdAsync(JobId jobId)
        {
            JobDbInfo? taskDbModel = await _context.GetJobDbSet().FindAsync(jobId.Value);

            return _mapper.Map<JobInfo>(taskDbModel);
        }

        public async Task<JobInfo> GetByIdAsync(JobId jobId)
        {
            JobInfo? jobInfo = await FindByIdAsync(jobId);
            if (jobInfo is null)
            {
                throw new NotFoundException(
                    $"Failed to found job info with ID '{jobId.ToString()}'."
                );
            }

            return jobInfo;
        }

        #endregion
    }
}
