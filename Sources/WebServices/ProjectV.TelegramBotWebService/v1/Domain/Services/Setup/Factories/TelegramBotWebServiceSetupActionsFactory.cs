using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Acolyte.Assertions;
using Acolyte.Common.Monads;
using Microsoft.Extensions.Options;
using ProjectV.CommonWebApi.Service.Setup.Actions;
using ProjectV.CommonWebApi.Service.Setup.Factories;
using ProjectV.CommonWebApi.Service.Setup.Handlers;
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

        private readonly FuncServiceSetupActionFactory _funcServiceActionFactory;

        private bool ShouldRunWebhookTaskOnSetup => _options.IsMode(TelegramBotWebServiceWorkingMode.WebhookViaServiceSetup);
        private bool IgnoreServiceSetupErrors => _options.IgnoreServiceSetupErrors;


        public TelegramBotWebServiceSetupActionsFactory(
            IOptions<TelegramBotWebServiceOptions> options,
            IBotWebhook botWebhook)
        {
            _options = options.GetCheckedValue();
            _botWebhook = botWebhook.ThrowIfNull(nameof(botWebhook));

            _funcServiceActionFactory = new FuncServiceSetupActionFactory();
        }

        #region IServiceSetupActionsFactory Implementation

        public ServicePreRunHandler CreatePreRunActions(CancellationToken cancellationToken)
        {
            _logger.Info("Creating pre-run actions.");

            IServiceSetupAction? onRunFailAction = null;

            var actions = new List<IServiceSetupAction>()
                .ApplyIf(ShouldRunWebhookTaskOnSetup, x => AppendSetWebhookTask(x, cancellationToken, out onRunFailAction));

            var handler = ServicePreRunHandler.CreateWithPossibleNoOpOnRunAction(actions, onRunFailAction);
            _logger.Info("Pre-run actions have been created.");
            return handler;
        }

        public ServicePostRunHandler CreatePostRunActions(CancellationToken cancellationToken)
        {
            _logger.Info("Creating post-run actions.");

            var actions = new List<IServiceSetupAction>()
                .ApplyIf(ShouldRunWebhookTaskOnSetup, x => AppendDeleteWebhookTask(x, cancellationToken));

            var handler = new ServicePostRunHandler(actions);
            _logger.Info("Post-run actions have been created.");
            return handler;
        }

        #endregion

        private IServiceSetupAction CreateServiceSetupAction(Func<Task> action)
        {
            return _funcServiceActionFactory.Create(action, IgnoreServiceSetupErrors);
        }

        private IServiceSetupAction CreateSetWebhookTask(CancellationToken cancellationToken)
        {
            return CreateServiceSetupAction(() => _botWebhook.SetWebhookAsync(cancellationToken));
        }

        private IServiceSetupAction CreateDeleteWebhookTask(CancellationToken cancellationToken)
        {
            return CreateServiceSetupAction(() => _botWebhook.DeleteWebhookAsync(cancellationToken));
        }

        private List<IServiceSetupAction> AppendSetWebhookTask(List<IServiceSetupAction> actions,
            CancellationToken cancellationToken, out IServiceSetupAction? onRunFailAction)
        {
            _logger.Info("Appending set webhook action.");

            var wrapper = CreateSetWebhookTask(cancellationToken);
            actions.Add(wrapper);

            onRunFailAction = CreateDeleteWebhookTask(cancellationToken);
            return actions;
        }

        private List<IServiceSetupAction> AppendDeleteWebhookTask(List<IServiceSetupAction> actions,
            CancellationToken cancellationToken)
        {
            _logger.Info("Appending delete webhook action.");

            var wrapper = CreateDeleteWebhookTask(cancellationToken);
            actions.Add(wrapper);

            return actions;
        }
    }
}
