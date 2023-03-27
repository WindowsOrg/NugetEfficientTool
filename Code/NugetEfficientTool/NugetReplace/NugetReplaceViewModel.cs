using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Input;
using NugetEfficientTool.Business;
using NugetEfficientTool.Utils;

namespace NugetEfficientTool
{
    public class NugetReplaceViewModel : BindingPropertyBase
    {
        public NugetReplaceViewModel()
        {
            AddNugetItemCommand = new ActionCommand(AddNugetItem);
            ReplaceNugetItemsCommand = new ActionCommand(ReplaceNugetItems);
            RevertNugetItemsCommand = new ActionCommand(RevertNugetItems);
            DeleteNugetItemCommand = new ActionCommand<NugetReplaceItem>(DeleteNugetItem);
            SortUpNugetItemCommand = new ActionCommand<NugetReplaceItem>(SortUpNugetItem);
        }

        private string _projectId = string.Empty;
        public void Initialize(INugetReplaceView view, string projectId, string solutionFileUrl)
        {
            _view = view;
            _projectId = projectId;
            SolutionFileUrl = solutionFileUrl;
            _nugetReplaceService = new NugetReplaceService(projectId);

            var nugetReplaceItems = new List<NugetReplaceItem>();
            var replaceNugetConfigs = NugetReplaceConfigs.GetNugetReplaceConfig(projectId);
            if (!string.IsNullOrEmpty(solutionFileUrl) && File.Exists(solutionFileUrl) && replaceNugetConfigs.Any())
            {
                nugetReplaceItems = replaceNugetConfigs.Select(i => new NugetReplaceItem(i.Name, i.SourceCsprojPath)
                {
                    HasReplaced = CheckNugetReplaced(SolutionFileUrl, i.Name)
                }).ToList();
            }
            else
            {
                //预置一个数据项
                nugetReplaceItems.Add(new NugetReplaceItem());
            }
            NugetReplaceItems = new ObservableCollection<NugetReplaceItem>(nugetReplaceItems);
            UpdateOperationStatus();
        }

        private bool CheckNugetReplaced(string solutionFileUrl, string nugetName)
        {
            return File.ReadAllText(solutionFileUrl).Contains(nugetName);
        }

        #region 添加Nuget

        public ICommand AddNugetItemCommand { get; }

        private void AddNugetItem()
        {
            NugetReplaceItems.Add(new NugetReplaceItem());
            UpdateOperationStatus();
        }

        #endregion

        #region 顺序

        public ICommand SortUpNugetItemCommand { get; }

        private void SortUpNugetItem(NugetReplaceItem obj)
        {
            var currentOrder = NugetReplaceItems.IndexOf(obj);
            if (currentOrder == 0)
            {
                return;
            }
            if (NugetReplaceItems.Any(i => i.HasReplaced))
            {
                NugetTools.Notification.ShowAsync(_view.Window, "当前有Nuget源已替换，不支持排序！");
                return;
            }
            NugetReplaceItems.Remove(obj);
            NugetReplaceItems.Insert(currentOrder - 1, obj);
        }

        #endregion

        #region 删除Nuget

        public ICommand DeleteNugetItemCommand { get; }

        private void DeleteNugetItem(NugetReplaceItem item)
        {
            if (item.HasReplaced)
            {
                NugetTools.Notification.ShowInfo(_view.Window, "Nuget已替换，请先还原引用再删除！");
                return;
            }
            NugetReplaceCacheManager.ClearReplacedNugetInfo(_projectId, SolutionFileUrl, item.NugetName);
            NugetReplaceItems.Remove(item);
            UpdateOperationStatus();
        }

        #endregion

        #region 替换Nuget

        public ICommand ReplaceNugetItemsCommand { get; }
        public ICommand RevertNugetItemsCommand { get; }

        /// <summary>
        /// 替换源包
        /// </summary>
        private void ReplaceNugetItems()
        {
            if (!CheckInputText(out var solutionFile, out var nugetItems)) return;
            try
            {
                foreach (var nugetReplaceItem in nugetItems)
                {
                    var replacedNugetInfo = _nugetReplaceService.Replace(solutionFile, nugetReplaceItem.NugetName, nugetReplaceItem.SourceCsprojFile);
                    if (replacedNugetInfo != null)
                    {
                        NugetReplaceCacheManager.SaveReplacedNugetInfo(_projectId, replacedNugetInfo);
                        nugetReplaceItem.HasReplaced = true;
                    }
                }
                UpdateOperationStatus();
            }
            catch (Exception exception)
            {
                nugetItems.ForEach(i => NugetReplaceCacheManager.ClearReplacedNugetInfo(_projectId, solutionFile, i.NugetName));
                NugetTools.Notification.ShowInfo(_view.Window, exception.Message);
                NugetTools.Log.Error(exception);
            }
        }
        /// <summary>
        /// 撤回原nuget引用
        /// </summary>
        private void RevertNugetItems()
        {
            if (!CheckInputText(out var solutionFile, out var nugetItems)) return;
            try
            {
                //对nuget包列表进行重新排序，先替换的先恢复
                var replacedNugetInfos = NugetReplaceCacheManager.GetReplacedNugetInfos(_projectId);
                nugetItems = nugetItems.OrderBy(i => replacedNugetInfos.FindIndex(replacedInfo =>
                      replacedInfo.Name == i.NugetName && replacedInfo.SourceCsprojPath == i.SourceCsprojFile)).ToList();
                foreach (var nugetReplaceItem in nugetItems)
                {
                    _nugetReplaceService.Revert(solutionFile, nugetReplaceItem.NugetName, nugetReplaceItem.SourceCsprojFile);
                    nugetReplaceItem.HasReplaced = false;
                }
                UpdateOperationStatus();
            }
            catch (Exception exception)
            {
                NugetTools.Notification.ShowInfo(_view.Window, exception.Message);
                NugetTools.Log.Error(exception);
            }
            //清空记录
            nugetItems.ForEach(i => NugetReplaceCacheManager.ClearReplacedNugetInfo(_projectId, solutionFile, i.NugetName));
        }

        private bool CheckInputText(out string solutionFile, out List<NugetReplaceItem> nugetItems)
        {
            solutionFile = SolutionFileUrl;
            nugetItems = NugetReplaceItems.Where(i => i.IsSelected).ToList();
            if (string.IsNullOrWhiteSpace(solutionFile))
            {
                NugetTools.Notification.ShowInfo(_view.Window, "项目路径不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            if (!File.Exists(solutionFile))
            {
                NugetTools.Notification.ShowInfo(_view.Window, "项目路径下找不到指定的解决方案，这是啥情况？？？");
                return false;
            }
            if (nugetItems.Any(i => string.IsNullOrWhiteSpace(i.NugetName)))
            {
                NugetTools.Notification.ShowInfo(_view.Window, "Nuget名称不能为空…… 心急吃不了热豆腐……");
                return false;
            }
            if (nugetItems.Any(i => string.IsNullOrWhiteSpace(i.SourceCsprojFile)))
            {
                NugetTools.Notification.ShowInfo(_view.Window, "源代码路径不能为空…… 心急吃不了热豆腐……");
                return false;
            }

            SolutionUrlUpdated?.Invoke(null, solutionFile);
            NugetReplaceConfigs.SaveNugetReplaceConfig(_projectId, nugetItems.Select(i => new ReplaceNugetConfig(i.NugetName, i.SourceCsprojFile)).ToList());
            return true;
        }

        public event EventHandler<string> SolutionUrlUpdated;

        #endregion

        #region 属性

        public ObservableCollection<NugetReplaceItem> NugetReplaceItems
        {
            get => _nugetReplaceItems;
            set
            {
                _nugetReplaceItems = value;
                OnPropertyChanged();
            }
        }
        /// <summary>
        /// 更新操作按钮状态
        /// </summary>
        public void UpdateOperationStatus()
        {
            var nugetReplaceItems = _nugetReplaceItems.Where(i => i.IsSelected).ToList();
            CanSelectedItemsReplace = nugetReplaceItems.Count > 0 && nugetReplaceItems.All(i => !i.HasReplaced);
            CanSelectedItemsRevert = nugetReplaceItems.Count > 0 && nugetReplaceItems.All(i => i.HasReplaced);
        }

        /// <summary>
        /// 解决方案
        /// </summary>
        public string SolutionFileUrl
        {
            get => _solutionFileUrl;
            set
            {
                _solutionFileUrl = value;
                OnPropertyChanged();
            }
        }
        private string _solutionFileUrl;

        public bool CanSelectedItemsReplace
        {
            get => _canSelectedItemsReplace;
            set
            {
                _canSelectedItemsReplace = value;
                OnPropertyChanged();
            }
        }
        private bool _canSelectedItemsReplace;

        public bool CanSelectedItemsRevert
        {
            get => _canSelectedItemRevert;
            set
            {
                _canSelectedItemRevert = value;
                OnPropertyChanged();
            }
        }

        private bool _canSelectedItemRevert;

        #endregion

        #region private fields

        private ObservableCollection<NugetReplaceItem> _nugetReplaceItems;
        private INugetReplaceView _view;
        private NugetReplaceService _nugetReplaceService;

        #endregion
    }
}
