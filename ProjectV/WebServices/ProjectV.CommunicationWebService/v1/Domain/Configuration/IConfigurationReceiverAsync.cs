﻿using System;
using System.Threading.Tasks;
using Acolyte.Common;
using ProjectV.Models.WebServices.Requests;
using ProjectV.Models.WebServices.Responses;

namespace ProjectV.CommunicationWebService.v1.Domain.Configuration
{
    public interface IConfigurationReceiverAsync : IDisposable
    {
        Task<Result<StartJobDataResponce, ErrorResponse>> ReceiveConfigForRequestAsync(StartJobParamsRequest jobParams);
    }
}
