using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HCGStudio.SrcUtils.Models;
using HCGStudio.SrcUtils.ViewModels;
using Microsoft.Win32;
using ReactiveUI;

namespace HCGStudio.SrcUtils
{
    /// <summary>
    /// SourceDetails.xaml 的交互逻辑
    /// </summary>
    public partial class SourceDetails
    {

        private void BindAll()
        {

            this.WhenActivated(disposableRegistration =>
            {
                //Bind name
                this.Bind(ViewModel,
                    viewModel => viewModel.Value.Name,
                    view => view.NameText.Text)
                    .DisposeWith(disposableRegistration);

                //Bind Category
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Value.Category,
                        view => view.CategoryText.Text)
                    .DisposeWith(disposableRegistration);

                //Bind Tags
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.TagsText,
                        view => view.TagsText.Text)
                    .DisposeWith(disposableRegistration);

                //Bind Path
                this.Bind(ViewModel,
                        viewModel => viewModel.Value.Path,
                        view => view.PathText.Text)
                    .DisposeWith(disposableRegistration);

                //Bind Description
                this.Bind(ViewModel,
                        viewModel => viewModel.Value.Description,
                        view => view.DescriptionText.Text)
                    .DisposeWith(disposableRegistration);

                //Bind ModifyTags
                this.BindCommand(ViewModel,
                    viewModel => viewModel.ModifyTags,
                    view => view.ModifyTags)
                    .DisposeWith(disposableRegistration);

                //Bind ModifyPath
                this.BindCommand(ViewModel,
                        viewModel => viewModel.ModifyPath,
                        view => view.ModifyPath)
                    .DisposeWith(disposableRegistration);

                //Bind ModifyCategory
                this.BindCommand(ViewModel,
                        viewModel => viewModel.ModifyCategory,
                        view => view.ModifyCategory)
                    .DisposeWith(disposableRegistration);

                //Bind Title
                this.Bind(ViewModel,
                        viewModel => viewModel.Value.Name,
                        view => view.Title)
                    .DisposeWith(disposableRegistration);

                //Subscribe Cancel
                Cancel.Click += (sender, e) => { Close(); };

                //Subscribe Save
                Save.Click += async (sender,e) =>
                {
                    if (string.IsNullOrWhiteSpace(ViewModel.Value.Name))
                    {
                        MessageBox.Show("请输入名称", "提示", MessageBoxButton.OK, MessageBoxImage.Asterisk);
                    }
                    else
                    {
                        await using var context = new SrcContext();
                        if (ViewModel.Value.SourceId == 0)
                        {
                            await context.Sources.AddAsync(ViewModel.Value);
                        }
                        else
                        {
                            var modify = context.Sources.First(src => src.SourceId == ViewModel.Value.SourceId);
                            modify.Description = ViewModel.Value.Description;
                            modify.Category = ViewModel.Value.Category;
                            modify.Name = ViewModel.Value.Name;
                            modify.TagsString = ViewModel.TagsText;
                            modify.Path = ViewModel.Value.Path;
                        }

                        //Add Tags
                        foreach (var tag in ViewModel.Value.Tags.Where(tag => !(from t in context.Tags where t.Name == tag select t).Any()))
                        {
                            await context.Tags.AddAsync(new Tag {Name = tag});
                        }

                        //Add Category
                        if (!(from c in context.Categories where c.Name == ViewModel.Value.Category select c).Any())
                        {
                            await context.Categories.AddAsync(new Category {Name = ViewModel.Value.Category});
                        }
                        await context.SaveChangesAsync().ConfigureAwait(false);
                        Close();
                    }
                };

                //Subscribe ModifyPath
                ModifyPath.Click += (sender, e) =>
                {
                    var dialog = new OpenFileDialog
                    {
                        Multiselect = false,
                        Title = "选择源",
                        Filter = "源文件 (*.*)|*.*",
                        CheckFileExists = true,
                        InitialDirectory = System.IO.Path.GetDirectoryName(ViewModel.Value.Path),
                        FileName = System.IO.Path.GetFileName(ViewModel.Value.Path)
                    };

                    dialog.ShowDialog();
                    ViewModel.Value.Path = System.IO.Path.GetFullPath(dialog.FileName);
                    ViewModel.RaisePropertyChanged(nameof(ViewModel.Value));
                };

            });
        }
        public SourceDetails()
        {
            InitializeComponent();
            ViewModel = new SourceViewModel(new Source());
            BindAll();

        }
        public SourceDetails(SourceViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;
            BindAll();
        }

    }
}
