// Save this to a file using filename: PSBook-7-GUIHost.cs
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Globalization;
using System.Management.Automation;
using System.Management.Automation.Host;
using System.Management.Automation.Runspaces;
using System.Windows.Forms;

namespace PSBook.Chapter7
{
    public sealed class GUIPSHost : PSHost
    {
        // private data
        private Guid instanceId;
        private Version version;
        private const string privateData = "gui host private data";
        private PSGUIForm gui;
        private Runspace runspace;

        public GUIPSHost(PSGUIForm form)
            : base()
        {
            gui = form;
            gui.InvokeButton.Click += new EventHandler(InvokeButton_Click);
            instanceId = Guid.NewGuid();
            version = new Version("0.0.0.1");
        }

        public void Initialize()
        {
            runspace = RunspaceFactory.CreateRunspace(this);
            runspace.Open();
        }

        private void InvokeButton_Click(object sender, EventArgs e)
        {
            // disable invoke button to make sure only 1
            // command is running
            gui.InvokeButton.Enabled = false;
            Pipeline pipeline = runspace.CreatePipeline(gui.InputTextBox.Text);
            pipeline.Commands[0].MergeMyResults(PipelineResultTypes.Error, PipelineResultTypes.Output);
            pipeline.Commands.Add("out-string");
            pipeline.Input.Close();
            pipeline.StateChanged +=
                new EventHandler<PipelineStateEventArgs>(pipeline_StateChanged);
            pipeline.InvokeAsync();
        }

        private void pipeline_StateChanged(object sender, PipelineStateEventArgs e)
        {
            Pipeline source = sender as Pipeline;
            // if the command completed update GUI.
            bool updateGUI = false;
            StringBuilder output = new StringBuilder();
            if (e.PipelineStateInfo.State == PipelineState.Completed)
            {
                updateGUI = true;
                Collection<PSObject> results = source.Output.ReadToEnd();
                foreach (PSObject result in results)
                {
                    string resultString = (string)LanguagePrimitives.ConvertTo(result, typeof(string));
                    output.Append(resultString);
                }
            }
            else if ((e.PipelineStateInfo.State == PipelineState.Stopped) ||
                     (e.PipelineStateInfo.State == PipelineState.Failed))
            {
                updateGUI = true;
                string message = string.Format("Command did not complete successfully. Reason: {0}",
                    e.PipelineStateInfo.Reason.Message);
                MessageBox.Show(message);
            }

            if (updateGUI)
            {
                PSGUIForm.SetOutputTextBoxContentDelegate optDelegate =
                    new PSGUIForm.SetOutputTextBoxContentDelegate(gui.SetOutputTextBoxContent);
                gui.OutputTextBox.Invoke(optDelegate, new object[] { output.ToString() });

                PSGUIForm.SetInvokeButtonStateDelegate invkBtnDelegate =
                    new PSGUIForm.SetInvokeButtonStateDelegate(gui.SetInvokeButtonState);
                gui.InvokeButton.Invoke(invkBtnDelegate, new object[] { true });
            }
        }

        public override Guid InstanceId
        {
            get { return instanceId; }
        }

        public override string Name
        {
            get { return "PSBook.Chapter7.Host"; }
        }

        public override Version Version
        {
            get { return version; }
        }

        public override CultureInfo CurrentCulture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        public override CultureInfo CurrentUICulture
        {
            get { return Thread.CurrentThread.CurrentUICulture; }
        }

        public override PSObject PrivateData
        {
            get
            {
                return PSObject.AsPSObject(privateData);
            }
        }

        public override void EnterNestedPrompt()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void ExitNestedPrompt()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void NotifyBeginApplication()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void NotifyEndApplication()
        {
            throw new Exception("The method or operation is not implemented.");
        }

        public override void SetShouldExit(int exitCode)
        {
            string message = string.Format("Exiting with exit code: {0}", exitCode);
            MessageBox.Show(message);
            runspace.CloseAsync();
            Application.Exit();
        }

        public override PSHostUserInterface UI
        {
            get { return null; }
        }


        [STAThread]
        public static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // attach form to the host and start message loop
            // of the form
            PSGUIForm form = new PSGUIForm();
            GUIPSHost host = new GUIPSHost(form);
            host.Initialize();

            Application.Run(form);
        }
    }
}
