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
    /// This is a minimalistic sample file system provider whose purpose is to illustrate the optional
    /// provider interfaces IContentCmdletProvider and IPropertyCmdletProvider. This sample only implements the
    /// minimalistic callback methods for the NavigationCmdletProvider base class so that the *-content & *-itemproperty 
    /// cmdlets will work.
    /// </summary>
    [CmdletProvider("SampleFileSystemProvider", ProviderCapabilities.None)]
    public class SampleFileSystemProvider : NavigationCmdletProvider, 
                                    IContentCmdletProvider,
                                    IPropertyCmdletProvider
    {
        // We only support get-item, ItemExists() because the point of this provider is to show how to use the 
        // IContentCmdlet interfaces not the base provider types.
        #region ItemCmdletProvider overrides            

        protected override void GetItem(string path)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::ClearContent(path = '{0}')", path));
            // =================================
            // NOTE: there's a lot of stuff we're not handling here like expanding wildcards or error handling
            // but since purpose of this provider is to show the IProperty & IContent interfaces, thats ok
            // ==================================

            // First check if we have a directory
            // ----------------------------------
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                WriteItemObject(dir, path, true);
                return;
            }

            // now check for files
            // --------------------
            string[] files = Directory.GetFiles(path);   

            foreach (string file in files)
            {
                FileInfo info = new FileInfo(file);
                WriteItemObject(file, path, false);
            }
        }

        protected override bool ItemExists(string path)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::ItemExists(path = '{0}')", path));

            if(File.Exists(path))
                return true;
            if(Directory.Exists(path))
                return true;
            return false;
        }    
   
        protected override bool IsValidPath(string path)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::IsValidPath(Path = '{0}')", path));

            try
            {
                string fullpath = Path.GetFullPath(path);
                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion

        #region IContentCmdletProvider Members

        public void ClearContent(string path)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::ClearContent(path = '{0}')", path));

            // First check if we have a directory, throw terminating error because
            // directories have no content
            // ----------------------------------
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                ErrorRecord error = new ErrorRecord(new InvalidOperationException("Directories have no content!"),
                   "InvalidOperation", ErrorCategory.InvalidOperation, path);
                ThrowTerminatingError(error);
            }

            FileStream fileStream = new FileStream(path, FileMode.Truncate, FileAccess.Write);
            fileStream.Close();           
        }

        public IContentReader GetContentReader(string path)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::GetContentReader(path = '{0}')", path));

            // NOTE: this method is invoked for each path supplied to get-content. Each path though can only result in 
            // a single IContentReader though so each path must resolve to a single item. That means no wildcards. 
            // We don't need to check because File.Exists() will return false in that case
            // ---------------------------------------------------------

            // First check if we have a directory, throw terminating error because
            // directories have no content
            // ----------------------------------
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                ErrorRecord error = new ErrorRecord(new InvalidOperationException("Directories have no content!"),
                    "InvalidOperation", ErrorCategory.InvalidOperation, path);
                ThrowTerminatingError(error);                
            }

            // now check for file
            // ------------------
            if(File.Exists(path))            
                return new FileContentReader(path, this);
            else
                return null;
        }

        public IContentWriter GetContentWriter(string path)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::GetContentWriter(path = '{0}')", path));

            // NOTE: this method is invoked for each path supplied to set-content. Each path though can only result in 
            // a single IContentWriter though so each path must resolve to a single item. That means no wildcards. 
            // We don't need to check because File.Exists() will return false in that case
            // ---------------------------------------------------------

            // First check if we have a directory, throw terminating error because
            // directories have no content
            // ----------------------------------
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                ErrorRecord error = new ErrorRecord(new InvalidOperationException("Directories have no content!"),
                   "InvalidOperation", ErrorCategory.InvalidOperation, path);
                ThrowTerminatingError(error); 
            }

            // now check for file
            // ------------------
            if (File.Exists(path))
                return new FileContentWriter(path, this);
            else
                return null;
        }    
      
        public object ClearContentDynamicParameters(string path)
        {
            return null;
        }

        public object GetContentReaderDynamicParameters(string path)
        {
            return null;
        }

        public object GetContentWriterDynamicParameters(string path)
        {
            return null;
        }

        #endregion

        // Since the file and directory objects have a static set of properties associated with them, we implemented 
        // the IPropertyCmdletProvider interface not IDynamicPropertyCmdletProvider. The innards of the methods are similar
        // except how you determine which properties are valid and which ones aren't
        #region IPropertyCmdletProvider Members

        /// <summary>
        /// What does it mean to "clear" a property for your provider? For filesystem it means set its value to null.
        /// Problem is that most of the properties on the FileSystemInfo object don't allow that. The only property 
        /// that you really want to allow is the "attributes" property which lets you set the read/archive/execute 
        /// settings.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="propertyToClear"></param>
        public void ClearProperty(string path, Collection<string> propertyToClear)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::ClearProperty(path = '{0}')", path));

            // TODO: These are placeholders to indicate things I left out on purpose to save time and highlight the
            // more improtant code but that should be done when writing a production quality provider
            // TODO: Determine if you need to do any path normalization
            // TODO: handle FILE I/O exceptions(access,security)
            // TODO: validate the property names collection. Make sure at least 1 property name is present AND
            //       it is one your item allows to be "cleared"
            
            FileSystemInfo fileinfo = null;         
            
            // First check if we have a directory, 
            // ----------------------------------
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {  
                fileinfo = dir; 
            }

            // now check for file
            // ------------------
            FileInfo file = new FileInfo(path);
            
            if (file.Exists)
            {  
                fileinfo = file;
            }

            if (fileinfo == null)
            {
                ErrorRecord error = new ErrorRecord(new ArgumentException("Item not found"),
                   "ObjectNotFound", ErrorCategory.ObjectNotFound, null);
                WriteError(error);
            }
            else
            {
                PSObject psobj = PSObject.AsPSObject(fileinfo);
                
                // I should validate that there is at least 1 property name in string collection
                if (propertyToClear[0].Equals("attributes", StringComparison.InvariantCultureIgnoreCase))
                {
                    // TODO: If we declared support for ShouldProcess, we should wrap the rest of this code
                    // block in a if(ShouldProcess()){ ... } statement in case user specified -whatif or -confirm
                    // ------------------------------------
                    fileinfo.Attributes = FileAttributes.Normal;

                    // PSObject is basically a collection of name value pairs. IF we successfully clear the
                    // property, we add that to a blank PSObject with the new "clear" value and write it out. This
                    // enables the caller of clear-content to supply the -PassThru parameter which causes the
                    // value to be written to the pipeline. Otherwise, nothing happens with the call to 
                    // writepropertyobject()
                    // ------------------------------
                    PSObject result = new PSObject();
                    result.Properties.Add(new PSNoteProperty(propertyToClear[0], fileinfo.Attributes));

                    // Now write out the proeprty("attribute") that was cleared.
                    WritePropertyObject(result, path);
                }
                else
                {
                    // write error indicating we can't set the property 
                    ErrorRecord error = new ErrorRecord(new ArgumentException("Unable to clear specified property"),
                            "InvalidArgument", ErrorCategory.InvalidArgument, null);
                    WriteError(error);
                }
            }
        }

        public object ClearPropertyDynamicParameters(string path, Collection<string> propertyToClear)
        {
            return null;
        }

        public void GetProperty(string path, Collection<string> providerSpecificPickList)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::GetProperty(path = '{0}')", path));

            // TODO: These are placeholders to indicate things I left out on purpose to save time and highlight the
            // more improtant code but that should be done when writing a production quality provider
            // TODO: handle FILE I/O exceptions(access,security)
            // TODO: Determine if you need to do any path normalization
            // TODO: validate the property names collection. Make sure there is at least 1 value in there. Also
            //       you may want to pre-validate the names and determine if a non-existent property name is
            //       an error or not. I chose to call WriteWarning() if the user specifies a property name that
            //       doesn't exist.

            FileSystemInfo fileinfo = null;

            // First check if we have a directory, 
            // ----------------------------------
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                fileinfo = dir;
            }

            // now check for file
            // ------------------
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                fileinfo = file;
            }

            if (fileinfo == null)
            {
                ErrorRecord error = new ErrorRecord(new ArgumentException("Item not found"),
                   "ObjectNotFound", ErrorCategory.ObjectNotFound, null);
                WriteError(error);
            }
            else
            {
                // By creating a PSObject from the FileSystemInfo object, we can extract the values of its
                // properties by string name. Otherwise we'd a big case statement to call teh different properties
                // based on the user-specified string proeprty name. PSObject is very useful for accessing an object's
                // properties like a hashtable.
                // -------------------------------------------------
                PSObject psobj = PSObject.AsPSObject(fileinfo);
                
                // At this point we have the item and now need to put the values of the specified
                // properties into a blank PSObject and write that to the pipeline.
                PSObject result = new PSObject();
                
                foreach (string name in providerSpecificPickList)
                {
                    // NOTE: For our case, if the property doesn't exist I'm simply giving a null value. Depending
                    // upon your provider, you may want to write an error or write a warning to indicate the 
                    // property doesn't exist. I will call to writewarning.
                    // ----------------------
                    PSPropertyInfo prop = psobj.Properties[name];
                    object value = null;
                    if (prop != null)
                    {
                        value = prop.Value;
                    }
                    else
                    {
                        WriteWarning(string.Format("Property name '{0}' doesn't exist for item at path '{1}'",
                            name, path));
                    }
                    result.Properties.Add(new PSNoteProperty(name, value));
                }

                WritePropertyObject(result, path);
            }
        }

        public object GetPropertyDynamicParameters(string path, Collection<string> providerSpecificPickList)
        {
            return null;
        }

        public void SetProperty(string path, PSObject propertyValue)
        {
            WriteVerbose(string.Format("SampleFileSystemProvider::SetProperty(path = '{0}')", path));

            // TODO: These are placeholders to indicate things I left out on purpose to save time and highlight the
            // more improtant code but that should be done when writing a production quality provider
            // TODO: Determine if you need to do any path normalization
            // TODO: handle FILE I/O exceptions(access,security)
            // TODO: validate the property names collection. Make sure there is at least 1 value in there. Also
            //       you may want to pre-validate the names and determine if a non-existent property name is
            //       an error or not. In this case, I consider a property that doesn't exist for the item to be
            //       an error due to the fact that the IPropertyCmdletPRovider is a STATIC set of properties and
            //       supplying an unknown property name with a value indicates the user did something wrong.
            // NOTE: we only allow the attributes property to be set. The other properties are maintained internally
            //       by the objects and are updated when operations are performed on the item(ie update file changes
            //       LastWriteTime)

            FileSystemInfo fileinfo = null;

            // First check if we have a directory, 
            // ----------------------------------
            DirectoryInfo dir = new DirectoryInfo(path);
            if (dir.Exists)
            {
                fileinfo = dir;
            }

            // now check for file
            // ------------------
            FileInfo file = new FileInfo(path);
            if (file.Exists)
            {
                fileinfo = file;
            }

            if (fileinfo == null)
            {
                ErrorRecord error = new ErrorRecord(new ArgumentException("Item not found"),
                   "ObjectNotFound", ErrorCategory.ObjectNotFound, null);
                WriteError(error);
            }
            else
            {
                // By creating a PSObject from the FileSystemInfo object, we can extract the values of its
                // properties by string name.                 
                // -------------------------------------------------
                PSObject psobj = PSObject.AsPSObject(fileinfo);

                // At this point we have the item and now need to put the values of the specified
                // properties into a blank PSObject and write that to the pipeline.
                PSObject result = new PSObject();

                foreach (PSPropertyInfo valInfo in propertyValue.Properties)
                {
                    // I'm validating the property exists in the item before trying to set it
                    // ---------------------------------------------------------
                    PSPropertyInfo prop = psobj.Properties[valInfo.Name];                    
                    if (prop != null)
                    {
                        string value = valInfo.Value.ToString();

                        // I wish there was a way around this but there doesn't seem to be. We need to
                        // eventually set the values on the item from the data store directly. So we need
                        // a switch statement for all the property names we care about.
                        // I could've included other properties on the FileSystemInfo object but figured 
                        // that two was enough.
                        // ---------------------------------------------
                        switch (valInfo.Name.ToLowerInvariant())
                        {
                            case "attributes":
                                
                                // TODO: catch exceptions this may throw and call writeerror()
                                FileAttributes atts = (FileAttributes)Enum.Parse(typeof(FileAttributes), value);
                                fileinfo.Attributes = atts;
                                result.Properties.Add(new PSNoteProperty(valInfo.Name, atts));
                                break;
                            case "lastwritetime":                                
                                // TODO: handle case where this doesn't work and value is null
                                // TODO: perhaps I should check if value is string also and try to do a DateTime.Parse()
                                // its up to the developer to choose how they want the values to be accepted
                                DateTime time = (DateTime)valInfo.Value;
                                fileinfo.LastWriteTime = time;
                                result.Properties.Add(new PSNoteProperty(valInfo.Name, time));
                                break;
                            default:
                                ErrorRecord error = new ErrorRecord(new ArgumentException("Access denied for updating this property"),
                            "InvalidArgument", ErrorCategory.InvalidArgument, valInfo.Name);
                                WriteError(error);
                                break;
                        }                        
                    }
                    else
                    {
                        ErrorRecord error = new ErrorRecord(new ArgumentException("Property name doesn't exist for item"),
                            "InvalidArgument", ErrorCategory.InvalidArgument, valInfo.Name);
                        WriteError(error);
                    }                    
                }

                WritePropertyObject(result, path);
            }
        }

        public object SetPropertyDynamicParameters(string path, PSObject propertyValue)
        {
            return null;
        }

        #endregion
    }

    /// <summary>
    /// Content writer for setting content of an item
    /// </summary>
    public class FileContentWriter : IContentWriter
    {
        string _path;
        TextWriter _writer;
        CmdletProvider _provider;

        public FileContentWriter(string path, CmdletProvider provider)
        {            
            _path = path;
            _writer = File.CreateText(_path);
            _provider = provider;    
        }

        #region IContentWriter Members

        public void Close()
        {
            _provider.WriteVerbose("Writer.Close()");
            _writer.Close();
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _provider.WriteVerbose(string.Format("Writer.Seek(offset = '{0}',origin = '{1}'", offset, origin));
            throw new NotImplementedException("TODO");
        }

        public System.Collections.IList Write(System.Collections.IList content)
        {
            _provider.WriteVerbose("Writer.Write()");
            foreach (object obj in content)
            {
                _writer.WriteLine("{0}", obj);
            }
            return content;
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {            
            _writer.Dispose();
        }

        #endregion
    }

    /// <summary>
    /// content reader for retrieving the content of an item
    /// </summary>
    public class FileContentReader : IContentReader
    {
        string _path;
        TextReader _reader;
        CmdletProvider _provider;


        public FileContentReader(string path, CmdletProvider provider)
        {           
            _path = path;
            _reader = File.OpenText(_path);
            _provider = provider;
        }

        #region IContentReader Members

        public void Close()
        {
            _provider.WriteVerbose("Reader.Close()");
            _reader.Close();
        }

        public System.Collections.IList Read(long readCount)
        {
            _provider.WriteVerbose(string.Format("Reader.Read(readCount = '{0}'", readCount));
            Collection<string> lines = new Collection<string>();
            
            int i = 0;
            bool toEnd = false;
            if(readCount <= 0)
                toEnd = true;
            string temp =_reader.ReadLine();

            if (temp != null)
                lines.Add(temp);
            
            while(toEnd || (++i < readCount))
            {
                lines.Add(temp);
                temp = _reader.ReadLine();
                if (temp != null)
                    break;
            }

            return lines;
        }

        public void Seek(long offset, SeekOrigin origin)
        {
            _provider.WriteVerbose(string.Format("Reader.Seek(offset = '{0}',origin = '{1}'", offset, origin));
            throw new Exception("The method or operation is not implemented.");
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {            
            _reader.Dispose();
        }

        #endregion
    }
}
