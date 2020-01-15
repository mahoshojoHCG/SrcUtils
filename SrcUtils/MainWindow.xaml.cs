using System;
using System.Diagnostics;
using System.IO;
using System.Reactive.Disposables;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using HCGStudio.SrcUtils.Models;
using HCGStudio.SrcUtils.ViewModels;
using ModernWpf.Controls;
using ReactiveUI;
using EventExtensions = System.Windows.EventExtensions;

namespace HCGStudio.SrcUtils
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            ViewModel = new AppViewModel();

            this.WhenActivated(disposableRegistration =>
            {

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.IsAvailable,
                        view => view.SearchResultsListBox.Visibility)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.SearchResults,
                        view => view.SearchResultsListBox.ItemsSource)
                    .DisposeWith(disposableRegistration);

                this.Bind(ViewModel,
                        viewModel => viewModel.SearchTerm,
                        view => view.SearchTextBox.Text)
                    .DisposeWith(disposableRegistration);

                //Check update
                this.Events().Activated.Subscribe(async e =>
                {
                    var version = await Updater.VersionInfo.GetLeastVersionAsync();
                    var that = Assembly.GetExecutingAssembly().GetName().Version.ToString(3);
                    if (version.Version != Assembly.GetExecutingAssembly().GetName().Version.ToString(3))
                    {
                        var updateDialog = new ContentDialog
                        {
                            Title = $"有新版本{version.Version}，是否更新？",
                            Content = $"更新日志：{version.ChangeLog}",
                            PrimaryButtonText = "是",
                            CloseButtonText = "否"
                        };
                        var result = await updateDialog.ShowAsync();
                        if (result == ContentDialogResult.Primary)
                        {
                            Process.Start(new ProcessStartInfo(version.DownloadLink)
                            {
                                UseShellExecute = true
                            });
                        }
                    }
                });

                //Drag
                this.Events().PreviewDragOver.Subscribe(e =>
                {
                    e.Handled = true;
                    e.Effects = DragDropEffects.All;
                });



                this.Events().Drop.Subscribe(e =>
                {
                    if (e.Data.GetDataPresent(DataFormats.FileDrop))
                    {
                        var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                        foreach (var file in files)
                        {
                            var src = new Source
                            {
                                Name = Path.GetFileNameWithoutExtension(file),
                                Path = file
                            };
                            var vm = new SourceViewModel(src);
                            var window = new SourceDetails(vm);
                            window.Show();
                        }
                    }
                });


            });
        }
    }
}
