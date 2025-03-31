using System;
using System.IO;
using System.Collections.Generic;

namespace STest.App.Domain.Interfaces
{
    /// <summary>
    /// Local data storage
    /// </summary>
    public interface ILocalData : IService
    {
        /// <summary>
        /// Get string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        string GetString(string key);

        /// <summary>
        /// Get string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        Dictionary<string, string> GetStrings(params string[] keys);

        /// <summary>
        /// Set string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="IOException"></exception>
        bool SetString(string key, string value);

        /// <summary>
        /// Set string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="IOException"></exception>
        bool SetStrings(Dictionary<string, string> data);

        /// <summary>
        /// Remove string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        bool RemoveString(string key);

        /// <summary>
        /// Remove string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        bool RemoveStrings(params string[] keys);

        /// <summary>
        /// Clear all values
        /// </summary>
        /// <exception cref="IOException"></exception>
        bool Clear();
    }
}