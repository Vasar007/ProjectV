﻿using System.Collections.Generic;
using System.Threading;
using Acolyte.Assertions;
using Acolyte.Common.Monads;
using Microsoft.Extensions.Options;
using ProjectV.CommonWebApi.Service.Setup.Actions;
using ProjectV.CommonWebApi.Service.Setup.Factories;
using ProjectV.Configuration;
using ProjectV.Logging;
using ProjectV.TelegramBotWebService.Options;
using ProjectV.TelegramBotWebService.v1.Domain.Webhooks;

namespace ProjectV.TelegramBotWebService.v1.Domain.Service.Setup.Factories
{
    public sealed class TelegramBotWebServiceSetupActionsFactory : IServiceSetupActionsFactory
    {
        private static readonly ILogger _logger =
            LoggerFactory.CreateLoggerFor<TelegramBotWebServiceSetupActionsFactory>();

        private readonly TelegramBotWebServiceOptions _options;

        private readonly IBotWebhook _botWebhook;

        private bool PreferServiceSetupOverHostedService => _options.PreferServiceSetupOverHostedService;


        public TelegramBotWebServiceSetupActionsFactory(
            IOptions<TelegramBotWebServiceOptions> options,
            IBotWebhook botWebhook)
        {
            _options = options.GetCheckedValue();
            _botWebhook = botWebhook.ThrowIfNull(nameof(botWebhook));
        }

        #region IServiceSetupActionsFactory Implementation

        public ServicePreRunHandler CreatePreRunActions(
            CancellationToken cancellationToken = default)
        {
            _logger.Info("Creating pre-run actions.");

            IServiceSetupAction? onRunFailAction = null;

            var actions = new List<IServiceSetupAction>()
                .ApplyIf(PreferServiceSetupOverHostedService, x => AppendSetWebhookTask(x, cancellationToken, out onRunFailAction));

            var handler = ServicePreRunHandler.CreateWithPossibleNoOpOnRunAction(actions, onRunFailAction);
            _logger.Info("Pre-run actions have been created.");
            return handler;
        }

        public ServicePostRunHandler CreatePostRunActions(
            CancellationToken cancellationToken = default)
        {
            _logger.Info("Creating post-run actions.");

            var actions = new List<IServiceSetupAction>()
                .ApplyIf(PreferServiceSetupOverHostedService, x => AppendDeleteWebhookTask(x, cancellationToken));

            var handler = new ServicePostRunHandler(actions);
            _logger.Info("Post-run actions have been created.");
            return handler;
        }

        #endregion

        private List<IServiceSetupAction> AppendSetWebhookTask(List<IServiceSetupAction> actions,
            CancellationToken cancellationToken, out IServiceSetupAction? onRunFailAction)
        {
            _logger.Info("Appending set webhook action.");

            var wrapper = new FuncServiceSetupAction(() => _botWebhook.SetWebhookAsync(cancellationToken));
            actions.Add(wrapper);

            onRunFailAction = wrapper;
            return actions;
        }

        private List<IServiceSetupAction> AppendDeleteWebhookTask(List<IServiceSetupAction> actions,
            CancellationToken cancellationToken)
        {
            _logger.Info("Appending delete webhook action.");

            var wrapper = new FuncServiceSetupAction(() => _botWebhook.DeleteWebhookAsync(cancellationToken));
            actions.Add(wrapper);

            return actions;
        }
    }
}
