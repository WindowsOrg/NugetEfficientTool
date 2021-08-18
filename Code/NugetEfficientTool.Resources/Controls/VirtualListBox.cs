using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace NugetEfficientTool.Resources
{
    /// <summary>
    /// 针对虚拟化处理的ListBox
    /// </summary>
    public class VirtualListBox : ListBox
    {
        private bool _isRefreshingSelectedItems = false;

        #region 虚化化选中

        #region 重新绑定ItemsSource时，更新SelectedItems列表

        /// <summary>
        /// 当前选中列表
        /// 注：解决重新绑定列表后，因listBox虚拟化ViewModel中的选中状态未能更新界面SelectedItems的问题。
        /// 解决方式：由绑定列表筛选出选中列表后，更新SelectedItems
        /// </summary>
        public static readonly DependencyProperty SelectedDocItemsProperty = DependencyProperty.Register(
            "SelectedDocItems", typeof(IList), typeof(VirtualListBox), new PropertyMetadata(
                new ObservableCollection<object>(),
                (d, e) => ((VirtualListBox)d).RefreshSelectedItems()));

        private void RefreshSelectedItems()
        {
            DoActionWhenSelecting(() =>
            {
                //ItemsSource重新绑定时，设置SelectedItems
                var newCustomSelectedItems = SelectedDocItems;
                if (SelectionMode == SelectionMode.Multiple)
                {
                    SelectedItems.Clear();
                    foreach (var newSelectedItem in newCustomSelectedItems)
                    {
                        SelectedItems.Add(newSelectedItem);
                    }
                }
                else
                {
                    if (newCustomSelectedItems.Count == 0)
                    {
                        SelectedItem = null;
                    }
                    else
                    {
                        SelectedItem = newCustomSelectedItems[0];
                    }
                }
                SelectedDocItemsUpdated?.Invoke(this, new SelectedDocItemsUpdateArgs()
                {
                    SelectionMode = SelectionMode,
                    SelectedItems = SelectedItems,
                    SelectedItem = SelectedItem,
                    Items = Items
                });
            });
        }
        /// <summary>
        /// 选中项变更
        /// 与<see cref="ListBox.SelectionChanged"/>不同，因虚拟化启动导致原有SelectionChanged准确，此事件在更新选中项后触发
        /// </summary>
        public event EventHandler<SelectedDocItemsUpdateArgs> SelectedDocItemsUpdated;

        private void DoActionWhenSelecting(Action action)
        {
            if (_isRefreshingSelectedItems)
            {
                return;
            }

            try
            {
                _isRefreshingSelectedItems = true;

                action.Invoke();
            }
            catch (Exception)
            {
                // ignored
            }
            finally
            {
                _isRefreshingSelectedItems = false;
            }
        }
        /// <summary>
        /// 自定义的列表选中项
        /// TODO 可以更新为新名称CustomSelectedItems
        /// </summary>
        public IList SelectedDocItems
        {
            get
            {
                return (IList)GetValue(SelectedDocItemsProperty);
            }
            set => SetValue(SelectedDocItemsProperty, value);
        }

        #endregion

        #region 列表选中时，更新列表IsSelected状态

        /// <summary>
        /// 解决云列表列表选中混乱问题
        /// why:界面更新SelectedItems时，因虚拟化导致不在当前视觉内的Item并没有将选中状态删除
        /// how:重写SelectionChanged，对比前后SelectedItems，将之前一部分SelectedItems的选中状态删除。
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            DoActionWhenSelecting(() =>
            {
                base.OnSelectionChanged(e);

                bool isVirtualizing = VirtualizingPanel.GetIsVirtualizing(this);

                if (isVirtualizing && (e.AddedItems.Count > 0 || e.RemovedItems.Count > 0))
                {
                    var customSelectItems = SelectedDocItems;

                    // 以下改动与此 BUG 相关
                    // 在明确了原因后在此再补充原因。初步定位为改了 DPI 后，此 OnSelectionChanged 事件会每次操作多触发一次。
                    var addedItems = e.AddedItems;
                    foreach (var addedItem in addedItems)
                    {
                        if (!customSelectItems.Contains(addedItem))
                        {
                            customSelectItems.Add(addedItem);
                        }
                    }

                    var deletedItems = e.RemovedItems;
                    foreach (var deletedItem in deletedItems)
                    {
                        if (customSelectItems.Contains(deletedItem))
                        {
                            customSelectItems.Remove(deletedItem);
                        }
                    }

                    SelectedDocItems = customSelectItems;
                }

                SelectedDocItemsUpdated?.Invoke(this, new SelectedDocItemsUpdateArgs()
                {
                    SelectionMode = SelectionMode,
                    SelectedItems = SelectedItems,
                    SelectedItem = SelectedItem,
                    Items = Items
                });
            });
        }

        #endregion

        #endregion


    }
    public class SelectedDocItemsUpdateArgs
    {
        public SelectionMode SelectionMode { get; set; }
        public IList SelectedItems { get; set; }
        public object SelectedItem { get; set; }
        public ItemCollection Items { get; set; }
    }
}
