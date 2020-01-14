using System;
using System.Collections.Generic;
using System.Reactive.Disposables;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using HCGStudio.SrcUtils.ViewModels;
using ReactiveUI;

namespace HCGStudio.SrcUtils
{
    /// <summary>
    /// InputWindow.xaml 的交互逻辑
    /// </summary>
    public partial class InputWindow : ReactiveWindow<InputViewModel>
    {
        public InputWindow()
        {

            InitializeComponent();
            ViewModel = new InputViewModel();

            this.WhenActivated(disposableRegistration =>
            {
                //Bind HintText
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Hint,
                        view => view.HintText)
                    .DisposeWith(disposableRegistration);

                //Bind Title
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Title,
                        view => view.Title)
                    .DisposeWith(disposableRegistration);

                //Bind Content
                this.Bind(ViewModel,
                        viewModel => viewModel.Value,
                        view => view.InputContent)
                    .DisposeWith(disposableRegistration);

                //Subscribe Yes
                Yes.Click += (sender, e) =>
                {
                    ViewModel.IsCanceled = false;
                    Close();
                };

                //Subscribe No
                No.Click += (sender, e) =>
                {
                    ViewModel.IsCanceled = true;
                    Close();
                };

            });
        }
        public InputWindow(InputViewModel vm)
        {
            InitializeComponent();
            ViewModel = vm;

            this.WhenActivated(disposableRegistration =>
            {
                //Bind HintText
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Hint,
                        view => view.HintText.Text)
                    .DisposeWith(disposableRegistration);

                //Bind Title
                this.OneWayBind(ViewModel,
                        viewModel => viewModel.Title,
                        view => view.Title)
                    .DisposeWith(disposableRegistration);

                //Bind Content
                this.Bind(ViewModel,
                        viewModel => viewModel.Value,
                        view => view.InputContent.Text)
                    .DisposeWith(disposableRegistration);

                //Subscribe Yes
                Yes.Click += (sender,e) =>
                {
                    ViewModel.IsCanceled = false;
                    Close();
                };

                //Subscribe No
                No.Click += (sender, e) =>
                {
                    ViewModel.IsCanceled = true;
                    Close();
                };

            });
        }
    }
}
