using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.IO;
using System.Xml;

using System.Management.Automation;
using System.Management.Automation.Provider;


namespace Providers
{
    /// <summary>
    /// Provider class for the XmlItemProvider which illustrates how to use the supported *-item cmdlets to access
    /// the underlying XML represented by this provider.
    /// </summary>
    [CmdletProvider("XmlItemProvider", ProviderCapabilities.ShouldProcess)]
    public class XmlItemProvider : ItemCmdletProvider
    {       
        public XmlItemProvider()
            : base()
        {
        }

        //clear-item, get-item, invoke-item, set-item
        // resolve-path, test-path
        #region ItemCmdletProvider overrides
        
        /// <summary>
        /// clear-item cmdlet callback
        /// </summary>
        /// <param name="path"></param>
        protected override void ClearItem(string path)
        {
            WriteVerbose(string.Format("XmlItemProvider::ClearItem(Path = '{0}')",path));

            string npath = XmlProviderUtils.NormalizePath(path);
            string xpath = XmlProviderUtils.PathNoDrive(npath);

            XmlNodeList nodes = GetXmlNodesFromPath(xpath);

            // throw terminating error if we can't find any items at path
            // ------------------------------------------------
            if (nodes == null || nodes.Count == 0)
            {
                ErrorRecord error = new ErrorRecord(new ItemNotFoundException(),
                    "ItemNotFound", ErrorCategory.ObjectNotFound, null);
                ThrowTerminatingError(error);
            }
            
            foreach (XmlNode node in nodes)
            {
                // ShouldProcess() enables use of -whatif & -confirm flags for clear-item
                // If path returns more than a single XMLNode, we call ShouldProcess() for each
                // node not one call to ShouldProcess for the entire operation
                // -----------------------------------------------------------
                if (base.ShouldProcess(node.Name))
                {
                    node.RemoveAll();
                }
            }
        }
        
        protected override object ClearItemDynamicParameters(string path)
        {
            return NameSpaceDynamicParameter();
        }
        
        /// <summary>
        /// get-item cmdlet callback
        /// </summary>
        /// <param name="path"></param>
        protected override void GetItem(string path)
        {
            WriteVerbose(string.Format("XmlItemProvider::GetItem(Path = '{0}')",path));
            string npath = XmlProviderUtils.NormalizePath(path);
            string xpath = XmlProviderUtils.PathNoDrive(npath);

            XmlDriveInfo drive = XmlProviderUtils.GetDriveFromPath(path,base.ProviderInfo);

            if (drive == null)
            {
                ErrorRecord error = new ErrorRecord(new InvalidProgramException("Unable to retrieve the drive for this path"),
                   "drive", ErrorCategory.InvalidData, null);
                WriteError(error);
            }            

            XmlDocument xml = drive.XmlDocument;
            XmlNodeList nodes = xml.SelectNodes(xpath,drive.NamespaceManager);
            
            // ------------------------------
            // NOTE: We could throw an ItemNotFoundException here if the nodelist returned is null
            // or empty. In this case I decided not to because the fact that get-item returns 
            // nothing indicates that case. For other operations such as clear-item or set-item
            // it is a good idea to throw that exception to indicate specifically why you're unable to
            // perform the action.
            // ------------------------------

            foreach(XmlNode node in nodes)
            {
                WriteItemObject(node, path, false);
            }
        }
        
        protected override object GetItemDynamicParameters(string path)
        {
            return NameSpaceDynamicParameter();
        }       

        /// <summary>
        /// test-path cmdlet callback
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override bool ItemExists(string path)
        {
            WriteVerbose(string.Format("XmlItemProvider::ItemExists(Path = '{0}')",path));

            string npath = XmlProviderUtils.NormalizePath(path);
            string xpath = XmlProviderUtils.PathNoDrive(npath);
          
            XmlDriveInfo drive = XmlProviderUtils.GetDriveFromPath(path,base.ProviderInfo);

            if (drive == null)
            {
                return false;
            }
            XmlDocument xml = drive.XmlDocument;
            if (xml.SelectSingleNode(xpath,drive.NamespaceManager) == null)                         
                return false;         
            else
                return true;
        }

        protected override object ItemExistsDynamicParameters(string path)
        {
            return NameSpaceDynamicParameter();
        }
        
        /// <summary>
        /// set-item cmdlet callback
        /// </summary>
        /// <param name="path"></param>
        /// <param name="value"></param>
        protected override void SetItem(string path, object value)
        {            
            WriteVerbose(string.Format("XmlItemProvider::SetItem(Path = '{0}')",path));                        
            
            // This is provider-specific, but I decided to make NULL an error case here. I could have easily have
            // chosen set-item with NULL to mean same as clear-item or make it remove the item but that would
            // be misleading and cause problems down the road if the user wasn't aware they had accidentally passed 
            // a NULL value.
            // ----------------------------------------------------------------------------------------
            if (value == null)
            {
                ErrorRecord error = new ErrorRecord(new InvalidCastException("Value can't be NULL"),
                       "error", ErrorCategory.InvalidData, value);
                ThrowTerminatingError(error);
            }                       

            // check to see if value is PSObject because its very common for a PSObject to be wrapped around 
            // the actual object especially if the item was retrieved via get-item previously
            // ------------------------------------------------------------
            if (value is PSObject)
            {
                value = (value as PSObject).BaseObject;
                WriteVerbose("value is PSObject");
            }

            // Now we make sure the value is of type XmlNode. Note that we could easily allow the value to be a string
            // and set the value of the xml element node to that but I've decided to stick with allowing XmlNodes only. 
            // The XmlNodes can be retrieved via get-item or using the built-in XML support for powershell which makes
            // navigating XML docs the same as calling properties on objects.
            // ---------------------------
            XmlNode newNode = value as XmlNode;            

            // not a valid XmlNode so we can't continue
            // -----------------------------------------
            if (newNode == null)
            {
                ErrorRecord error = new ErrorRecord(new InvalidCastException("Value is not of valid XmlNode Type"),
                    "error", ErrorCategory.InvalidType, value);
                ThrowTerminatingError(error);
            }

            // lets get the item(s) the path indicates
            // ---------------------------------------
            XmlNodeList nodes = GetXmlNodesFromPath(path);

            WriteVerbose(string.Format("XmlItemProvider::SetItem(name = '{0}', value = '{1}')", 
                newNode.Name, newNode.Value));

            // Why am I not checking to make sure an least 1 item was returned? Because ItemExists() is invoked
            // before SetItem() and I wouldn't be in this method if that didn't indicate the path points to at least
            // 1 item and I use the same internal API (XmlNode.SelectNodes()) to retrieve the XmlNodes. I could include
            // some defensive programming though....
            // -------------------------------------
            
            foreach (XmlNode node in nodes)
            {    
                if (node.ParentNode == null)
                {
                    ErrorRecord error = new ErrorRecord(new NotSupportedException("Unable to set root node!"),
                    "error", ErrorCategory.InvalidOperation, value);
                    ThrowTerminatingError(error);
                }
                else
                {
                    if (base.ShouldProcess(node.Name))
                    {
                        node.RemoveAll();

                        foreach (XmlNode nd in newNode.ChildNodes)
                            node.AppendChild(nd.Clone());

                        foreach (XmlAttribute att in newNode.Attributes)
                            node.Attributes.Append(att.Clone() as XmlAttribute);
                    }
                }
            }
        }

        protected override object SetItemDynamicParameters(string path, object value)
        {            
            return NameSpaceDynamicParameter();
        }
           
        /// <summary>
        /// Callback used to verify the syntax of a path. This method is usually invoked before the callback method
        /// for the cmdlet(ie set-item, clear-item)
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override bool IsValidPath(string path)
        {
            WriteVerbose(string.Format("XmlItemProvider::IsValidPath(Path = '{0}')", path));

            // Check if the path is null or empty.
            if (string.IsNullOrEmpty(path))
            {
                return false;
            }

            // Convert all separators in the path to a uniform one.
            path = XmlProviderUtils.NormalizePath(path);

            // Split path
            string[] pathChunks = path.Split(XmlProviderUtils.XML_PATH_SEP.ToCharArray());

            foreach (string pathChunk in pathChunks)
            {
                if (pathChunk.Length == 0)
                    return false;
            }

            return true;
        }

        #endregion

        // new-psdrive, get-psdrive, remove-psdrive
        #region DriveCmdletProvider overrides
              
        protected override PSDriveInfo NewDrive(PSDriveInfo drive)
        {
            WriteVerbose("XmlItemProvider::NewDrive()");
            RuntimeDefinedParameterDictionary dynamicParameters = base.DynamicParameters as RuntimeDefinedParameterDictionary;
            string path = dynamicParameters["path"].Value.ToString();
            return new XmlDriveInfo(path, drive);
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            WriteVerbose("XmlItemProvider::RemoveDrive()");
            XmlDriveInfo xmldrive = drive as XmlDriveInfo;
            xmldrive.XmlDocument.Save(xmldrive.Path);
            xmldrive.XmlDocument = null;
            return base.RemoveDrive(drive);
        }

        protected override object NewDriveDynamicParameters()
        {            
            RuntimeDefinedParameterDictionary dynamicParameters = new RuntimeDefinedParameterDictionary();
            Collection<Attribute> atts = new Collection<Attribute>();
            ParameterAttribute parameterAttribute = new ParameterAttribute();
            parameterAttribute.Mandatory = true;
            atts.Add(parameterAttribute);
            dynamicParameters.Add("path", new RuntimeDefinedParameter("path", typeof(string), atts));

            return dynamicParameters;
        }

        #endregion           

        #region Helpers

        private XmlNodeList GetXmlNodesFromPath(string path)
        {
            string npath = XmlProviderUtils.NormalizePath(path);
            string xpath = XmlProviderUtils.PathNoDrive(npath);

            XmlDriveInfo drive = XmlProviderUtils.GetDriveFromPath(path, base.ProviderInfo);

            if (drive == null)
            {
                ErrorRecord error = new ErrorRecord(new InvalidProgramException("Unable to retrieve the drive for this path"),
                    "drive", ErrorCategory.InvalidData, null);
                WriteError(error);
                return null;
            }
            XmlDocument xml = drive.XmlDocument;
            return xml.SelectNodes(xpath);
        }

        private object NameSpaceDynamicParameter()
        {            
            RuntimeDefinedParameterDictionary dynamicParameters = new RuntimeDefinedParameterDictionary();
            Collection<Attribute> atts = new Collection<Attribute>();
            ParameterAttribute parameterAttribute = new ParameterAttribute();
            atts.Add(parameterAttribute);
            dynamicParameters.Add("namespace", new RuntimeDefinedParameter("namespace", typeof(string), atts));
            return dynamicParameters;      
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
