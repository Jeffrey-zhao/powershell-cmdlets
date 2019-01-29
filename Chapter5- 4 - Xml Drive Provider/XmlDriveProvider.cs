using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml;

using System.Management.Automation;
using System.Management.Automation.Provider;

namespace Providers
{
    /// <summary>
    /// Sample XmlDriveProvider that illustrates how to properly override the DriveCmdletProvider APIs.
    /// Since we're only illustrating the "drive" concept in this provider, the user can't do much after mapping
    /// an XML document to a drive. They can access the public XmlDocument property in the XmlDriveInfo class after
    /// getting an instance of teh drive via get-psdrive. Ideally, the user should be using cmdlets like get-item to access the 
    /// XML document and more advanced provider samples illustrate how to do this. This provider just shows the DriveCmdletProvider
    /// overrides and APIs in isolation.
    /// </summary>
    [CmdletProvider("XmlDriveProvider", ProviderCapabilities.None)]
    public class XmlDriveProvider : DriveCmdletProvider
    {
        public XmlDriveProvider()
            : base()
        {
        }
             
        // new-psdrive, get-psdrive, remove-psdrive
        #region DriveCmdletProvider overrides

        /// <summary>
        /// This callback is invoked when the provider is initialized which happens when the snapin containing the 
        /// provider is added to the PowerShell session. This method enables the provider implementor to create any
        /// default drives that should be created without any user invention. 
        /// For example: If this were a filesystem provider, we would create PSDriveInfo instances for each drive 
        /// present in the OS's filesystem.
        /// </summary>
        /// <returns></returns>
        protected override Collection<PSDriveInfo> InitializeDefaultDrives()
        {
            WriteVerbose("XmlDriveProvider::InitializeDefaultDrives()");
            return null;
        }

        /// <summary>
        /// Callback for new-psdrive
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            WriteVerbose("XmlDriveProvider::NewDrive()");
            RuntimeDefinedParameterDictionary dynamicParameters = base.DynamicParameters as RuntimeDefinedParameterDictionary;
            string path = dynamicParameters["path"].Value.ToString();
            return new XmlDriveInfo(path, drive);
        }

        /// <summary>
        /// callback for remove-psdrive
        /// </summary>
        /// <param name="drive"></param>
        /// <returns></returns>
        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            WriteVerbose("XmlDriveProvider::RemoveDrive()");
            XmlDriveInfo xmldrive = drive as XmlDriveInfo;
            xmldrive.XmlDocument.Save(xmldrive.Path);
            xmldrive.XmlDocument = null;
            return base.RemoveDrive(drive);
        }

        /// <summary>
        /// This callback method returns dynamic parameters for the new-psdrive cmdlet. Note that we create a mandatory
        /// dynamic parameter which means the user must supply a value for it when executing new-psdrive for this provider.
        /// </summary>
        /// <returns></returns>
        protected override object NewDriveDynamicParameters()
        {
            WriteVerbose("XmlDriveProvider::NewDriveDynamicParameters()");
            
            RuntimeDefinedParameterDictionary dynamicParameters = new RuntimeDefinedParameterDictionary();
            Collection<Attribute> atts = new Collection<Attribute>();
            ParameterAttribute parameterAttribute = new ParameterAttribute();
            parameterAttribute.Mandatory = true;
            atts.Add(parameterAttribute);
            dynamicParameters.Add("path", new RuntimeDefinedParameter("path", typeof(string), atts));
            
            return dynamicParameters;
        }

        #endregion
        
        #region CmdletProvider overrides

        /// <summary>
        /// Callback when the provider is intially loaded which occurs at snapin loading time.
        /// </summary>
        /// <param name="providerInfo"></param>
        /// <returns></returns>
        protected override ProviderInfo Start(ProviderInfo providerInfo)
        {
            WriteVerbose("XmlDriveProvider::Start()");

            // perform initialization for onetime startup of the provider
            // NOTE: Any state or persistent data for the provider should be placed in 
            // a subclass of ProviderInfo and returned from this method. That instance
            // will be used in future callbacks of interaction with the provider
            // -------------------------------------------------------
            return providerInfo;
        }       

        /// <summary>
        /// Callback that is invoked when the provider is stopped which occurs when the snapin is unloaded via
        /// remove-pssnapin
        /// </summary>
        protected override void Stop()
        {
            // perform any cleanup or resource mgmt that was created in Start()
            // ----------------------------------------------------------------
            WriteVerbose("XmlDriveProvider::Stop()");
        }

        #endregion           
    }

    /// <summary>
    /// Derived class from PSDriveInfo so we can add our own personal information to the drive context.
    /// In this case, we're adding the Path & XmlDocument property for use in our provider. We decided to map each
    /// XML document to a drive and so this information is needed in order to access the XmlDocument instance that
    /// we will use to navigate the XML file.
    /// </summary>
    public class XmlDriveInfo : PSDriveInfo
    {
        private string _path;
        private XmlDocument _xml;
        private XmlNamespaceManager _mgr;

        public string Path
        {
            get { return _path; }
        }
        public XmlDocument XmlDocument
        {
            get { return _xml; }
            internal set { _xml = value; }
        }
        public XmlNamespaceManager NamespaceManager
        {
            get { return _mgr; }           
        }

        public XmlDriveInfo(string path, PSDriveInfo drive)
            : base(drive)
        {
            _path = path;
            _xml = new XmlDocument();
            _xml.Load(_path);
            _mgr = new XmlNamespaceManager(_xml.NameTable);
            //Console.WriteLine("default namespace = '{0}'", _mgr.DefaultNamespace);
            //Console.WriteLine("XmlDoc namespace URI = '{0}'", _xml.NamespaceURI);            
        }
    }
}
