using System;
using AutoMapper;
using ProjectV.DataAccessLayer.Services.Jobs.Models;
using ProjectV.DataAccessLayer.Services.Tokens.Models;
using ProjectV.DataAccessLayer.Services.Users.Models;
using ProjectV.Models.Authorization.Tokens;
using ProjectV.Models.Internal.Jobs;
using ProjectV.Models.Users;

namespace ProjectV.DataAccessLayer
{
    public sealed class DataAccessLayerMapperProfile : Profile
    {
        public DataAccessLayerMapperProfile()
        {
            MapJobModels();
            MapUserModels();
            MapTokenModels();
        }

        private void MapJobModels()
        {
            CreateMap<JobDbInfo, JobInfo>();
            CreateMap<JobInfo, JobDbInfo>();

            CreateMap<Guid, JobId>()
                .ConvertUsing(guid => JobId.Wrap(guid));
            CreateMap<JobId, Guid>()
               .ConvertUsing(jobId => jobId.Value);
        }

        private void MapUserModels()
        {
            CreateMap<UserDbInfo, UserInfo>();
            CreateMap<UserInfo, UserDbInfo>();

            CreateMap<Guid, UserId>()
                .ConvertUsing(guid => UserId.Wrap(guid));
            CreateMap<UserId, Guid>()
               .ConvertUsing(jobId => jobId.Value);

            CreateMap<string, UserName>()
               .ConvertUsing(str => UserName.Wrap(str));
            CreateMap<UserName, string>()
               .ConvertUsing(userName => userName.Value);
        }

        private void MapTokenModels()
        {
            CreateMap<RefreshTokenDbInfo, RefreshTokenInfo>();
            CreateMap<RefreshTokenInfo, RefreshTokenDbInfo>();

            CreateMap<Guid, RefreshTokenId>()
                .ConvertUsing(guid => RefreshTokenId.Wrap(guid));
            CreateMap<RefreshTokenId, Guid>()
               .ConvertUsing(jobId => jobId.Value);
        }
    }
}
