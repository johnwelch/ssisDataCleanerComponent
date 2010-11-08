using System;
using System.Windows.Forms;

using Microsoft.SqlServer.Dts.Pipeline.Design;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime;

namespace DataCleansing
{
    public class DataCleanerUI : IDtsComponentUI
    {
        private IDTSComponentMetaData100 _cmd = null;
        private IServiceProvider _sp = null;

        public void Initialize(IDTSComponentMetaData100 dtsComponentMetadata, IServiceProvider serviceProvider)
        {
            _cmd = dtsComponentMetadata;
            _sp = serviceProvider;
        }

        public void New(IWin32Window parentWindow)
        {

        }

        public bool Edit(IWin32Window parentWindow, Variables variables, Connections connections)
        {
            var propertiesEditor = new DataCleanerDialog();
            propertiesEditor.Connections = connections;
            propertiesEditor.ComponentMetaData = _cmd;
            propertiesEditor.ServiceProvider = _sp;
            ////propertiesEditor.Variables = variables;
            
            return propertiesEditor.ShowDialog(parentWindow) == DialogResult.OK;
        }

        public void Delete(IWin32Window parentWindow)
        {

        }

        public void Help(IWin32Window parentWindow)
        {

        }
    }
}
