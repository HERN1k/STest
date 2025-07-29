using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml;
using System.Linq;
using Microsoft.UI.Xaml.Media.Animation;

namespace STest.App.Domain.Interfaces
{
    public interface IXamlUIUtilities : IService
    {
        /// <summary>
        /// Finds an element in a Grid by its row and column position.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="row"></param>
        /// <param name="column"></param>
        public UIElement? FindElementByGridPosition(Grid? grid, int row, int column);
        /// <summary>
        /// Finds all neighbors of a specified type in the visual tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="child"></param>
        public IOrderedEnumerable<T> FindNeighbors<T>(FrameworkElement? child) where T : FrameworkElement;
        /// <summary>
        /// Finds an element in the visual tree by its tag.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        /// <param name="tag"></param>
        /// <returns></returns>
        public T? FindElementByTag<T>(DependencyObject? parent, object tag) where T : FrameworkElement;
        /// <summary>
        /// Finds a child element of a specified type in the visual tree.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="parent"></param>
        public T? FindChild<T>(DependencyObject? parent) where T : DependencyObject;
        /// <summary>  
        /// Retrieves a storyboard resource by its name.  
        /// Throws an exception if the resource is not found or is not a storyboard.  
        /// </summary>  
        /// <param name="name">The name of the storyboard resource.</param>  
        /// <returns>The storyboard resource.</returns>  
        /// <exception cref="InvalidOperationException">Thrown if the storyboard is not found.</exception>  
        /// <exception cref="InvalidCastException">Thrown if the resource is not a storyboard.</exception>  
        public Storyboard GetStoryboard(ResourceDictionary resources, string name);
        /// <summary>  
        /// Executes the specified storyboard animation on a UI element.  
        /// Stops the storyboard if it is already running and sets the target element for each animation.  
        /// </summary>  
        /// <param name="storyboard">The storyboard to execute.</param>  
        /// <param name="element">The UI element to animate.</param>  
        public void ExecuteAnimation(Storyboard storyboard, UIElement element);
    }
}