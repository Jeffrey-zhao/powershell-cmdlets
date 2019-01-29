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
    /// Provider class declaration for the XmlNavigationProvider
    /// This provider adds combine-path and movie-item in addition to the cmdlets supported by the
    /// containercmdletprovider base class.
    /// </summary>
    [CmdletProvider("XmlNavigationProvider", ProviderCapabilities.ShouldProcess)]
    public class XmlNavigationProvider : NavigationCmdletProvider
    {
        public XmlNavigationProvider()
            : base()
        {
        }

        // combine-path, move-item
        #region NavigationCmdletProvider overrides

        /// <summary>
        /// Since our path syntax is the same as default(strings delimited by the path separator) we can call
        /// the base.GetChildName() to extract the childname for the path string. 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override string GetChildName(string path)
        {           
            WriteVerbose(string.Format("XmlNavigationProvider::GetChildName(Path = '{0}')",
                path));
            return base.GetChildName(path);
        }

        /// <summary>
        /// Since our path syntax is the same as default(strings delimited by the path separator) we can call
        /// the base.GetParentPath() to extract the parent path from the supplied string. 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="root"></param>
        /// <returns></returns>
        protected override string GetParentPath(string path, string root)
        {            
            WriteVerbose(string.Format("XmlNavigationProvider::GetParentPath(Path = '{0}', root = '{1}')",
                path,root));
            return base.GetParentPath(path,root);
        }
        
        /// <summary>
        /// verifies item specified by path is a container which will allow the usre to navigate to this location as well
        /// as other container related operations.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        protected override bool IsItemContainer(string path)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::IsItemContainer(Path = '{0}')",
                path));

            // see if item exists at path and indicate if it is container 
            // if its a container, we can set-location to it
            string xpath = XmlProviderUtils.NormalizePath(path);

            // Note: We assume that user is setting location to the first XMLNode of possibly more than 
            // one. What we might want to do is provide some syntax that indicates which of multiple nodes of same name
            // to change to. Something like 'drive:\root\node[4]' to indicate the 4th node. This will require more manipulation
            // of the path to make sure we strip it before it does to the xpath query.
            // -----------------------------------------------------------------------
            XmlNode  node  = GetSingleXmlNodeFromPath(xpath);

            if (node == null)
                return false;
            else
                return IsNodeContainer(node);
        }
        
        /// <summary>
        /// combine-path cmdlet callback
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <returns></returns>
        protected override string MakePath(string parent, string child)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::MakePath(parent = '{0}', child = '{1}')",
                parent, child));

            // calling base.MakePath(string,string) simply does a string concatenation inserting the path separator 
            // between the two strings. If the path syntax for your provider is more complex than this, you
            // will need to supply your own code here to create a single path from two path strings.
            // --------------------------------------------------
            return base.MakePath(parent,child);
        }
         
        /// <summary>
        /// move-item cmdlet callback
        /// Basically the same as CopyItem except we remove the nodes from their original location after
        /// they get copied over.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="destination"></param>
        protected override void MoveItem(string path, string destination)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::MoveItem(Path = '{0}', destination = '{1}')",
                path, destination));

            string xpath = XmlProviderUtils.NormalizePath(path);
            XmlNodeList nodes = GetXmlNodesFromPath(xpath);

            XmlNode destNode = GetSingleXmlNodeFromPath(destination);

            XmlDocument xmldoc = GetXmlDocumentFromCurrentDrive();
            if (xmldoc == null)
                return;

            foreach (XmlNode nd in nodes)
            {
                if (base.ShouldProcess(nd.Name))
                {
                    destNode.AppendChild(nd.Clone());

                    // remove node from old location
                    nd.ParentNode.RemoveChild(nd);
                }
            }
        }

        protected override object MoveItemDynamicParameters(string path, string destination)
        {
            return NameSpaceDynamicParameter();
        }

        protected override string NormalizeRelativePath(string path, string basePath)
        {
            Console.WriteLine("XmlNavigationProvider::NormalizeRelativePath(Path = '{0}', basePath = '{1}')",
                path,basePath);
            return base.NormalizeRelativePath(path,basePath);
        }

        #endregion

        // copy-item, get-childitem, new-item, remove-item, rename-item
        #region ContainerCmdletProvider overrides

        protected override void CopyItem(string path, string copyPath, bool recurse)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::CopyItem(Path = '{0}', CopyPath = '{1}', recurse = '{2}')",
                path, copyPath, recurse));

            string xpath = XmlProviderUtils.NormalizePath(path);

            XmlNodeList nodes = GetXmlNodesFromPath(xpath);

            // Now that we have the node(s) to copy we need to figure out where to copy them
            // There are 3 possible scenarios here:
            // 1) there's already an XML node at the place indicated by copyPath
            // 2) There's not a node there but the node indicated by the path minus the childitem name exists
            //    i.e. \root\one\two\three , \root\one\two exists but \root\one\two\three doesn't
            // 3) The path doesn't exist somewhere in the middle, there is no item. 
            //    i.e. \root\one\two\three , \root\one exists but not \root\one\two. which means we couldn't copy
            //         to \root\one\two\three without creating the \two\ item inbetween one & three. This is one of
            //         those internal provider details. The typical behavior here is to create any missing inbetween
            //         nodes if the -force flag is specified.
            XmlNode destNode = GetSingleXmlNodeFromPath(copyPath);

            XmlDocument xmldoc = GetXmlDocumentFromCurrentDrive();
            if (xmldoc == null)
                return;

            foreach (XmlNode nd in nodes)
            {
                if (base.ShouldProcess(nd.Name))
                {
                    /*
                    XmlNode newChildNode = xmldoc.ImportNode(
                        nd,
                        recurse);
                    destNode.AppendChild(newChildNode);
                    
                    // attributes
                    destNode.Attributes.RemoveAll();
                    foreach (XmlAttribute attrib in nd.Attributes)
                    {
                        destNode.Attributes.Append(attrib.Clone());
                    }   
                     */

                    destNode.AppendChild(nd.Clone());
                }
            }
        }

        protected override object CopyItemDynamicParameters(string path, string destination, bool recurse)
        {
            return NameSpaceDynamicParameter();
        }

        protected override void GetChildItems(string path, bool recurse)
        {            
            WriteVerbose(string.Format("XmlNavigationProvider::GetChildItems(Path = '{0}', recurse = '{1}')",
                path, recurse));

            string xpath = XmlProviderUtils.NormalizePath(path);

            // Don't need to remove the drive from the path since container provider infrastructure does that 
            // for me automatically 
            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;
            XmlDocument xml = drive.XmlDocument;
            XmlNodeList nodes = xml.SelectNodes(xpath, drive.NamespaceManager);

            // ------------------------------
            // NOTE: We could create an ErrorRecord and call WriteError() if SelectNodes() returns null.
            // In this case I decided not to because the fact that get-item returns 
            // nothing indicates that case. For other operations such as clear-item or set-item
            // it is a good idea to throw that exception to indicate specifically why you're unable to
            // perform the action.
            // ------------------------------

            foreach (XmlNode node in nodes)
            {
                foreach (XmlNode innerNode in node.ChildNodes)
                {
                    WriteItemObject(innerNode, path, IsNodeContainer(innerNode));
                }
            }
        }

        protected override object GetChildItemsDynamicParameters(string path, bool recurse)
        {
            return NameSpaceDynamicParameter();
        }

        protected override void GetChildNames(string path, ReturnContainers returnContainers)
        {           
            Console.WriteLine("XmlNavigationProvider::GetChildNames(Path = '{0}')", path);
            WriteVerbose(string.Format("XmlNavigationProvider::GetChildNames(Path = '{0}', ReturnContainers = '{1}'",
                path, returnContainers));

            string xpath = XmlProviderUtils.NormalizePath(path);

            XmlNode node = GetSingleXmlNodeFromPath(xpath);

            foreach (XmlNode nd in node.ChildNodes)
            {
                WriteItemObject(nd, path, IsNodeContainer(nd));                
            }
        }       

        protected override object GetChildNamesDynamicParameters(string path)
        {
            return NameSpaceDynamicParameter();
        }

        protected override bool HasChildItems(string path)
        {
            Console.WriteLine("XmlNavigationProvider::HasChildItems(Path = '{0}')", path);
            WriteVerbose(string.Format("XmlNavigationProvider::HasChildItems(Path = '{0}')", path));
            string xpath = XmlProviderUtils.NormalizePath(path);

            XmlNode node = GetSingleXmlNodeFromPath(xpath);

            if (node.HasChildNodes)
                return true;
            else
                return false;
        }

        protected override void NewItem(string path, string itemTypeName, object newItemValue)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::RemoveItemNewItem(Path = '{0}', itemtype = '{1}', newvalue = '{2}')",
                path, itemTypeName, newItemValue));

            // first check if item already exists at that path
            // -----------------------------------------------
            string xpath = XmlProviderUtils.NormalizePath(path);

            // we need to get the parent of the new node so we can add to its children
            // we do this by chopping the last item from the path if there isn't already an item
            // at the path. in which case we need to check force flag or error out
            // for example: new item path = drive:/root/one/two
            // the parent node would be at drive:/root/one
            // --------------------------------------------
            XmlNode parent = null;

            XmlNode destNode = GetSingleXmlNodeFromPath(xpath);

            if (destNode != null)
            {
                parent = destNode.ParentNode;
                if (base.Force)
                    destNode.ParentNode.RemoveChild(destNode);
                else
                {
                    // write error
                    ErrorRecord err = new ErrorRecord(new ArgumentException("item already exists!"), "AlreadyExists",
                        ErrorCategory.InvalidArgument, path);
                    WriteError(err);
                    return;
                }
            }
            else
            {
                parent = GetParentNodeFromLeaf(xpath);
            }

            if (parent == null)
            {
                // write error
                ErrorRecord err = new ErrorRecord(new ItemNotFoundException("ParentPath doesn't exist"), "ObjectNotFound",
                    ErrorCategory.ObjectNotFound, path);
                WriteError(err);
                return;
            }

            string endName = GetLastPathName(xpath);
            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;
            XmlDocument xmldoc = drive.XmlDocument;

            XmlNode newNode = xmldoc.CreateNode(itemTypeName, endName, parent.NamespaceURI);

            // lets call shouldprocess
            if (ShouldProcess(path))
            {
                parent.AppendChild(newNode);
            }
        }

        protected override object NewItemDynamicParameters(string path, string itemTypeName, object newItemValue)
        {
            return NameSpaceDynamicParameter();
        }

        protected override void RemoveItem(string path, bool recurse)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::RemoveItem(Path = '{0}', recurse = '{1}')", path, recurse));
            string xpath = XmlProviderUtils.NormalizePath(path);
            XmlNodeList nodes = GetXmlNodesFromPath(xpath);

            // NOTE: since we remove nodes, the -recurse flag is kind of assumed. Once a node is removed any of 
            // its children are removed also so no need to recurse any further.
            // -------------------------------------------------------------
            foreach (XmlNode node in nodes)
            {
                if (base.ShouldProcess(node.Name))
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
        }

        protected override object RemoveItemDynamicParameters(string path, bool recurse)
        {
            return NameSpaceDynamicParameter();
        }

        protected override void RenameItem(string path, string newName)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::RenameItem(Path = '{0}', newname = '{1}')", path, newName));

            // create new node with new name and then copy childnodes and attributes over
            // --------------------------------------------------------------------------
            string xpath = XmlProviderUtils.NormalizePath(path);
            XmlNodeList nodes = GetXmlNodesFromPath(xpath);

            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;
            XmlDocument xmldoc = drive.XmlDocument;

            foreach (XmlNode nd in nodes)
            {
                if (ShouldProcess(path))
                {
                    XmlNode newNode = xmldoc.CreateNode(nd.NodeType, newName, nd.ParentNode.NamespaceURI);
                    nd.ParentNode.ReplaceChild(newNode, nd);
                }
            }
        }

        protected override object RenameItemDynamicParameters(string path, string newName)
        {
            return NameSpaceDynamicParameter();
        }

        #endregion

        //clear-item, get-item, invoke-item, set-item
        // resolve-path, test-path
        #region ItemCmdletProvider overrides

        protected override void ClearItem(string path)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::ClearItem(Path = '{0}')", path));

            XmlNodeList nodes = GetXmlNodesFromPath(path);

            // write object not found error if we can't find it
            // ------------------------------------------------
            if (nodes == null || nodes.Count == 0)
            {
                ErrorRecord error = new ErrorRecord(new ItemNotFoundException(),
                    "ItemNotFound", ErrorCategory.ObjectNotFound, null);
                WriteError(error);
            }

            foreach (XmlNode node in nodes)
            {
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

        protected override void GetItem(string path)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::GetItem(Path = '{0}')", path));
            string xpath = XmlProviderUtils.NormalizePath(path);

            // Don't need to remove the drive from the path since container provider infrastructure does that 
            // for me automatically 
            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;
            XmlDocument xml = drive.XmlDocument;
            XmlNodeList nodes = xml.SelectNodes(xpath, drive.NamespaceManager);

            // ------------------------------
            // NOTE: We could throw an ItemNotFoundException here if the nodelist returned is null
            // or empty. In this case I decided not to because the fact that get-item returns 
            // nothing indicates that case. For other operations such as clear-item or set-item
            // it is a good idea to throw that exception to indicate specifically why you're unable to
            // perform the action.
            // ------------------------------

            foreach (XmlNode node in nodes)
            {
                WriteItemObject(node, path, false);
            }
        }

        protected override object GetItemDynamicParameters(string path)
        {
            return NameSpaceDynamicParameter();
        }

        protected override bool ItemExists(string path)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::ItemExists(Path = '{0}')", path));

            string xpath = XmlProviderUtils.NormalizePath(path);

            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;
            if (drive == null)
                return false;
            XmlDocument xml = drive.XmlDocument;
            if (xml.SelectSingleNode(xpath, drive.NamespaceManager) == null)
                return false;
            else
                return true;
        }

        protected override object ItemExistsDynamicParameters(string path)
        {
            return NameSpaceDynamicParameter();
        }

        protected override void SetItem(string path, object value)
        {
            WriteVerbose(string.Format("XmlNavigationProvider::SetItem(Path = '{0}')", path));

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

        protected override bool IsValidPath(string path)
        {
            Console.WriteLine("XmlNavigationProvider::IsValidPath(Path = '{0}')", path);
            WriteVerbose(string.Format("XmlNavigationProvider::IsValidPath(Path = '{0}')", path));

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
            WriteVerbose("XmlNavigationProvider::NewDrive()");
            RuntimeDefinedParameterDictionary dynamicParameters = base.DynamicParameters as RuntimeDefinedParameterDictionary;
            string path = dynamicParameters["path"].Value.ToString();
            return new XmlDriveInfo(path, drive);
        }

        protected override PSDriveInfo RemoveDrive(PSDriveInfo drive)
        {
            WriteVerbose("XmlNavigationProvider::RemoveDrive()");
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

        private bool IsNodeContainer(XmlNode xmlNode)
        {
            if ((xmlNode.NodeType == XmlNodeType.Entity) ||
                (xmlNode.NodeType == XmlNodeType.Element) ||
                (xmlNode.NodeType == XmlNodeType.Document))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public XmlNode GetParentNodeFromLeaf(string path)
        {
            // chunk the path and remove the last node name
            throw new NotImplementedException("TODO");
            //return null;
        }

        public string GetLastPathName(string path)
        {
            string[] paths = ChunkPath(path);
            if (paths.Length > 0)
                return paths[paths.Length - 1];
            else
                return null;
        }

        public string[] ChunkPath(string path)
        {
            // NOTE: we're assuming a normalized path so only XML_PATH_SEP's no backslashes
            return path.Split(XmlProviderUtils.XML_PATH_SEP.ToCharArray());
        }

        public XmlDocument GetXmlDocumentFromCurrentDrive()
        {
            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;

            if (drive == null)
            {
                ErrorRecord error = new ErrorRecord(new InvalidProgramException("Unable to retrieve the drive for this path"),
                   "drive", ErrorCategory.InvalidData, null);
                WriteError(error);
                return null;
            }

            XmlDocument xml = drive.XmlDocument;
            return xml;
        }

        public XmlNodeList GetXmlNodesFromPath(string path)
        {
            string npath = XmlProviderUtils.NormalizePath(path);
            string xpath = XmlProviderUtils.PathNoDrive(npath);
            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;
            if (drive == null)
            {
                ErrorRecord error = new ErrorRecord(new InvalidProgramException("Unable to retrieve the drive for this path"),
                    "drive", ErrorCategory.InvalidData, null);
                ThrowTerminatingError(error);
            }
            XmlDocument xml = drive.XmlDocument;
            return xml.SelectNodes(xpath);
        }

        public XmlNode GetSingleXmlNodeFromPath(string path)
        {
            string npath = XmlProviderUtils.NormalizePath(path);
            string xpath = XmlProviderUtils.PathNoDrive(npath);
            XmlDriveInfo drive = base.PSDriveInfo as XmlDriveInfo;
            if (drive == null)
            {
                ErrorRecord error = new ErrorRecord(new InvalidProgramException("Unable to retrieve the drive for this path"),
                    "drive", ErrorCategory.InvalidData, null);
                ThrowTerminatingError(error);
            }
            XmlDocument xml = drive.XmlDocument;
            return xml.SelectSingleNode(xpath);
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
