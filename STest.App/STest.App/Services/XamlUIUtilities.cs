using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml;
using System.Linq;
using Microsoft.UI.Xaml.Media.Animation;
using System;
using STest.App.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using STest.App.Utilities;

namespace STest.App.Services
{
    public sealed class XamlUIUtilities : IXamlUIUtilities
    {
        private readonly ILogger<XamlUIUtilities> m_logger;

        public XamlUIUtilities(ILogger<XamlUIUtilities> logger)
        {
            m_logger = logger;
        }

        /// <summary>
        /// Finds an element in a Grid by its row and column position.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public UIElement? FindElementByGridPosition(Grid? grid, int row, int column)
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
        public IOrderedEnumerable<T> FindNeighbors<T>(FrameworkElement? child) where T : FrameworkElement
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
        public T? FindElementByTag<T>(DependencyObject? parent, object tag) where T : FrameworkElement
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
        public T? FindChild<T>(DependencyObject? parent) where T : DependencyObject
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
        /// Retrieves a storyboard resource by its name.  
        /// Throws an exception if the resource is not found or is not a storyboard.  
        /// </summary>  
        /// <param name="name">The name of the storyboard resource.</param>  
        /// <returns>The storyboard resource.</returns>  
        /// <exception cref="InvalidOperationException">Thrown if the storyboard is not found.</exception>  
        /// <exception cref="InvalidCastException">Thrown if the resource is not a storyboard.</exception>  
        public Storyboard GetStoryboard(ResourceDictionary resources, string name)
        {
            ArgumentNullException.ThrowIfNull(resources);
            ArgumentException.ThrowIfNullOrEmpty(name);

            if (resources.TryGetValue(name, out var storyboard))
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
        public void ExecuteAnimation(Storyboard storyboard, UIElement element)
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
                ex.Show(m_logger);
            }
        }
    }
}