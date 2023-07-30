using System.Collections;
using System.Reflection;

namespace SqliteDna.Integration
{
    internal abstract class BaseFunctionModuleParams
    {
        public abstract void OnCreate(string[]? arguments);
        public abstract string GetSchema();
        public abstract IEnumerable GetData();
        public abstract PropertyInfo[]? GetProperties();
    }

    internal class StaticFunctionModuleParams : BaseFunctionModuleParams
    {
        private Func<string[]?, IEnumerable> func;
        private PropertyInfo[] properties;
        private IEnumerable? data;

        public StaticFunctionModuleParams(Func<string[]?, IEnumerable> func, PropertyInfo[] properties)
        {
            this.func = func;
            this.properties = properties;
        }

        public override void OnCreate(string[]? arguments)
        {
            data = func(arguments);
        }

        public override string GetSchema()
        {
            string schema = "Value";
            if (properties.Length > 0)
            {
                schema = String.Join(",", properties.Select(i => i.Name));
            }

            return schema;
        }

        public override IEnumerable GetData()
        {
            return data!;
        }

        public override PropertyInfo[]? GetProperties()
        {
            return properties;
        }
    }

    internal class DynamicFunctionModuleParams : BaseFunctionModuleParams
    {
        private Func<string[]?, DynamicTable> func;
        private DynamicTable? dynamicTable;

        public DynamicFunctionModuleParams(Func<string[]?, DynamicTable> func)
        {
            this.func = func;
        }

        public override void OnCreate(string[]? arguments)
        {
            dynamicTable = func(arguments);
        }

        public override string GetSchema()
        {
            return dynamicTable!.Schema;
        }

        public override IEnumerable GetData()
        {
            return dynamicTable!.Data;
        }

        public override PropertyInfo[]? GetProperties()
        {
            return null;
        }
    }
}
