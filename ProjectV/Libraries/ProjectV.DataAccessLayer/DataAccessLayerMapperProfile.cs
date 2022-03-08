using System;
using AutoMapper;
using ProjectV.DataAccessLayer.Services.Jobs;
using ProjectV.Models.Internal.Jobs;

namespace ProjectV.DataAccessLayer
{
    public sealed class DataAccessLayerMapperProfile : Profile
    {
        public DataAccessLayerMapperProfile()
        {
            CreateMap<JobDbInfo, JobInfo>();
            CreateMap<JobInfo, JobDbInfo>();

            CreateMap<Guid, JobId>()
                .ConvertUsing(guid => JobId.Wrap(guid));
            CreateMap<JobId, Guid>()
               .ConvertUsing(jobId => jobId.Value);
        }
    }
}
