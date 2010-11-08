using Microsoft.SqlServer.Dts.Runtime.Wrapper;

namespace DataCleansing
{
    internal class ColumnInfo
    {
        private readonly string _columnName;
        private readonly DataType _dataType;
        private readonly int _bufferIndex;
        private readonly int _precision;
        private readonly int _scale;
        private readonly int _length;

        public ColumnInfo()
        {
        }

        public ColumnInfo(string columnName, DataType dataType, int bufferIndex, int length, int precision, int scale)
        {
            _columnName = columnName;
            _dataType = dataType;
            _bufferIndex = bufferIndex;
            _precision = precision;
            _scale = scale;
            _length = length;
        }

        public int BufferIndex
        {
            get { return _bufferIndex; }
        }

        public DataType ColumnDataType
        {
            get { return _dataType; }
        }

        public string ColumnName
        {
            get { return _columnName; }
        }

        public int Precision
        {
            get { return _precision; }
        }

        public int Length
        {
            get { return _length; }
        }

        public int Scale
        {
            get { return _scale; }
        }

        public CleaningOperation Operation { get; set; }

        public string FormatString { get; set; }

        public int MinValue { get; set; }

        public int MaxValue { get; set; }

        public string[] ValueList { get; set; }
    }
}
