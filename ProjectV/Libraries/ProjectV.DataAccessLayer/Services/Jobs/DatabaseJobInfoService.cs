using System.Threading.Tasks;
using Acolyte.Assertions;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ProjectV.DataAccessLayer.Services.Basic;
using ProjectV.DataAccessLayer.Services.Jobs.Models;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DataAccessLayer.Services.Jobs
{
    public sealed class DatabaseJobInfoService : DataServiceBase<JobId, JobInfo>, IJobInfoService
    {
        private readonly ProjectVDbContext _context;

        private readonly IMapper _mapper;


        public DatabaseJobInfoService(
            ProjectVDbContext context,
            IMapper mapper)
        {
            _context = context.ThrowIfNull(nameof(context));
            _mapper = mapper.ThrowIfNull(nameof(mapper));
        }

        #region IJobInfoService Implementation

        public async Task<int> AddAsync(JobInfo jobInfo)
        {
            jobInfo.ThrowIfNull(nameof(jobInfo));

            return await _context.ExecuteIfCanUseDb(
                () => _context.GetJobDbSet(),
                dbSet => AddJobAsync(dbSet)
            );

            async ValueTask<int> AddJobAsync(DbSet<JobDbInfo> dbSet)
            {
                var jobDbModel = _mapper.Map<JobDbInfo>(jobInfo);

                await dbSet.AddAsync(jobDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        public override async Task<JobInfo?> FindByIdAsync(JobId jobId)
        {
            JobDbInfo? jobDbModel = await _context.ExecuteIfCanUseDb(
                () => _context.GetJobDbSet(),
                dbSet => dbSet.FindAsync(jobId.Value)
            );

            return _mapper.Map<JobInfo>(jobDbModel);
        }

        public async Task<int> UpdateAsync(JobInfo jobInfo)
        {
            jobInfo.ThrowIfNull(nameof(jobInfo));

            return await _context.ExecuteIfCanUseDb(
                () => _context.GetJobDbSet(),
                dbSet => UpdateJobAsync(dbSet)
            );

            async ValueTask<int> UpdateJobAsync(DbSet<JobDbInfo> dbSet)
            {
                var jobDbModel = _mapper.Map<JobDbInfo>(jobInfo);

                dbSet.Update(jobDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        public async Task<int> DeleteAsync(JobId jobId)
        {
            return await _context.ExecuteIfCanUseDb(
                () => _context.GetJobDbSet(),
                dbSet => DeleteJobAsync(dbSet)
            );

            async ValueTask<int> DeleteJobAsync(DbSet<JobDbInfo> dbSet)
            {
                JobInfo? jobInfo = await FindByIdAsync(jobId);
                var jobDbModel = _mapper.Map<JobDbInfo>(jobInfo);

                dbSet.Remove(jobDbModel);
                return await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
