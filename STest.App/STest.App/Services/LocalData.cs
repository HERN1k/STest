using System;
using System.IO;
using Windows.Storage;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using System.Collections.Generic;
using System.Text.Json;
using System.Collections.Concurrent;
using System.Text;
using Microsoft.Extensions.Logging;

namespace STest.App.Services
{
    /// <summary>
    /// Local data storage
    /// </summary>
    public sealed class LocalData : ILocalData
    {
        /// <summary>
        /// Logger
        /// </summary>
        private readonly ILogger<LocalData> m_logger;
        /// <summary>
        /// Path to the local data file
        /// </summary>
        private readonly string m_path;
        /// <summary>
        /// Values
        /// </summary>
        private readonly ConcurrentDictionary<string, string> m_values;
        /// <summary>
        /// Zero data
        /// </summary>
        private readonly byte[] m_zeroData;
        /// <summary>
        /// Lock
        /// </summary>
        private readonly object m_lock;

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="logger"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        public LocalData(ILogger<LocalData> logger)
        {
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_path = Path.Combine(ApplicationData.Current.LocalFolder.Path, Constants.APP_LOCAL_DATA_FILE_NAME);
            m_values = new ConcurrentDictionary<string, string>();
            m_zeroData = BuildZeroData();
            m_lock = new object();
            EnsureFileAvailability();
            ReadValues();
        }

        /// <summary>
        /// Get string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public string GetString(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            lock (m_lock)
            {
                return m_values.TryGetValue(key, out string? value) ? value : string.Empty;
            }
        }

        /// <summary>
        /// Get string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public Dictionary<string, string> GetStrings(params string[] keys)
        {
            var result = new Dictionary<string, string>();

            lock (m_lock)
            {
                foreach (string key in keys)
                {
                    result.Add(key, m_values.TryGetValue(key, out string? value) ? value : string.Empty);
                }
            }

            return result;
        }

        /// <summary>
        /// Set string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="IOException"></exception>
        public bool SetString(string key, string value)
        {
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));
            ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

            lock (m_lock)
            {
                m_values.AddOrUpdate(key, value, (_, _) => value);

                return UpdateValues();
            }
        }

        /// <summary>
        /// Set string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="IOException"></exception>
        public bool SetStrings(Dictionary<string, string> data)
        {
            ArgumentNullException.ThrowIfNull(data, nameof(data));

            lock (m_lock)
            {
                foreach (KeyValuePair<string, string> pair in data)
                {
                    m_values.AddOrUpdate(pair.Key, pair.Value, (_, _) => pair.Value);
                }

                return UpdateValues();
            }
        }

        /// <summary>
        /// Remove string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        public bool RemoveString(string key)
        {
            ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

            lock (m_lock)
            {
                m_values.TryRemove(key, out _);

                return UpdateValues();
            }
        }

        /// <summary>
        /// Remove string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        public bool RemoveStrings(params string[] keys)
        {
            ArgumentNullException.ThrowIfNull(keys, nameof(keys));

            lock (m_lock)
            {
                foreach (string key in keys)
                {
                    m_values.TryRemove(key, out _);
                }

                return UpdateValues();
            }
        }

        /// <summary>
        /// Clear all values
        /// </summary>
        /// <exception cref="IOException"></exception>
        public bool Clear()
        {
            lock (m_lock)
            {
                m_values.Clear();

                return UpdateValues();
            }
        }

        /// <summary>
        /// Create recovery data
        /// </summary>
        /// <exception cref="IOException"></exception>
        private byte[] CreateRecovery()
        {
            try
            {
                var recovery = File.ReadAllBytes(m_path);

                if (recovery.Length == 0)
                {
                    recovery = m_zeroData;
                }

                return recovery;
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "An error occurred while creating recovery: {Message}", ex.Message);

                throw new IOException(ex.Message, ex);
            }
        }

        /// <summary>
        /// Ensure file availability
        /// </summary>
        /// <exception cref="IOException"></exception>
        private void EnsureFileAvailability()
        {
            if (!File.Exists(m_path))
            {
                try
                {
                    using var stream = File.Create(m_path);

                    stream.Write(m_zeroData, 0, m_zeroData.Length);

                    stream?.Close();
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, "An error occurred while ensuring file availability: {Message}", ex.Message);

                    throw new IOException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Update values
        /// </summary>
        /// <returns></returns>
        /// <exception cref="IOException"></exception>
        private bool UpdateValues()
        {
            lock (m_lock)
            {
                byte[]? recovery = null;

                try
                {
                    recovery = CreateRecovery();

                    using var stream = File.Open(m_path, FileMode.Create, FileAccess.Write);

                    if (m_values.IsEmpty)
                    {
                        stream.Write(m_zeroData, 0, m_zeroData.Length);
                    }
                    else
                    {
                        var data = JsonSerializer.SerializeToUtf8Bytes(
                            value: m_values,
                            options: new() { WriteIndented = true }
                        );

                        stream.Write(data, 0, data.Length);
                    }

                    stream?.Close();

                    return true;
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, "An error occurred while updating values: {Message}", ex.Message);

                    try
                    {
                        if (recovery != null)
                        {
                            File.WriteAllBytes(m_path, recovery);
                        }
                    }
                    catch (Exception)
                    {
                        if (ex is IOException)
                        {
                            throw;
                        }

                        throw new IOException(ex.Message, ex);
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Read values
        /// </summary>
        /// <exception cref="IOException"></exception>
        private void ReadValues()
        {
            lock (m_lock)
            {
                try
                {
                    m_values.Clear();

                    using JsonDocument document = JsonDocument.Parse(File.ReadAllText(m_path));

                    foreach (JsonProperty property in document.RootElement.EnumerateObject())
                    {
                        m_values.TryAdd(property.Name, property.Value.GetString() ?? string.Empty);
                    }
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, "An error occurred while reading values: {Message}", ex.Message);

                    throw new IOException(ex.Message, ex);
                }
            }
        }

        /// <summary>
        /// Build zero data "{\n}"
        /// </summary>
        /// <returns></returns>
        private static byte[] BuildZeroData()
        {
            var result = new List<byte>() { 0x7B };

            result.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));

            result.Add(0x7D);

            return result.ToArray();
        }
    }
}