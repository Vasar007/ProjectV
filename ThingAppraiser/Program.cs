using ThingAppraiser.Core;

namespace ThingAppraiser
{
    public class Program
    {
        private static void Main(string[] args)
        {
            // Show the case when we have a movies to appraise.
            var shell = new Shell();

            Shell.Status status;
            if (args.Length == 1)
            {
                status = shell.Run(args[0]);
            }
            else
            {
                Shell.OutputMessage("Enter filename which contains the Things: ");
                status = shell.Run(Shell.GetMessage());
            }

            if (status == Shell.Status.Nothing)
            {
                Shell.OutputMessage("Result is empty. Closing...");
                Shell.GetMessage();
                return;
            }

            Shell.OutputMessage("Work was finished! Press enter to exit...");
            Shell.GetMessage();
        }
    }

}
