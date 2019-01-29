using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

using System.Management.Automation;
using System.Management.Automation.Provider;


namespace Providers
{
    [CmdletProvider("HelloWorldProvider", ProviderCapabilities.None)]
    public class HelloWorldProvider : CmdletProvider
    {
        public HelloWorldProvider()
            : base()
        {            
        }

        #region CmdletProvider overrides

        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            Console.WriteLine("Provider1::Start()");

            // perform initialization for onetime startup of the provider
            // NOTE: Any state or persistent data for the provider should be placed in 
            // a subclass of ProviderInfo and returned from this method. That instance
            // will be used in future callbacks of interaction with the provider
            // -------------------------------------------------------
            return providerInfo;
        }        

        protected override void Stop()
        {
            // perform any cleanup or resource mgmt that was created in Start()
            // ----------------------------------------------------------------
            Console.WriteLine("Provider1::Stop()");
        }

        #endregion           
    }
}
