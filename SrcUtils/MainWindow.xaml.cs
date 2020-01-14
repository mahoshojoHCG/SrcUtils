using System;
using System.IO;
using System.Reactive.Disposables;
using System.Windows;
using System.Windows.Controls;
using HCGStudio.SrcUtils.Models;
using HCGStudio.SrcUtils.ViewModels;
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
