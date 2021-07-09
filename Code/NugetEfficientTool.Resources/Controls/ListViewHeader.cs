using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using NugetEfficientTool.Utils;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// 为 <see cref="ListBox"/>、<see cref="ListView"/> 等控件提供表头。
    /// 表头包含排序字段和排序指示功能。
    /// </summary>
    public class ListViewHeader : Control
    {
        #region 构造函数和初始化

        static ListViewHeader()
        {
            // 覆盖基类的默认样式，重新提供一个新的默认样式。在 /Themes/ListViewHeader.xaml 文件中。
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ListViewHeader), new FrameworkPropertyMetadata(typeof(ListViewHeader)));
        }

        public override void OnApplyTemplate()
        {
            MainGrid = (Grid)Template.FindName(nameof(MainGrid), this);
            InitHeaderList();
        }
        /// <summary>
        /// 如果试图在模板应用之前获取此值，则为 null。
        /// 请注意：如果尚未显示之前，或者刚显示时处于 Collapse 的状态，则会保持此值为 null。
        /// </summary>
        [CanBeNull, SuppressMessage("ReSharper", "InconsistentNaming")]
        private Grid MainGrid;

        #endregion

        #region 移植的逻辑

        private void InitHeaderList()
        {
            if (MainGrid != null && MainGrid.Children.Count == 0 && Headers != null)
            {
                SetGridDefinitions(Headers);

                int headerIndex = 0;
                foreach (var header in Headers)
                {
                    var headerButton = new Button()
                    {
                        Content = header.Content,
                        HorizontalContentAlignment = header.HorizontalAlignment,
                        VerticalAlignment = VerticalAlignment.Center,
                        Padding = header.Padding,
                        Style = SortingButtonStyle,
                    };
                    MainGrid.Children.Add(headerButton);

                    headerButton.SetValue(SortHelper.SortFieldProperty, header.SortName);
                    if (!string.IsNullOrEmpty(header.SortName))
                    {
                        headerButton.Cursor = Cursors.Hand;
                    }

                    headerButton.SetValue(Grid.ColumnProperty, headerIndex++);
                    headerButton.Click += HeaderButton_Click;
                }

                //初始化完成后，排序
                SetSortingFiled(SortOption);
            }
        }

        private Button _lastHeaderButton;
        public event EventHandler<ListSortOption> SortButtonClicked;

        private void HeaderButton_Click(object sender, RoutedEventArgs e)
        {
            var headerButton = sender as Button;
            var sortingField = (string)headerButton?.GetValue(SortHelper.SortFieldProperty);
            if (!string.IsNullOrEmpty(sortingField))
            {
                var currentDirection = ((ListViewSortDirection?)headerButton.GetValue(SortHelper.SortDirectionProperty)) ?? ListViewSortDirection.Default;
                var direction = currentDirection == ListViewSortDirection.Ascending ? ListSortDirection.Descending : ListSortDirection.Ascending;
                SortOption = new ListSortOption() { SortField = sortingField, SortDirection = direction };
                SortButtonClicked?.Invoke(this, SortOption);
            }
        }

        /// <summary>
        /// 设置Grid行列
        /// </summary>
        private void SetGridDefinitions(IEnumerable<HeaderContent> headers)
        {
            foreach (var header in headers)
            {
                MainGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = header.Width,MinWidth = header.MinWidth});
            }
        }

        /// <summary>
        /// 课件排序
        /// </summary>
        /// <param name="sortOption"></param>
        private void SetSortingFiled(ListSortOption sortOption)
        {
            //当数据全部绑定时，且MainGrid不为空，才触发排序
            if (sortOption == null || Headers == null || MainGrid == null)
            {
                return;
            }

            if (string.IsNullOrEmpty(sortOption.SortField))
            {
                Debug.WriteLine(new InvalidOperationException($"【列表排序控件】{nameof(SortOption)}.{nameof(sortOption.SortField)} 排序字段未初始化"));
                return;
            }

            var headerButton = MainGrid.Children.OfType<Button>()
                .FirstOrDefault(button => (string)button.GetValue(SortHelper.SortFieldProperty) == sortOption.SortField);
            if (headerButton == null)
            {
                Debug.WriteLine(new InvalidOperationException($"【列表排序控件】列表中未找到{nameof(SortOption)}.{nameof(sortOption.SortField)}={sortOption.SortField}对应的名称！"));
                return;
            }

            //设置上一个标题列的显示状态
            if (_lastHeaderButton != null && !Equals(headerButton, _lastHeaderButton))
            {
                _lastHeaderButton.SetValue(SortHelper.SortDirectionProperty, ListViewSortDirection.Default);
            }

            //设置排序
            headerButton.SetValue(SortHelper.SortDirectionProperty,
                sortOption.SortDirection == ListSortDirection.Ascending ? ListViewSortDirection.Ascending : ListViewSortDirection.Descending);
            SortItems(sortOption.SortField, sortOption.SortDirection);

            _lastHeaderButton = headerButton;
        }

        private void SortItems(string sortingField, ListSortDirection sortingDirection)
        {
            if (SortTarget is ListBox listBox)
            {
                listBox.Items.SortDescriptions.Clear();
                listBox.Items.SortDescriptions.Add(new SortDescription(sortingField, sortingDirection));
                listBox.Items.Refresh();
            }
            OnSortChanged(sortingField, sortingDirection);
        }

        #endregion

        #region 依赖属性

        /// <summary>
        /// 标识 <see cref="SortingButtonStyle"/> 的依赖项属性。
        /// </summary>
        public static readonly DependencyProperty SortingButtonStyleProperty = DependencyProperty.Register(
            "SortingButtonStyle", typeof(Style), typeof(ListViewHeader), new PropertyMetadata(null));

        /// <summary>
        /// 获取或设置排序按钮的样式。
        /// </summary>
        [CanBeNull]
        public Style SortingButtonStyle
        {
            get => (Style)GetValue(SortingButtonStyleProperty);
            set => SetValue(SortingButtonStyleProperty, value);
        }

        private ListViewHeaderContentCollection _headers = null;
        /// <summary>
        /// 头部列表
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ListViewHeaderContentCollection Headers
        {
            get
            {
                if (this._headers == null)
                    this._headers = new ListViewHeaderContentCollection();
                return _headers;
            }
        }

        public static readonly DependencyProperty SortOptionProperty = DependencyProperty.Register(
            "SortOption", typeof(ListSortOption), typeof(ListViewHeader),
            new PropertyMetadata(default(ListSortOption), (d, e) => ((ListViewHeader)d).SetSortingFiled((ListSortOption)e.NewValue)));

        /// <summary>
        /// 排序选项
        /// <para>由<see cref="ListViewHeader"/>提供，可设置排序选项</para>
        /// </summary>
        public ListSortOption SortOption
        {
            get => (ListSortOption)GetValue(SortOptionProperty);
            set => SetValue(SortOptionProperty, value);
        }

        public static readonly DependencyProperty SortTargetProperty = DependencyProperty.Register("SortTarget", typeof(DependencyObject), typeof(ListViewHeader),
            new PropertyMetadata(default(DependencyObject)));

        public DependencyObject SortTarget
        {
            get { return (DependencyObject)GetValue(SortTargetProperty); }
            set { SetValue(SortTargetProperty, value); }
        }

        #endregion

        #region 路由事件

        /// <summary>
        /// 标识 <see cref="SortChanged"/> 的路由事件。
        /// </summary>
        public static readonly RoutedEvent SortChangedEvent = EventManager.RegisterRoutedEvent(
            "SortChanged", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(ListViewHeader));


        public event RoutedEventHandler SortChanged
        {
            add => AddHandler(SortChangedEvent, value);
            remove => RemoveHandler(SortChangedEvent, value);
        }

        protected virtual void OnSortChanged(string sortingField, ListSortDirection sortingDirection)
        {
            var args = Tuple.Create(sortingField, sortingDirection);
            var newEvent = new RoutedEventArgs(SortChangedEvent, args);
            RaiseEvent(newEvent);
            Command?.Execute(null);
        }

        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(ListViewHeader), new PropertyMetadata(default(ICommand)));

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        #endregion
    }
    /// <summary>
    /// 排序列表附加属性辅助类
    /// </summary>
    internal class SortHelper
    {
        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.RegisterAttached("SortDirection",
            typeof(ListViewSortDirection), typeof(SortHelper), new PropertyMetadata(ListViewSortDirection.Default));

        public static void SetSortDirection(DependencyObject element, ListViewSortDirection value)
        {
            element.SetValue(SortDirectionProperty, value);
        }

        public static ListViewSortDirection GetSortDirection(DependencyObject element)
        {
            return (ListViewSortDirection)element.GetValue(SortDirectionProperty);
        }

        public static readonly DependencyProperty SortFieldProperty = DependencyProperty.RegisterAttached("SortField",
            typeof(string), typeof(SortHelper), new PropertyMetadata(default(string)));

        public static void SetSortField(DependencyObject element, string value)
        {
            element.SetValue(SortFieldProperty, value);
        }

        public static string GetSortField(DependencyObject element)
        {
            return (string)element.GetValue(SortFieldProperty);
        }
    }
    internal enum ListViewSortDirection
    {
        Default,
        Ascending,
        Descending,
    }
    /// <summary>
    /// 排序选项
    /// 用于列表排序
    /// </summary>
    public class ListSortOption
    {
        /// <summary>
        /// 排序方向
        /// </summary>
        public ListSortDirection SortDirection { get; set; }

        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; } = string.Empty;
    }
    /// <summary>
    /// 标题列表内容
    /// </summary>
    public sealed class ListViewHeaderContentCollection : IList<HeaderContent>, ICollection<HeaderContent>, IEnumerable<HeaderContent>, IEnumerable, IList, ICollection
    {
        private readonly List<HeaderContent> _headContents = new List<HeaderContent>();
        IEnumerator<HeaderContent> IEnumerable<HeaderContent>.GetEnumerator()
        {
            return _headContents.GetEnumerator();
        }

        public IEnumerator<HeaderContent> GetEnumerator()
        {
            return _headContents.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(HeaderContent item)
        {
            _headContents.Add(item);
        }

        public int Add(object value)
        {
            if (value is HeaderContent headerContent)
            {
                _headContents.Add(headerContent);
            }
            else if (value is ListViewHeaderContentCollection headerContentCollection)
            {
                _headContents.AddRange(headerContentCollection._headContents);
            }
            return _headContents.Count;
        }

        public bool Contains(object value)
        {
            return _headContents.Contains((HeaderContent)value);
        }

        public void Clear()
        {
            _headContents.Clear();
        }

        public int IndexOf(object value)
        {
            return _headContents.IndexOf((HeaderContent)value);
        }

        public void Insert(int index, object value)
        {
            _headContents.Insert(index, (HeaderContent)value);
        }

        public void Remove(object value)
        {
            _headContents.Remove((HeaderContent)value);
        }

        void IList.RemoveAt(int index)
        {
            _headContents.RemoveAt(index);
        }

        object IList.this[int index]
        {
            get => _headContents[index];
            set => _headContents[index] = (HeaderContent)value;
        }

        public bool Contains(HeaderContent item)
        {
            return _headContents.Contains(item);
        }

        public void CopyTo(HeaderContent[] array, int arrayIndex)
        {
            _headContents.CopyTo(array, arrayIndex);
        }

        public bool Remove(HeaderContent item)
        {
            return _headContents.Remove(item);
        }

        public void CopyTo(Array array, int index)
        {
            _headContents.CopyTo((HeaderContent[])array, index);
        }

        public int Count => _headContents.Count;

        public object SyncRoot { get; }

        public bool IsSynchronized { get; }

        public bool IsReadOnly { get; }

        public bool IsFixedSize { get; }

        public int IndexOf(HeaderContent item)
        {
            return _headContents.IndexOf(item);
        }

        public void Insert(int index, HeaderContent item)
        {
            _headContents.Insert(index, item);
        }

        void IList<HeaderContent>.RemoveAt(int index)
        {
            _headContents.RemoveAt(index);
        }

        public HeaderContent this[int index]
        {
            get => _headContents[index];
            set => _headContents[index] = value;
        }

    }
    public class HeaderContent
    {
        public string Content { get; set; }

        public GridLength Width { get; set; }

        public double MinWidth { get; set; }

        public HorizontalAlignment HorizontalAlignment { get; set; } = HorizontalAlignment.Left;

        public Thickness Padding { get; set; } = new Thickness(0.0);

        /// <summary>
        /// 用于排序的字段的名称
        /// </summary>
        public string SortName { get; set; }

        public int GridColumn { get; set; }
    }
    public static class GridExtension
    {
        public static void SetColumn(HeaderContent headerContent, int value)
        {
            headerContent.GridColumn = value;
        }
    }
}
