using System.ServiceProcess;

namespace getMotherDetails
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            var servicesToRun = new ServiceBase[]
            {
                new GetMotherDetails()
            };
            ServiceBase.Run(servicesToRun);
        }
        
    }
}
