using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace NugetEfficientTool.Utils
{
    public static class TreeExtensions
    {
        /// <summary>
        /// 查找指定类型/接口的视觉父级。
        /// </summary>
        public static T VisualAncestorByInterface<T>(this DependencyObject source)
            where T : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            Func<DependencyObject, DependencyObject> parentSelector = VisualTreeHelper.GetParent;
            for (DependencyObject d = parentSelector(source); d != null; d = parentSelector(d))
            {
                T r = d as T;
                if (r != null) return r;
            }
            return null;
        }

        /// <summary>
        /// 查找指定类型/接口的视觉父级。
        /// </summary>
        public static T VisualAncestorByInterface<T, TBoundary>(this DependencyObject source)
            where T : class where TBoundary : class
        {
            if (source == null) throw new ArgumentNullException(nameof(source));

            Func<DependencyObject, DependencyObject> parentSelector = VisualTreeHelper.GetParent;
            for (DependencyObject d = parentSelector(source); d != null; d = parentSelector(d))
            {
                TBoundary b = d as TBoundary;
                if (b != null) return null;

                T r = d as T;
                if (r != null) return r;
            }
            return null;
        }

        /// <summary>
        /// 获取<paramref name="element"/>指定类型的子元素
        /// </summary>
        /// <remarks>来源：http://stackoverflow.com/questions/10293236/accessing-the-scrollviewer-of-a-listbox-from-c-sharp </remarks>
        /// <typeparam name="T">子元素的类型</typeparam>
        /// <param name="element">要查找的元素</param>
        /// <returns>第一个指定类型的子元素</returns>
        public static T VisualDescendant<T>(this Visual element) where T : class
        {
            if (element == null)
            {
                return default(T);
            }
            if (element.GetType() == typeof(T))
            {
                return element as T;
            }
            T foundElement = null;
            (element as FrameworkElement)?.ApplyTemplate();
            for (var i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = visual.VisualDescendant<T>();
                if (foundElement != null)
                {
                    break;
                }
            }
            return foundElement;
        }

        /// <summary>
        /// 查找指定类型的所有下层元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="type"></param>
        /// <param name="specificTypeOnly"></param>
        /// <returns></returns>
        private static IEnumerable<Visual> VisualDescendants(Visual element, Type type, bool specificTypeOnly)
        {
            if (element == null)
                return null;

            if (specificTypeOnly
                ? (element.GetType() == type)
                : (element.GetType() == type) || (element.GetType().IsSubclassOf(type)))
                return new List<Visual> { element };

            var foundElements = new List<Visual>();
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                if (visual is null)
                {
                    continue;
                }
                foundElements.AddRange(VisualDescendants(visual, type, specificTypeOnly));
            }

            return foundElements;
        }

        /// <summary>
        /// 查找指定类型的所有下层元素
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="element"></param>
        /// <returns></returns>
        public static IEnumerable<T> VisualDescendants<T>(this Visual element) where T : Visual
        {
            IEnumerable<Visual> temp = VisualDescendants(element, typeof(T), true);
            return temp.Cast<T>();
        }

        /// <summary>
        /// 查找指定Name的下层元素
        /// </summary>
        /// <param name="element"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Visual VisualDescendant(this Visual element, string name)
        {
            if (element == null) return null;
            if ((element is FrameworkElement) && (element as FrameworkElement).Name == name)
                return element;

            Visual foundElement = null;
            if (element is FrameworkElement)
                (element as FrameworkElement).ApplyTemplate();

            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(element); i++)
            {
                var visual = VisualTreeHelper.GetChild(element, i) as Visual;
                foundElement = VisualDescendant(visual, name);
                if (foundElement != null)
                    break;
            }

            return foundElement;
        }
    }
}
