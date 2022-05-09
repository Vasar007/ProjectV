﻿using System;
using System.Threading.Tasks;
using Acolyte.Common;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.Core.Services.Clients
{
    public interface ICommunicationServiceClient : IDisposable
    {
        Task<Result<TokenResponse, ErrorResponse>> LoginAsync(LoginRequest login);
        Task<Result<ProcessingResponse, ErrorResponse>> StartJobAsync(StartJobParamsRequest jobParams);
    }
}