using System;
using System.Collections.Generic;
using System.Windows.Forms;

using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace DataCleansing
{
    using System.Collections;
    using System.Runtime.InteropServices;

    using Microsoft.SqlServer.Dts.Runtime;
    using Microsoft.SqlServer.Dts.Runtime.Design;

    public partial class DataCleanerDialog : Form
    {
        ////Variables _vars = null;
        private Dictionary<string, int> _columnInfo = null;

        private bool _populatingList = false;

        private IDTSInputColumn100 _column;

        public DataCleanerDialog()
        {
            InitializeComponent();
        }

        public IServiceProvider ServiceProvider { get; set; }

        public IDTSComponentMetaData100 ComponentMetaData { get; set; }

        public Connections Connections { get; set; }

        private void DataCleanerDialog_Load(object sender, EventArgs e)
        {
            // Populate Connection Manager List
            cboConnectionManager.Items.Clear();
            cboConnectionManager.Items.Add("<New...>");
            foreach (ConnectionManager connMgr in Connections)
            {
                if (connMgr.CreationName.StartsWith("ADO.NET"))
                {
                    cboConnectionManager.Items.Add(connMgr.Name);
                }
            }

            string connMgrID = ComponentMetaData.RuntimeConnectionCollection[DataCleaner.DataCleansingConnection].ConnectionManagerID;
            if (Connections.Contains(connMgrID))
            {
                cboConnectionManager.SelectedItem = Connections[connMgrID].Name;
            }

            chkListInputColumns.Visible = false;
            chkListInputColumns.Items.Clear();
            IDTSVirtualInput100 virtualInput = ComponentMetaData.InputCollection[0].GetVirtualInput();

            _columnInfo = new Dictionary<string, int>(virtualInput.VirtualInputColumnCollection.Count);

            int itemIndex = 0;
            _populatingList = true;
            foreach (IDTSVirtualInputColumn100 virtualColumn in virtualInput.VirtualInputColumnCollection)
            {
                itemIndex = chkListInputColumns.Items.Add(virtualColumn.Name);
                chkListInputColumns.SetItemChecked(itemIndex, virtualColumn.UsageType == DTSUsageType.UT_READWRITE);
                _columnInfo.Add(virtualColumn.Name, virtualColumn.LineageID);
            }

            _populatingList = false;
            chkListInputColumns.Visible = true;
        }

        private void chkListInputColumns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            if (_populatingList)
            {
                return;
            }

            IDTSVirtualInput100 virtualInput = ComponentMetaData.InputCollection[0].GetVirtualInput();

            int selectedItemLineageId = _columnInfo[(string)chkListInputColumns.Items[e.Index]];
            if (e.NewValue == CheckState.Checked)
            {
                virtualInput.SetUsageType(selectedItemLineageId, DTSUsageType.UT_READWRITE);
                var column = ComponentMetaData.InputCollection[0].InputColumnCollection.GetInputColumnByLineageID(selectedItemLineageId);
                Utility.AddColumnProperties(column);
            }
            else
            {
                var column = ComponentMetaData.InputCollection[0].InputColumnCollection.GetInputColumnByLineageID(selectedItemLineageId);
                Utility.RemoveColumnProperties(column);
                virtualInput.SetUsageType(selectedItemLineageId, DTSUsageType.UT_IGNORED);
            }
        }

        private void chkListInputColumns_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_populatingList)
            {
                return;
            }

            if (_column != null)
            {
                this.UpdateProperties(_column);
            }

            IDTSInputColumn100 column = null;
            try
            {
                column = ComponentMetaData.InputCollection[0].InputColumnCollection.GetInputColumnByLineageID(_columnInfo[chkListInputColumns.SelectedItem.ToString()]);
            }
            catch (COMException)
            {
                // Ignore - means that the item is not in the input collection
            }
            
            _column = column;
            if (column == null)
            {
                foreach (int checkedIndex in chkOperations.CheckedIndices)
                {
                    chkOperations.SetItemChecked(checkedIndex, false);
                }

                txtFormatString.Clear();
                txtMinValue.Clear();
                txtMaxValue.Clear();
                txtValidValues.Clear();
                grpColumnProperties.Enabled = false;
            }
            else
            {
                var operation = Utility.GetPropertyValue<CleaningOperation>(column, "Operation");
                for (int index = 0; index < this.chkOperations.Items.Count; index++)
                {
                    var item = (string)this.chkOperations.Items[index];
                    var enumItem = (CleaningOperation)Enum.Parse(typeof(CleaningOperation), item);
                    if (Utility.ContainsFlag(operation, enumItem))
                    {
                        chkOperations.SetItemChecked(index, true);
                    }
                    else
                    {
                        chkOperations.SetItemChecked(index, false);
                    }
                }

                txtFormatString.Text = Utility.GetPropertyValue<string>(column, "FormatString");
                txtMinValue.Text = Utility.GetPropertyValue<string>(column, "MinValue");
                txtMaxValue.Text = Utility.GetPropertyValue<string>(column, "MaxValue");
                txtValidValues.Text = Utility.GetPropertyValue<string>(column, "ValueList");

                grpColumnProperties.Enabled = true;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (_column != null)
            {
                UpdateProperties(_column);
            }
        }

        private void UpdateProperties(IDTSInputColumn100 column)
        {
            CleaningOperation operation = CleaningOperation.None;
            for (int index = 0; index < this.chkOperations.CheckedItems.Count; index++)
            {
                var item = (string)this.chkOperations.CheckedItems[index];
                var enumItem = (CleaningOperation)Enum.Parse(typeof(CleaningOperation), item);
                operation |= enumItem;
            }

            Utility.SetPropertyValue(column, "Operation", operation);
            Utility.SetPropertyValue(column, "FormatString", txtFormatString.Text);
            Utility.SetPropertyValue(column, "MinValue", string.IsNullOrEmpty(txtMinValue.Text) ? 0 : Convert.ToInt32(txtMinValue.Text));
            Utility.SetPropertyValue(column, "MaxValue", string.IsNullOrEmpty(txtMaxValue.Text) ? 0 : Convert.ToInt32(txtMaxValue.Text));
            Utility.SetPropertyValue(column, "ValueList", txtValidValues.Text);
        }

        private void cboConnectionManager_SelectedIndexChanged(object sender, EventArgs e)
        {
            // If it's <New..>, create a connection manager
            if ((string)cboConnectionManager.SelectedItem == "<New...>")
            {
                int currentIndex = -1;
                var connectionService = (IDtsConnectionService)ServiceProvider.GetService(typeof(IDtsConnectionService));
                ArrayList newConnections = connectionService.CreateConnection("ADO.NET");

                foreach (ConnectionManager connMgr in newConnections)
                {
                    currentIndex = cboConnectionManager.Items.Add(connMgr.Name);
                }

                cboConnectionManager.SelectedIndex = currentIndex;
            }

            // Set the runtime connection manager
            if (Connections.Contains(cboConnectionManager.SelectedItem))
            {
                ComponentMetaData.RuntimeConnectionCollection[DataCleaner.DataCleansingConnection].ConnectionManagerID =
                    Connections[cboConnectionManager.SelectedItem].ID;
            }
        
        }
    }
}
