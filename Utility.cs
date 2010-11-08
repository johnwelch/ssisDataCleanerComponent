using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.SqlServer.Dts.Pipeline.Wrapper;

namespace DataCleansing
{
    internal static class Utility
    {
        public static void AddColumnProperties(IDTSInputColumn100 column)
        {
            AddProperty(column.CustomPropertyCollection, "Operation", "Cleaning operation to perform on the column", CleaningOperation.None, typeof(CleaningOperation).AssemblyQualifiedName, false);
            AddProperty(column.CustomPropertyCollection, "FormatString", "Format string to apply", string.Empty);
            AddProperty(column.CustomPropertyCollection, "MinValue", "Minimum Value for Range Validation", 0, true);
            AddProperty(column.CustomPropertyCollection, "MaxValue", "Maximum Value for Range Validation", 0, true);
            AddProperty(column.CustomPropertyCollection, "ValueList", "Known Good Values", string.Empty);
        }

        public static void RemoveColumnProperties(IDTSInputColumn100 column)
        {
            RemoveProperty(column.CustomPropertyCollection, "Operation");
            RemoveProperty(column.CustomPropertyCollection, "FormatString");
            RemoveProperty(column.CustomPropertyCollection, "MinValue");
            RemoveProperty(column.CustomPropertyCollection, "MaxValue");
            RemoveProperty(column.CustomPropertyCollection, "ValueList");
        }

        public static void RemoveProperty(IDTSCustomPropertyCollection100 propertyCollection, string name)
        {
            int id = -1;
            foreach (IDTSCustomProperty100 existingProperty in propertyCollection)
            {
                if (existingProperty.Name == name)
                {
                    id = existingProperty.ID;
                    break;
                }
            }

            if (id > -1)
            {
                propertyCollection.RemoveObjectByID(id);
            }
        }

        public static void AddProperty(IDTSCustomPropertyCollection100 propertyCollection, string name, string description, object defaultValue)
        {
            AddProperty(propertyCollection, name, description, defaultValue, string.Empty, false);
        }

        public static void AddProperty(IDTSCustomPropertyCollection100 propertyCollection, string name, string description, object defaultValue, bool supportsExpression)
        {
            AddProperty(propertyCollection, name, description, defaultValue, string.Empty, supportsExpression);
        }

        public static void AddProperty(IDTSCustomPropertyCollection100 propertyCollection, string name, string description, object defaultValue, string typeConverter, bool supportsExpression)
        {
            foreach (IDTSCustomProperty100 existingProperty in propertyCollection)
            {
                if (existingProperty.Name == name)
                {
                    return;
                }
            }

            var property = propertyCollection.New();
            property.Name = name;
            property.Description = description;
            property.TypeConverter = typeConverter;
            property.Value = defaultValue;
            if (supportsExpression)
            {
                property.ExpressionType = DTSCustomPropertyExpressionType.CPET_NOTIFY;
            }
        }

        public static T GetPropertyValue<T>(IDTSInputColumn100 column, string name)
        {
            foreach (IDTSCustomProperty100 existingProperty in column.CustomPropertyCollection)
            {
                if (existingProperty.Name == name)
                {
                    try
                    {
                        return (T)existingProperty.Value;
                    }
                    catch (InvalidCastException)
                    {
                        return (T)Convert.ChangeType(existingProperty.Value, typeof(T));
                    }
                    
                }
            }

            throw new DataCleanerException(string.Format("Property {0} doesn't exist in the collection.", name));
        }

        public static void SetPropertyValue(IDTSInputColumn100 column, string name, object value)
        {
            foreach (IDTSCustomProperty100 existingProperty in column.CustomPropertyCollection)
            {
                if (existingProperty.Name == name)
                {
                    existingProperty.Value = value;
                    return;
                }
            }

            throw new DataCleanerException(string.Format("Property {0} doesn't exist in the collection.", name));
        }

        public static bool ContainsFlag(CleaningOperation enumValue, CleaningOperation flag)
        {
            return (enumValue & flag) == flag;
        }

    }
}
