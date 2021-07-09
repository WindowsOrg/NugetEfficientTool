using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// 解决复杂文本格式化样式的文本框控件
    /// 如"已使用应用软件 {0} 天"，天数需要标红加粗，或者用于【文本】【文字按钮】【文本】的组合
    /// </summary>
    public class ComplexTextBlock : TextBlock
    {

        #region 格式化内容列表

        public static DependencyProperty ContentFormatsProperty =
            DependencyProperty.Register("ContentFormats", typeof(ContentFormatsCollection), typeof(ComplexTextBlock),
                new PropertyMetadata(default(ContentFormatsCollection), ContentFormatsPropertyChanged));

        /// <summary>
        /// 格式化内容列表
        /// </summary>
        public ContentFormatsCollection ContentFormats
        {
            get => (ContentFormatsCollection)GetValue(ContentFormatsProperty);
            set => SetValue(ContentFormatsProperty, value);
        }

        private static void ContentFormatsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoadComplexContent(d);
        }

        #endregion

        #region 文本

        /// <summary>
        /// 重写文本属性
        /// <remarks>延用原有文本属性，在其基础上处理格式化文本</remarks>
        /// </summary>
        public new static DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ComplexTextBlock), new PropertyMetadata(TextPropertyChanged));

        public static string GetText(DependencyObject element)
        {
            return (string)element.GetValue(TextProperty);
        }
        public static void SetText(DependencyObject element, string value)
        {
            element.SetValue(TextProperty, value);
        }

        private static void TextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            LoadComplexContent(d);
        }

        #endregion

        #region 加载复杂文本

        /// <summary>
        /// 格式化字符Key
        /// </summary>
        private const string FormattedKey = "{0}";

        /// <summary>
        /// 加载复杂文本
        /// </summary>
        /// <param name="dependencyObject"></param>
        private static void LoadComplexContent(DependencyObject dependencyObject)
        {
            if (!(dependencyObject is ComplexTextBlock complexTextBlock))
            {
                return;
            }

            string text = GetText(complexTextBlock);
            var contentFormats = complexTextBlock.ContentFormats;

            if (string.IsNullOrEmpty(text) || contentFormats == null || contentFormats.Count == 0)
            {
                //如果当前文本为空或者待格式化列表为空，则显示为当前文本
                complexTextBlock.Text = text;
                return;
            }

            for (int i = 0; i < contentFormats.Count; i++)
            {
                text = text.Replace("{" + $"{i}" + "}", FormattedKey);
            }

            var list =
                GetTextList(text);

            //清空当前文本
            complexTextBlock.Text = null;
            if (contentFormats.All(x => x is TextBlock))
            {
                TurnTextBlockToRun(complexTextBlock, list);
                return;
            }

            //分段加载文本
            var stackPanel = new StackPanel();
            stackPanel.Orientation = Orientation.Horizontal;
            stackPanel.VerticalAlignment = VerticalAlignment.Center;

            int formatIndex = 0;
            foreach (var paraText in list)
            {
                if (paraText == FormattedKey)
                {
                    var uiElement = contentFormats[formatIndex++];
                    if (uiElement is FrameworkElement frameworkElement)
                    {
                        frameworkElement.VerticalAlignment = VerticalAlignment.Center;
                    }
                    stackPanel.Children.Add(uiElement);
                }
                else
                {
                    var textLine = new TextBlock();
                    if (complexTextBlock.Style != null)
                    {
                        textLine.Style = complexTextBlock.Style;
                    }
                    else
                    {
                        textLine.HorizontalAlignment = complexTextBlock.HorizontalAlignment;
                        textLine.Background = complexTextBlock.Background;
                        textLine.FontFamily = complexTextBlock.FontFamily;
                        textLine.FontSize = complexTextBlock.FontSize;
                        textLine.Foreground = complexTextBlock.Foreground;
                        textLine.FontWeight = complexTextBlock.FontWeight;
                        textLine.FontStyle = complexTextBlock.FontStyle;
                    }
                    textLine.VerticalAlignment = VerticalAlignment.Center;
                    textLine.Text = paraText;
                    stackPanel.Children.Add(textLine);
                }
            }
            complexTextBlock.Inlines.Add(stackPanel);
        }

        private static void TurnTextBlockToRun(ComplexTextBlock complexTextBlock, List<string> list)
        {
            int formatIndex = 0;
            foreach (var paraText in list)
            {
                if (paraText == FormattedKey)
                {
                    var uiElement = complexTextBlock.ContentFormats[formatIndex++];
                    if (uiElement is FrameworkElement frameworkElement)
                    {
                        frameworkElement.VerticalAlignment = VerticalAlignment.Center;
                    }

                    var inlineUIContainer = new InlineUIContainer(uiElement);
                    complexTextBlock.Inlines.Add(inlineUIContainer);
                }
                else
                {
                    var run = new Run(paraText)
                    {
                        Background = complexTextBlock.Background,
                        FontFamily = complexTextBlock.FontFamily,
                        FontSize = complexTextBlock.FontSize,
                        Foreground = complexTextBlock.Foreground,
                        FontWeight = complexTextBlock.FontWeight,
                        FontStyle = complexTextBlock.FontStyle
                    };
                    complexTextBlock.Inlines.Add(run);
                }
            }
        }

        /// <summary>
        /// 获取分段文本列表
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static List<string> GetTextList(string text)
        {
            var list = new List<string>();
            var formatIndex = text.IndexOf(FormattedKey, StringComparison.Ordinal);

            //1.不存在格式化关键字，则直接返回当前文本
            if (formatIndex == -1)
            {
                list.Add(text);
                return list;
            }

            //2.存在格式化关键字
            if (formatIndex == 0)
            {
                list.Add(FormattedKey);
            }
            else
            {
                list.Add(text.Substring(0, formatIndex));
                list.Add(FormattedKey);
            }

            //获取下一格式化文本
            if (formatIndex < text.Length)
            {
                list.AddRange(GetTextList(text.Substring(formatIndex + FormattedKey.Length)));
            }

            return list;
        }

        #endregion

    }

    /// <summary>
    /// 格式化内容列表
    /// <remarks>可以添加任意继承了<see cref="UIElement"/>基类的控件，如文本框或者按钮</remarks>
    /// </summary>
    public sealed class ContentFormatsCollection : IList<UIElement>, IList, ICollection, IEnumerable
    {
        private readonly List<UIElement> _headContents = new List<UIElement>();

        public IEnumerator<UIElement> GetEnumerator()
        {
            return _headContents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(UIElement item)
        {
            _headContents.Add(item);
        }

        public int Add(object value)
        {
            _headContents.Add((UIElement)value);
            return _headContents.Count;
        }

        public bool Contains(object value)
        {
            return _headContents.Contains((UIElement)value);
        }

        public void Clear()
        {
            _headContents.Clear();
        }

        public int IndexOf(object value)
        {
            return _headContents.IndexOf((UIElement)value);
        }

        public void Insert(int index, object value)
        {
            _headContents.Insert(index, (UIElement)value);
        }

        public void Remove(object value)
        {
            _headContents.Remove((UIElement)value);
        }

        void IList.RemoveAt(int index)
        {
            _headContents.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get => _headContents[index];
            set => _headContents[index] = (UIElement)value;
        }

        public bool Contains(UIElement item)
        {
            return _headContents.Contains(item);
        }

        public void CopyTo(UIElement[] array, int arrayIndex)
        {
            _headContents.CopyTo(array, arrayIndex);
        }

        public bool Remove(UIElement item)
        {
            return _headContents.Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            _headContents.CopyTo((UIElement[])array, index);
        }

        public int Count => _headContents.Count;

        public object SyncRoot { get; }

        public bool IsSynchronized { get; }

        public bool IsReadOnly { get; }

        public bool IsFixedSize { get; }

        public int IndexOf(UIElement item)
        {
            return _headContents.IndexOf(item);
        }

        public void Insert(int index, UIElement item)
        {
            _headContents.Insert(index, item);
        }

        void IList<UIElement>.RemoveAt(int index)
        {
            _headContents.RemoveAt(index);
        }

        public UIElement this[int index]
        {
            get => _headContents[index];
            set => _headContents[index] = value;
        }
    }
}
