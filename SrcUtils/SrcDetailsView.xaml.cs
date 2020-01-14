using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HCGStudio.SrcUtils.Models;
using HCGStudio.SrcUtils.ViewModels;
using ReactiveUI;

namespace HCGStudio.SrcUtils
{
    /// <summary>
    /// SrcDetailsView.xaml 的交互逻辑
    /// </summary>
    public partial class SrcDetailsView
    {
        public SrcDetailsView()
        {
            InitializeComponent();

            this.WhenActivated(disposableRegistration =>
            {
                // Our 4th parameter we convert from Url into a BitmapImage. 
                // This is an easy way of doing value conversion using ReactiveUI binding.
                //this.OneWayBind(ViewModel,
                //        viewModel => viewModel.IconUrl,
                //        view => view.IconImage.Source,
                //        url => url == null ? null : new BitmapImage(url))
                //    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Value.Name,
                        view => view.TitleRun.Text)
                    .DisposeWith(disposableRegistration);

                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Value.Description,
                        view => view.DescriptionRun.Text)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.OpenFile,
                        view => view.OpenButton)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.ViewDetails,
                        view => view.Details)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.OpenFile,
                        view => view.OpenMenu)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.ViewDetails,
                        view => view.ViewMenu)
                    .DisposeWith(disposableRegistration);

                this.BindCommand(ViewModel,
                        viewModel => viewModel.DeleteSource,
                        view => view.DeleteMenu)
                    .DisposeWith(disposableRegistration);

                this.Events().MouseDoubleClick.Subscribe(e =>
                {
                    Process.Start(new ProcessStartInfo(ViewModel.Value.Path)
                    {
                        UseShellExecute = true
                    });
                });
            });
        }
    }
}
