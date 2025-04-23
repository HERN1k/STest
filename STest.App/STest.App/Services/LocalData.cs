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
using System.Collections.Frozen;

namespace STest.App.Services
{
    /// <summary>
    /// Local data storage
    /// </summary>
    public sealed class LocalData : ILocalData
    {
        private readonly ILogger<LocalData> m_logger;
        private readonly string m_path;
        private readonly ConcurrentDictionary<string, string> m_values;
        private readonly byte[] m_zeroData;
        private readonly object m_lock;

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
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

                lock (m_lock)
                {
                    return m_values.TryGetValue(key, out string? value) ? value : string.Empty;
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                throw;
            }
        }

        /// <summary>
        /// Get string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        public FrozenDictionary<string, string> GetStrings(params string[] keys)
        {
            try
            {
                var result = new Dictionary<string, string>();

                lock (m_lock)
                {
                    foreach (string key in keys)
                    {
                        result.Add(key, m_values.TryGetValue(key, out string? value) ? value : string.Empty);
                    }
                }

                return result.ToFrozenDictionary();
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                throw;
            }
        }

        /// <summary>
        /// Get all values
        /// </summary>
        public FrozenDictionary<string, string> GetAll()
        {
            lock (m_lock)
            {
                return m_values.ToFrozenDictionary(k => k.Key, v => v.Value);
            }
        }

        /// <summary>
        /// Set string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="OverflowException"></exception>
        /// <exception cref="IOException"></exception>
        public bool SetString(string key, string value)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));
                ArgumentException.ThrowIfNullOrEmpty(value, nameof(value));

                lock (m_lock)
                {
                    m_values.AddOrUpdate(key, value, (_, _) => value);

                    return UpdateValues();
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                throw;
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
            try
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
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                throw;
            }
        }

        /// <summary>
        /// Remove string value by key
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        public bool RemoveString(string key)
        {
            try
            {
                ArgumentException.ThrowIfNullOrEmpty(key, nameof(key));

                lock (m_lock)
                {
                    m_values.TryRemove(key, out _);

                    return UpdateValues();
                }
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                throw;
            }
        }

        /// <summary>
        /// Remove string values by keys
        /// </summary>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="IOException"></exception>
        public bool RemoveStrings(params string[] keys)
        {
            try
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
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                throw;
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

                    m_logger.LogInformation("The local data storage file is created at the \"{path}\"", m_path);

                    stream.Write(m_zeroData, 0, m_zeroData.Length);

                    stream?.Close();
                }
                catch (Exception ex)
                {
                    m_logger.LogError(ex, "An error occurred while ensuring file availability: {Message}", ex.Message);

                    throw new IOException(ex.Message, ex);
                }
            }
            else
            {
                m_logger.LogInformation("The local data storage file is located at the \"{path}\"", m_path);
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

                    m_logger.LogInformation("The data was updated successfully");

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

                    m_logger.LogInformation("The data was read successfully");
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
        private byte[] BuildZeroData()
        {
            try
            {
                var result = new List<byte>() { 0x7B };

                result.AddRange(Encoding.UTF8.GetBytes(Environment.NewLine));

                result.Add(0x7D);

                return result.ToArray();
            }
            catch (Exception ex)
            {
                m_logger.LogError(ex, "{Message}", ex.Message);

                throw;
            }
        }
    }
}