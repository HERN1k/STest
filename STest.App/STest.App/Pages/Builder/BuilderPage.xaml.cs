using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Versioning;
using System.Text.Json;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using STest.App.Domain.Interfaces;
using STest.App.Utilities;
using STLib.Core.Testing;

namespace STest.App.Pages.Builder
{
    /// <summary>
    /// Builder page
    /// </summary>
    [SupportedOSPlatform("windows10.0.17763.0")]
    public sealed partial class BuilderPage : Page
    {
        public ExtendedObservableCollection<Test> TestsList { get; set; }
        public ExtendedObservableCollection<CoreTask> TasksList { get; set; }

        private readonly ILocalization m_localization;
        private readonly ILocalData m_localData;
        private readonly ITestManager m_testManager;
        private readonly ILogger<BuilderPage> m_logger;
        private readonly Storyboard m_fadeInAnimation;
        private readonly Storyboard m_fadeOutAnimation;
        private Test? m_thisTest;

        public BuilderPage()
        {
            this.InitializeComponent();
            m_localization = ServiceHelper.GetService<ILocalization>();
            m_localData = ServiceHelper.GetService<ILocalData>();
            m_testManager = ServiceHelper.GetService<ITestManager>();
            m_logger = ServiceHelper.GetLogger<BuilderPage>();
            m_fadeInAnimation = GetStoryboard("FadeInAnimation");
            m_fadeOutAnimation = GetStoryboard("FadeOutAnimation");
            TestsList = new ExtendedObservableCollection<Test>();
            TasksList = new ExtendedObservableCollection<CoreTask>();
            this.DataContext = this;
        }

        /// <inheritdoc />
        protected override async void OnNavigatedTo(NavigationEventArgs args)
        {
            try
            {
                base.OnNavigatedTo(args);

                ReopenTest();
                await SetTestListItemsAsync();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <inheritdoc />
        protected override void OnNavigatingFrom(NavigatingCancelEventArgs args)
        {
            try
            {
                base.OnNavigatingFrom(args);
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
        
        /// <summary>
        /// Get the localized string by key
        /// </summary>
        /// <param name="key"></param>
        private string T(string key) => m_localization.T(key);

        /// <summary>
        /// Finds an element in a Grid by its row and column position.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public static UIElement? FindElementByGridPosition(Grid? grid, int row, int column)
        {
            if (grid == null)
            {
                return null;
            }

            foreach (UIElement item in grid.Children)
            {
                if (item is not FrameworkElement element)
                {
                    continue;    
                }

                int elementRow = Grid.GetRow(element);
                int elementColumn = Grid.GetColumn(element);

                if (elementRow == row && elementColumn == column)
                {
                    return element;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds all neighbors of a specified type in the visual tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        public static IOrderedEnumerable<T> FindNeighbors<T>(FrameworkElement? child) where T : FrameworkElement
        {
            if (child == null)
            {
                return Enumerable.Empty<T>().OrderBy(_ => 0);
            }

            if (child.Parent is Panel parent)
            {
                return parent.Children.OfType<T>().OrderBy(parent.Children.IndexOf);
            }
            else
            {
                return Enumerable.Empty<T>().OrderBy(_ => 0);
            }
        }

        /// <summary>
        /// Finds an element in the visual tree by its tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static T? FindElementByTag<T>(DependencyObject? parent, object tag) where T : FrameworkElement
        {
            if (parent == null)
            {
                return null;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T element && Equals(element.Tag, tag))
                {
                    return element;
                }

                var result = FindElementByTag<T>(child, tag);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Finds a child element of a specified type in the visual tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        public static T? FindChild<T>(DependencyObject? parent) where T : DependencyObject
        {
            if (parent == null)
            {
                return null;
            }

            int childCount = VisualTreeHelper.GetChildrenCount(parent);

            for (int i = 0; i < childCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);

                if (child is T target)
                {
                    return target;
                }

                var result = FindChild<T>(child);
                if (result != null)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieves the task from the data context.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dataContext"></param>
        /// <returns></returns>
        public T? GetTaskFromDataContext<T>(object? dataContext) where T : CoreTask
        {
            if (dataContext == null)
            {
                return null;
            }

            if (dataContext is not CoreTask task)
            {
                return null;
            }

            return TasksList.FirstOrDefault(t => t.TaskID.Equals(task.TaskID)) as T;
        }

        /// <summary> 
        /// Sets the items in the test list.  
        /// Displays a message if the list is empty or shows the "Create New Test" button otherwise.  
        /// </summary>  
        private async Task SetTestListItemsAsync()
        {
            try
            {
                var tests = await m_testManager.GetBuilderTestsAsync();
                var now = DateTime.Now;

                TestsList.Clear();
                TestsList.AddRange(tests.OrderBy(t => Math.Abs((t.Created - now).Ticks)));

                if (TestsList.Count == 0)
                {
                    EmptyTestsListButton.Visibility = Visibility.Visible;
                }
                else
                {
                    CreateNewTestButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>
        /// Event handler for the TestBuilderCloseButton click event.
        /// </summary>
        /// <param name="testID"></param>
        private void OpenTestBuilder(Guid testID)
        {
            try
            {
                m_thisTest = TestsList.FirstOrDefault(t => t.TestID == testID)
                    ?? throw new ArgumentNullException(nameof(testID), $"Test not found.");

                ExecuteAnimation(m_fadeInAnimation, TestBuilderBorder);
                TestBuilderBorder.Visibility = Visibility.Visible;

                TestsBuilderName.Text = m_thisTest.Name;
                TestsBuilderDescription.Text = m_thisTest.Description;
                TestsBuilderInstructions.Document.SetText(Microsoft.UI.Text.TextSetOptions.None, m_thisTest.Instructions);
                TestsBuilderTime.Time = m_thisTest.TestTime;
                TestCodeText.Text = m_thisTest.Code;
                TestCodeButton.Tag = m_thisTest.Code;

                TasksList.Clear();
                TasksList.AddRange(m_thisTest.Tasks);

                SetCurrentTest();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }

        /// <summary>  
        /// Reopens the current test if it exists in the memory cache.  
        /// Adds the test to the list if the list is empty and opens the test builder.  
        /// </summary>  
        private void ReopenTest()
        {
            var test = m_testManager.GetCurrentBuilderTest();

            if (test != null)
            {
                if (!TestsList.Contains(test))
                {
                    TestsList.Add(test);
                }

                OpenTestBuilder(test.TestID);
            }
        }

        /// <summary>  
        /// Sets the current test in the memory cache.  
        /// Removes existing tasks and adds updated tasks from the task list.  
        /// </summary>  
        private void SetCurrentTest()
        {
            if (m_thisTest != null)
            {
                m_thisTest.Tasks.Clear();

                foreach (var task in TasksList)
                {
                    m_thisTest.Tasks.Add(task);
                }

                m_testManager.SetCurrentBuilderTest(m_thisTest);
            }
        }

        /// <summary>  
        /// Retrieves a storyboard resource by its name.  
        /// Throws an exception if the resource is not found or is not a storyboard.  
        /// </summary>  
        /// <param name="name">The name of the storyboard resource.</param>  
        /// <returns>The storyboard resource.</returns>  
        /// <exception cref="InvalidOperationException">Thrown if the storyboard is not found.</exception>  
        /// <exception cref="InvalidCastException">Thrown if the resource is not a storyboard.</exception>  
        private Storyboard GetStoryboard(string name)
        {
            if (this.Resources.TryGetValue(name, out var storyboard))
            {
                return storyboard as Storyboard
                    ?? throw new InvalidCastException($"Type \"{name}\" is not storyboard.");
            }
            else
            {
                throw new InvalidOperationException($"Storyboard with name \"{name}\" not found in resources.");
            }
        }

        /// <summary>  
        /// Executes the specified storyboard animation on a UI element.  
        /// Stops the storyboard if it is already running and sets the target element for each animation.  
        /// </summary>  
        /// <param name="storyboard">The storyboard to execute.</param>  
        /// <param name="element">The UI element to animate.</param>  
        private void ExecuteAnimation(Storyboard storyboard, UIElement element)
        {
            if (storyboard == null || element == null)
            {
                return;
            }

            try
            {
                storyboard.Stop();

                for (int i = 0; i < storyboard.Children.Count; i++)
                {
                    Storyboard.SetTarget(storyboard.Children[i], element);
                }

                storyboard.Begin();
            }
            catch (Exception ex)
            {
                ex.Show(this, m_logger);
            }
        }
    }
}