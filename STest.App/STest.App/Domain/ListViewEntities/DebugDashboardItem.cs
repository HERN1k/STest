using System.Collections.Generic;

namespace STest.App.Domain.ListViewEntities
{
    public class DebugDashboardItem(string key, string value)
    {
        public string Key { get; private set; } = key ?? string.Empty;
        public string Value { get; private set; } = value ?? string.Empty;
        public KeyValuePair<string, string> Item => new(Key, Value);
    }
}