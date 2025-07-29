using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using STLib.Core.Testing;
using STest.App.Pages.Builder;
using System.Linq;

namespace STest.App.Services
{
    public sealed class TestManager: ITestManager
    {
        private readonly ILocalData m_localData;
        private readonly IMemoryCache m_memoryCache;
        private readonly ILogger<TestManager> m_logger;
        private readonly Guid m_userID;

        public TestManager(ILocalData localData, IMemoryCache memoryCache, ILogger<TestManager> logger)
        {
            m_localData = localData ?? throw new ArgumentNullException(nameof(localData));
            m_memoryCache = memoryCache ?? throw new ArgumentNullException(nameof(memoryCache));
            m_logger = logger ?? throw new ArgumentNullException(nameof(logger));
            m_userID = GetUserID();
        }

        /// <summary>
        /// Get the current <see cref="Test"/> in <see cref="BuilderPage"/> from <see cref="ILocalData"/>
        /// </summary>
        public Test? GetCurrentBuilderTest()
        {
            return m_memoryCache.Get<Test>(Constants.CURRENT_TEST_IN_BUILDER);
        }

        /// <summary>
        /// Set the current <see cref="Test"/> in <see cref="BuilderPage"/> to <see cref="ILocalData"/>
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public bool SetCurrentBuilderTest(Test test)
        {
            ArgumentNullException.ThrowIfNull(test);

            m_memoryCache.Set<Test>(Constants.CURRENT_TEST_IN_BUILDER, test, TimeSpan.FromMinutes(30));

            return true;
        }

        /// <summary>
        /// Remove the current <see cref="Test"/> in <see cref="BuilderPage"/> from <see cref="ILocalData"/>
        /// </summary>
        public bool ClearCurrentBuilderTest()
        {
            m_memoryCache.Remove(Constants.CURRENT_TEST_IN_BUILDER);

            return true;
        }

        /// <summary>
        /// Get the list of <see cref="Test"/> from <see cref="IMemoryCache"/>
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
#pragma warning disable CS1998
        public async Task<IEnumerable<Test>> GetBuilderTestsAsync()
#pragma warning restore CS1998
        {
            List<Test>? tests = m_memoryCache.Get<List<Test>>(Constants.BUILDER_TESTS_LOCAL_DATA);

            if (tests == null)
            {
#if DEBUG
                tests = GetBuilderTestsList();
#elif RELEASE
                // TODO: Get tests from server
#endif
            }

            m_memoryCache.Set<List<Test>>(Constants.BUILDER_TESTS_LOCAL_DATA, tests, TimeSpan.FromHours(1));

            return tests;
        }

        /// <summary>
        /// Save or update the current <see cref="Test"/> in <see cref="BuilderPage"/> to API
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
#pragma warning disable CS1998
        public async Task<IEnumerable<Test>> SaveOrUpdateCurrentBuilderTestAsync()
#pragma warning restore CS1998
        {
            List<Test>? tests = null;
            Test test = GetCurrentBuilderTest() ?? throw new ApplicationException($"Test not found.");

            test.OnDevSave();
#if DEBUG
            tests = (await GetBuilderTestsAsync()).ToList();

            tests.RemoveAll(t => t.TestID.Equals(test.TestID));
            tests.Add(test);

            List<string> serializedTestsList = new List<string>();
            foreach (var t in tests)
            {
                serializedTestsList.Add(Serialize(t));
            }

            m_localData.SetString(Constants.DEV_LIST_TEST_LOCAL_DATA, JsonSerializer.Serialize(serializedTestsList));
#elif RELEASE
            // TODO: Send tests to server
#endif
            ClearBuilderTests();

            return tests;
        }

        /// <summary>
        /// Remove the <see cref="Test"/> from API 
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
#pragma warning disable CS1998
        public async Task<IEnumerable<Test>> RemoveTestAsync(Test test)
#pragma warning restore CS1998
        {
            ArgumentNullException.ThrowIfNull(test);

            List<Test>? tests = null;

            test.OnDevSave();
#if DEBUG
            tests = (await GetBuilderTestsAsync()).ToList();

            tests.RemoveAll(t => t.TestID.Equals(test.TestID));

            List<string> serializedTestsList = new List<string>();
            foreach (var t in tests)
            {
                serializedTestsList.Add(Serialize(t));
            }

            m_localData.SetString(Constants.DEV_LIST_TEST_LOCAL_DATA, JsonSerializer.Serialize(serializedTestsList));
#elif RELEASE
            // TODO: Send tests to server
#endif
            ClearBuilderTests();

            return tests;
        }

        /// <summary>
        /// Remove the list of <see cref="Test"/> from <see cref="IMemoryCache"/>
        /// </summary>
        public bool ClearBuilderTests()
        {
            m_memoryCache.Remove(Constants.BUILDER_TESTS_LOCAL_DATA);

            return true;
        }

        /// <summary>
        /// Serialize the <see cref="Test"/> to Base64 string
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
#pragma warning disable CA1822
        public string Serialize(Test test)
#pragma warning restore CA1822
        {
            ArgumentNullException.ThrowIfNull(test);

            return Convert.ToBase64String(test.SerializeToByteArray());
        }

        /// <summary>
        /// Deserialize the <see cref="Test"/> from Base64 string
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
#pragma warning disable CA1822
        public Test? Deserialize(string str)
#pragma warning restore CA1822
        {
            ArgumentException.ThrowIfNullOrEmpty(str);

            return Test.DeserializeFromByteArray(Convert.FromBase64String(str));
        }

        /// <summary>
        /// Get the user ID from local data
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        private Guid GetUserID()
        {
            if (!Guid.TryParse(m_localData.GetString(Constants.USER_ID_LOCAL_DATA), out Guid userID))
            {
                throw new ApplicationException($"User ID not found.");
            }

            if (userID.Equals(Guid.Empty))
            {
                throw new ApplicationException($"User ID is empty.");
            }

            return userID;
        }
#if DEBUG
        private List<Test> GetBuilderTestsList()
        {
            List<Test> tests = new List<Test>();
            
            string testsListString = m_localData.GetString(Constants.DEV_LIST_TEST_LOCAL_DATA) ?? "[]";

            List<string> testsList = JsonSerializer.Deserialize<List<string>>(testsListString)
                ?? throw new ApplicationException("Dev tests list not found.");

            foreach (var testString in testsList)
            {
                var test = Deserialize(testString);

                if (test != null)
                {
                    tests.Add(test);
                }
            }

            return tests;
        }
#endif
    }
}