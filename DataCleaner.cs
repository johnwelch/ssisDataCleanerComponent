using System;
using System.Collections.Generic;
using Microsoft.SqlServer.Dts.Pipeline;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;
using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace DataCleansing
{
    using System.Data.SqlClient;

    [DtsPipelineComponent(ComponentType = ComponentType.Transform, CurrentVersion = 0,
        Description = "Standardizes common data cleansing operations", DisplayName = "Data Cleaner", IconResource = "",
        UITypeName = "DataCleansing.DataCleanerUI, DataCleaner, Version=1.0.0.0, Culture=neutral, PublicKeyToken=6fb9d4add692893d")]
    public class DataCleaner : PipelineComponent
    {
        public const string DataCleansingConnection = "Data Cleansing Connection";

        private readonly string[] _delimiters = { "," };

        private List<ColumnInfo> _columnInfos = null;

        private int _errorOutId;

        private int _outputId;

        private SqlConnection _connection;

        private readonly Dictionary<DataType, object> _defaultValues = new Dictionary<DataType, object>()
            { 
                { DataType.DT_STR, string.Empty },
                { DataType.DT_I4, 0 },
                { DataType.DT_DBTIMESTAMP, new DateTime(1950, 01, 01) }
            };

        #region Design Time

        /// <summary>
        /// Initial method to set up component properties
        /// </summary>
        public override void ProvideComponentProperties()
        {
            this.ComponentMetaData.UsesDispositions = true;

            // Add a single input
            var input = this.ComponentMetaData.InputCollection.New();
            input.Name = "Input";
            input.Description = "Input for the Data Cleaner transformation";
            input.HasSideEffects = false; // Determines if component is left in data flow when run in OptimizedMode and output is not connected
            input.ErrorRowDisposition = DTSRowDisposition.RD_RedirectRow;
            input.ErrorOrTruncationOperation = "ValidationFailure";

            // Add a synchronous output
            var output = this.ComponentMetaData.OutputCollection.New();
            output.Name = "Output";
            output.Description = "Output for the Data Cleaner transformation";
            output.HasSideEffects = false; // Determines if component is left in data flow when run in OptimizedMode and output is not connected
            output.SynchronousInputID = input.ID;
            output.ExclusionGroup = 1;

            // Add an error output
            var errorOutput = this.ComponentMetaData.OutputCollection.New();
            errorOutput.IsErrorOut = true;
            errorOutput.Name = "Error Output";
            errorOutput.Description = "Error Output for the Data Cleaner transformation";
            errorOutput.HasSideEffects = false; // Determines if component is left in data flow when run in OptimizedMode and output is not connected
            errorOutput.SynchronousInputID = input.ID;
            errorOutput.ExclusionGroup = 1;

            // Add a connection
            var conn = this.ComponentMetaData.RuntimeConnectionCollection.New();
            conn.Name = DataCleansingConnection;
            conn.Description = "ADO.NET Connection to SQL Server";
        }

        public override IDTSInput100 InsertInput(DTSInsertPlacement insertPlacement, int inputID)
        {
            throw new InvalidOperationException("The Data Cleaner component can only have a single input.");
        }

        public override IDTSOutput100 InsertOutput(DTSInsertPlacement insertPlacement, int outputID)
        {
            throw new InvalidOperationException("The Data Cleaner component can only have two outputs.");
        }

        public override void DeleteInput(int inputID)
        {
            throw new InvalidOperationException("The Data Cleaner component must have one input");
        }

        public override void DeleteOutput(int outputID)
        {
            throw new InvalidOperationException("The Data Cleaner component must have two outputs.");
        }

        public override IDTSInputColumn100 SetUsageType(int inputID, IDTSVirtualInput100 virtualInput, int lineageID, DTSUsageType usageType)
        {
            var column = base.SetUsageType(inputID, virtualInput, lineageID, usageType);

            if (usageType == DTSUsageType.UT_READWRITE)
            {
                Utility.AddColumnProperties(column);
            }
            else
            {
                Utility.RemoveColumnProperties(column);
            }

            return column;
        }

        public override DTSValidationStatus Validate()
        {
            bool hasError = false;
            bool cancel;
            foreach (IDTSInputColumn100 column in ComponentMetaData.InputCollection[0].InputColumnCollection)
            {
                CleaningOperation operation = CleaningOperation.None;
                if (column.CustomPropertyCollection["Operation"].Value == null)
                {
                    ComponentMetaData.FireError(0, string.Empty, "The Operation property must be set.", string.Empty, 0, out cancel);
                    hasError = true;
                    continue;
                }

                operation = (CleaningOperation)column.CustomPropertyCollection["Operation"].Value;

                if (column.CustomPropertyCollection["FormatString"].Value == null &&
                    Utility.ContainsFlag(operation, CleaningOperation.FormatValue))
                {
                    ComponentMetaData.FireError(0, string.Empty, "The FormatString property must be set.", string.Empty, 0, out cancel);
                    hasError = true;
                }

                if (column.CustomPropertyCollection["MinValue"].Value == null &&
                    Utility.ContainsFlag(operation, CleaningOperation.ValidateRange))
                {
                    ComponentMetaData.FireError(0, string.Empty, "The MinValue property must be set.", string.Empty, 0, out cancel);
                    hasError = true;
                }

                if (column.CustomPropertyCollection["MaxValue"].Value == null &&
                    Utility.ContainsFlag(operation, CleaningOperation.ValidateRange))
                {
                    ComponentMetaData.FireError(0, string.Empty, "The MaxValue property must be set.", string.Empty, 0, out cancel);
                    hasError = true;
                }

                if (column.CustomPropertyCollection["ValueList"].Value == null &&
                    Utility.ContainsFlag(operation, CleaningOperation.ValidateKnownGood))
                {
                    ComponentMetaData.FireError(0, string.Empty, "The ValueList property must be set.", string.Empty, 0, out cancel);
                    hasError = true;
                }

            }

            if (hasError)
            {
                return DTSValidationStatus.VS_ISBROKEN;
            }

            return DTSValidationStatus.VS_ISVALID;
        }

        #endregion

        #region Run Time

        public override void AcquireConnections(object transaction)
        {
            var runtimeConn = this.ComponentMetaData.RuntimeConnectionCollection[DataCleansingConnection];
            if (runtimeConn == null || runtimeConn.ConnectionManager == null)
            {
                bool cancel;
                ComponentMetaData.FireError(0, string.Empty, "The Connection Manager must be set.", string.Empty, 0, out cancel);
            }
            else
            {
                object tempConn = runtimeConn.ConnectionManager.AcquireConnection(transaction);
                if (tempConn is SqlConnection)
                {
                    _connection = (SqlConnection)tempConn;
                    if (_connection.State == System.Data.ConnectionState.Closed)
                    {
                        _connection.Open();
                    }
                }
                else
                {
                    bool cancel;
                    ComponentMetaData.FireError(0, string.Empty, "The BatchDestination can only use ADO.NET SQL Server connections.", string.Empty, 0, out cancel);
                }
            }
        }

        public override void ReleaseConnections()
        {
            if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public override void PreExecute()
        {
            _columnInfos = new List<ColumnInfo>(this.ComponentMetaData.InputCollection[0].InputColumnCollection.Count);

            IDTSInput100 input = this.ComponentMetaData.InputCollection[0];

            // Look at each input, and then each column, storing important metadata.
            foreach (IDTSInputColumn100 col in input.InputColumnCollection)
            {
                // Find the position in buffers that this column will take, and add it to the map.
                var columnInfo = new ColumnInfo(
                    col.Name,
                    col.DataType,
                    this.BufferManager.FindColumnByLineageID(input.Buffer, col.LineageID),
                    col.Length,
                    col.Precision,
                    col.Scale);
                columnInfo.Operation = (CleaningOperation)col.CustomPropertyCollection["Operation"].Value;
                columnInfo.FormatString = col.CustomPropertyCollection["FormatString"].Value.ToString();
                columnInfo.MinValue = (int)col.CustomPropertyCollection["MinValue"].Value;
                columnInfo.MaxValue = (int)col.CustomPropertyCollection["MaxValue"].Value;
                columnInfo.ValueList = col.CustomPropertyCollection["ValueList"].Value.ToString().Split(
                    _delimiters, StringSplitOptions.RemoveEmptyEntries);

                _columnInfos.Add(columnInfo);
            }

            foreach (IDTSOutput100 output in this.ComponentMetaData.OutputCollection)
            {
                if (output.IsErrorOut)
                {
                    _errorOutId = output.ID;
                    break;
                }
                else
                {
                    _outputId = output.ID;
                }
            }
        }

        public override void ProcessInput(int inputID, PipelineBuffer buffer)
        {
            int colIndex = 0;
            while (buffer.NextRow())
            {
                try
                {
                    foreach (ColumnInfo col in _columnInfos)
                    {
                        colIndex = col.BufferIndex;
                        if (Utility.ContainsFlag(col.Operation, CleaningOperation.SetNullDefault))
                        {
                            ReplaceNullValueWithDefault(buffer, col);
                        }

                        if (Utility.ContainsFlag(col.Operation, CleaningOperation.TrimString))
                        {
                            TrimString(buffer, col);
                        }

                        if (Utility.ContainsFlag(col.Operation, CleaningOperation.FormatValue))
                        {
                            FormatValue(buffer, col);
                        }

                        if (Utility.ContainsFlag(col.Operation, CleaningOperation.ValidateRange))
                        {
                            ValidateRange(buffer, col);
                        }

                        if (Utility.ContainsFlag(col.Operation, CleaningOperation.ValidateKnownGood))
                        {
                            ValidateKnownGoodValue(buffer, col);
                        }
                    }

                    buffer.DirectRow(_outputId);
                }
                catch (Exception)
                {
                    buffer.DirectErrorRow(_errorOutId, 100, colIndex);
                }

            }
        }

        #endregion

        #region Data Cleansing Functions

        private void ReplaceNullValueWithDefault(PipelineBuffer buffer, ColumnInfo col)
        {
            if (buffer.IsNull(col.BufferIndex))
            {
                buffer[col.BufferIndex] = _defaultValues[col.ColumnDataType];
            }
        }

        private void TrimString(PipelineBuffer buffer, ColumnInfo col)
        {
            if (col.ColumnDataType == DataType.DT_STR || col.ColumnDataType == DataType.DT_WSTR)
            {
                buffer[col.BufferIndex] = buffer.GetString(col.BufferIndex).Trim();
            }
        }

        private void FormatValue(PipelineBuffer buffer, ColumnInfo col)
        {
            try
            {
                buffer[col.BufferIndex] = string.Format(col.FormatString, Convert.ToDouble(buffer.GetString(col.BufferIndex)));
            }
            catch (OverflowException)
            {
                throw new FormatException("Value is not in a valid format");
            }
            catch (FormatException)
            {
                throw new FormatException("Value is not in a valid format");
            }
        }

        private void ValidateRange(PipelineBuffer buffer, ColumnInfo col)
        {
            int value = buffer.GetInt32(col.BufferIndex);
            if (value < col.MinValue || value > col.MaxValue)
            {
                throw new ArgumentException("Column Value out of range", "buffer");
            }
        }

        private void ValidateKnownGoodValue(PipelineBuffer buffer, ColumnInfo col)
        {
            string value = buffer.GetString(col.BufferIndex);
            bool valid = false;
            foreach (var validValue in col.ValueList)
            {
                if (validValue == value)
                {
                    valid = true;
                    break;
                }
            }

            if (!valid)
            {
                throw new ArgumentException("Column Value not in valid list", "buffer");
            }
        }

        #endregion

        #region Helpers

        private static object GetBufferColumnValue(PipelineBuffer buffer, ColumnInfo col)
        {
            if (buffer.IsNull(col.BufferIndex))
            {
                return null;
            }

            switch (col.ColumnDataType)
            {
                case DataType.DT_BOOL:
                    return buffer.GetBoolean(col.BufferIndex);
                case DataType.DT_BYTES:
                    return buffer.GetBytes(col.BufferIndex);
                case DataType.DT_CY:
                    return buffer.GetDecimal(col.BufferIndex);
                case DataType.DT_DATE:
                    return buffer.GetDateTime(col.BufferIndex);
                case DataType.DT_DBDATE:
                    return buffer.GetDate(col.BufferIndex);
                case DataType.DT_DBTIME:
                    return buffer.GetTime(col.BufferIndex);
                case DataType.DT_DBTIME2:
                    return buffer.GetTime(col.BufferIndex);
                case DataType.DT_DBTIMESTAMP:
                    return buffer.GetDateTime(col.BufferIndex);
                case DataType.DT_DBTIMESTAMP2:
                    return buffer.GetDateTime(col.BufferIndex);
                case DataType.DT_DBTIMESTAMPOFFSET:
                    return buffer.GetDateTimeOffset(col.BufferIndex);
                case DataType.DT_DECIMAL:
                    return buffer.GetDecimal(col.BufferIndex);
                case DataType.DT_FILETIME:
                    return buffer.GetDateTime(col.BufferIndex);
                case DataType.DT_GUID:
                    return buffer.GetGuid(col.BufferIndex);
                case DataType.DT_I1:
                    return buffer.GetSByte(col.BufferIndex);
                case DataType.DT_I2:
                    return buffer.GetInt16(col.BufferIndex);
                case DataType.DT_I4:
                    return buffer.GetInt32(col.BufferIndex);
                case DataType.DT_I8:
                    return buffer.GetInt64(col.BufferIndex);
                case DataType.DT_IMAGE:
                    return buffer.GetBlobData(col.BufferIndex, 0, (int)buffer.GetBlobLength(col.BufferIndex));
                case DataType.DT_NTEXT:
                    return buffer.GetBlobData(col.BufferIndex, 0, (int)buffer.GetBlobLength(col.BufferIndex));
                case DataType.DT_NUMERIC:
                    return buffer.GetDecimal(col.BufferIndex);
                case DataType.DT_R4:
                    return buffer.GetSingle(col.BufferIndex);
                case DataType.DT_R8:
                    return buffer.GetDouble(col.BufferIndex);
                case DataType.DT_STR:
                    return buffer.GetString(col.BufferIndex);
                case DataType.DT_TEXT:
                    return buffer.GetBlobData(col.BufferIndex, 0, (int)buffer.GetBlobLength(col.BufferIndex));
                case DataType.DT_UI1:
                    return buffer.GetByte(col.BufferIndex);
                case DataType.DT_UI2:
                    return buffer.GetUInt16(col.BufferIndex);
                case DataType.DT_UI4:
                    return buffer.GetUInt32(col.BufferIndex);
                case DataType.DT_UI8:
                    return buffer.GetUInt64(col.BufferIndex);
                case DataType.DT_WSTR:
                    return buffer.GetString(col.BufferIndex);
                default:
                    return null;
            }
        }

        #endregion
    }

    [Flags]
    public enum CleaningOperation
    {
        None = 0x0,
        SetNullDefault = 0x1,
        TrimString = 0x2,
        FormatValue = 0x4,
        ValidateRange = 0x8,
        ValidateKnownGood = 0x10
    }
}
