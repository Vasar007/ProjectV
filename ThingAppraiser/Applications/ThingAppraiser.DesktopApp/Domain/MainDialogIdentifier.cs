using ThingAppraiser.Extensions;

namespace ThingAppraiser.DesktopApp.Domain
{
    internal static class MainDialogIdentifier
    {
        private readonly static object _syncRoot = new object();

        private static object? _dialogIdentifier;

        public static object DialogIdentifier
        {
#pragma warning disable CS8603 // Possible null reference return.
            get => _dialogIdentifier.ThrowIfNull(nameof(_dialogIdentifier));
#pragma warning restore CS8603 // Possible null reference return.
            private set => _dialogIdentifier = value.ThrowIfNull(nameof(value));
        }

        public static bool HasValue => !(_dialogIdentifier is null);

        public static bool SetDialogIdentifierOnce(object dialogIdentifier)
        {
            if (_dialogIdentifier is null)
            {
                lock (_syncRoot)
                {
                    if (_dialogIdentifier is null)
                    {
                        DialogIdentifier = dialogIdentifier;
                        return true;
                    }
                }
            }
            return false;
        }

        public static void SetDialogIdentifierAnyway(object dialogIdentifier)
        {
            lock (_syncRoot)
            {
                DialogIdentifier = dialogIdentifier;
            }
        }
    }
}
