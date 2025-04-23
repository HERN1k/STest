using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using Microsoft.Extensions.Caching.Memory;
using STLib.Core.Testing;
using STest.App.Pages.Builder;

namespace STest.App.Domain.Interfaces
{
    public interface ITestManager : IService
    {
        /// <summary>
        /// Get the current <see cref="Test"/> in <see cref="BuilderPage"/> from <see cref="ILocalData"/>
        /// </summary>
        Test? GetCurrentBuilderTest();
        /// <summary>
        /// Set the current <see cref="Test"/> in <see cref="BuilderPage"/> to <see cref="ILocalData"/>
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="ArgumentNullException"></exception>
        bool SetCurrentBuilderTest(Test test);
        /// <summary>
        /// Remove the current <see cref="Test"/> in <see cref="BuilderPage"/> from <see cref="ILocalData"/>
        /// </summary>
        bool ClearCurrentBuilderTest();
        /// <summary>
        /// Get the list of <see cref="Test"/> from <see cref="IMemoryCache"/>
        /// </summary>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        Task<IEnumerable<Test>> GetBuilderTestsAsync();
        /// <summary>
        /// Save or update the current <see cref="Test"/> in <see cref="BuilderPage"/> to API
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        Task<IEnumerable<Test>> SaveOrUpdateCurrentBuilderTestAsync();
        /// <summary>
        /// Remove the <see cref="Test"/> from API 
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="ApplicationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        Task<IEnumerable<Test>> RemoveTestAsync(Test test);
        /// <summary>
        /// Remove the list of <see cref="Test"/> from <see cref="IMemoryCache"/>
        /// </summary>
        bool ClearBuilderTests();
        /// <summary>
        /// Deserialize the <see cref="Test"/> from Base64 string
        /// </summary>
        /// <param name="str"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        Test? Deserialize(string str);
        /// <summary>
        /// Serialize the <see cref="Test"/> to Base64 string
        /// </summary>
        /// <param name="test"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="JsonException"></exception>
        /// <exception cref="NotSupportedException"></exception>
        string Serialize(Test test);
    }
}