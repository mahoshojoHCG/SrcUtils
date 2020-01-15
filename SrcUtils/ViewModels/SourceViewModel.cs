using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Windows;
using System.Windows.Forms;
using HCGStudio.SrcUtils.Models;
using ModernWpf.Controls;
using ReactiveUI;
using Clipboard = System.Windows.Clipboard;
using MessageBox = System.Windows.MessageBox;
using SaveFileDialog = Microsoft.Win32.SaveFileDialog;

namespace HCGStudio.SrcUtils.ViewModels
{

    public class SourceViewModel : ReactiveObject, ICloneable
    {
        private Source _value;
        public Source Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        public string TagsText
        {
            get => _value.TagsString;
            set
            {
                _value.TagsString = value;
                this.RaisePropertyChanged();
            }

        }

        public ReactiveCommand<Unit, Unit> OpenFile { get; }
        public ReactiveCommand<Unit, Unit> ModifyCategory { get; }
        public ReactiveCommand<Unit, Unit> ModifyTags { get; }
        public ReactiveCommand<Unit, Unit> ModifyPath { get; }
        public ReactiveCommand<Unit, Unit> ViewDetails { get; }
        public ReactiveCommand<Unit, Unit> OpenFolder { get; }
        public ReactiveCommand<Unit, Unit> DeleteSource { get; }
        public ReactiveCommand<Unit, Unit> CopyTo { get; }
        public ReactiveCommand<Unit, Unit> MoveTo { get; }
        public ReactiveCommand<Unit, Unit> CopyToClipBoard { get; }

        public SourceViewModel(Source src)
        {
            _value = src;

            OpenFile = ReactiveCommand.CreateFromTask(async () =>
            {
                if (!File.Exists(Value.Path) && !Directory.Exists(Value.Path))
                {
                    var result = MessageBox.Show("无法定位到文件，是否从记录中删除这一项？", "未能找到指定源文件", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        await using var context = new SrcContext();
                        var toDelete = context.Sources.First(source => source.SourceId == Value.SourceId);
                        if (toDelete != null)
                        {
                            context.Sources.Remove(toDelete);
                            await context.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(Value.Path)
                        {
                            UseShellExecute = true
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "错误");
                    }
                }

            });

            ModifyCategory = ReactiveCommand.Create(() =>
            {
                var inputModel = new InputViewModel
                {
                    Hint = "请输入新的分类",
                    Title = "修改分类",
                    Value = Value.Category
                };
                var input = new InputWindow(inputModel);
                input.ShowDialog();
                if (!inputModel.IsCanceled)
                {
                    Value.Category = inputModel.Value;
                }
                this.RaisePropertyChanged(nameof(Value));
            });

            ModifyTags = ReactiveCommand.Create(() =>
            {
                var inputModel = new InputViewModel
                {
                    Hint = "请输入新的标签，用英文分号;分割",
                    Title = "修改标签",
                    Value = TagsText
                };
                var input = new InputWindow(inputModel);
                input.ShowDialog();
                if (!inputModel.IsCanceled)
                {
                    TagsText = inputModel.Value;
                }
            });

            ModifyPath = ReactiveCommand.Create(() =>
            {

                this.RaisePropertyChanged(nameof(Value));
            });

            DeleteSource = ReactiveCommand.CreateFromTask(async () =>
            {
                await using var context = new SrcContext();
                var toDelete = context.Sources.First(source => source.SourceId == Value.SourceId);
                if (toDelete != null)
                {
                    context.Sources.Remove(toDelete);
                    await context.SaveChangesAsync();
                }
            });

            ViewDetails = ReactiveCommand.Create(() =>
            {
                var window = new SourceDetails(Clone());
                window.Show();
            });

            OpenFolder = ReactiveCommand.CreateFromTask(async () =>
            {

                if (!File.Exists(Value.Path) && !Directory.Exists(Value.Path))
                {
                    var result = MessageBox.Show("无法定位到文件，是否从记录中删除这一项？", "未能找到指定源文件", MessageBoxButton.YesNo);
                    if (result == MessageBoxResult.Yes)
                    {
                        await using var context = new SrcContext();
                        var toDelete = context.Sources.First(source => source.SourceId == Value.SourceId);
                        if (toDelete != null)
                        {
                            context.Sources.Remove(toDelete);
                            await context.SaveChangesAsync();
                        }
                    }
                }
                else
                {
                    try
                    {
                        Process.Start(new ProcessStartInfo(Path.GetDirectoryName(Value.Path))
                        {
                            UseShellExecute = true
                        });
                    }
                    catch (Exception e)
                    {
                        MessageBox.Show(e.Message, "错误");
                    }
                }
            });

            CopyTo = ReactiveCommand.CreateFromTask(async () =>
            {
                //for file
                if (File.Exists(Value.Path))
                {
                    var dialog = new SaveFileDialog
                    {
                        Title = "选择复制到的位置",
                        Filter = $"相同文件名的源|{Path.GetFileName(Value.Path)}|更改了文件名的源 (*.*)|*.*",
                        CheckFileExists = true,
                        InitialDirectory = Path.GetDirectoryName(Value.Path),
                        FileName = Path.GetFileName(Value.Path)
                    };

                    dialog.ShowDialog();


                    //Check if source and dest is the same
                    var realPath = Path.GetFullPath(dialog.FileName);
                    if (realPath == Value.Path)
                    {
                        var contentDialog = new ContentDialog
                        {
                            Title = "错误",
                            Content = "不能复制到相同的位置！",
                            CloseButtonText = "好"
                        };

                        await contentDialog.ShowAsync();
                    }
                    else
                    {

                        try
                        {
                            File.Copy(Value.Path, realPath, true);
                        }
                        catch (Exception e)
                        {
                            var contentDialog = new ContentDialog
                            {
                                Title = "错误",
                                Content = e.Message,
                                CloseButtonText = "好"
                            };

                            await contentDialog.ShowAsync();
                        }
                    }

                }
                else if (Directory.Exists(Value.Path))
                {
                    //For folder
                    var dialog = new FolderBrowserDialog
                    {
                        SelectedPath = Value.Path
                    };

                    dialog.ShowDialog();

                    var realPath = Path.GetFullPath(dialog.SelectedPath);

                    if (realPath == Value.Path)
                    {
                        var contentDialog = new ContentDialog
                        {
                            Title = "错误",
                            Content = "不能复制到相同的位置！",
                            CloseButtonText = "好"
                        };

                        await contentDialog.ShowAsync();
                    }
                    else
                    {
                        try
                        {
                            //Copy folders
                            foreach (var dirPath in Directory.GetDirectories(Value.Path, "*",
                                SearchOption.AllDirectories))
                            {
                                Directory.CreateDirectory(Path.Combine(realPath, dirPath[Value.Path.Length..]));
                            }

                            //Copy files
                            foreach (var newPath in Directory.GetFiles(Value.Path, "*.*",
                                SearchOption.AllDirectories))
                            {
                                File.Copy(newPath, Path.Combine(realPath, newPath[Value.Path.Length..]), true);
                            }
                        }
                        catch (Exception e)
                        {
                            var contentDialog = new ContentDialog
                            {
                                Title = "错误",
                                Content = e.Message,
                                CloseButtonText = "好"
                            };

                            await contentDialog.ShowAsync();
                        }
                    }
                }
            });

            MoveTo = ReactiveCommand.CreateFromTask(async () =>
            {
                //for file
                if (File.Exists(Value.Path))
                {
                    var dialog = new SaveFileDialog
                    {
                        Title = "选择移动到的位置",
                        Filter = $"相同文件名的源|{Path.GetFileName(Value.Path)}|更改了文件名的源 (*.*)|*.*",
                        CheckFileExists = true,
                        InitialDirectory = Path.GetDirectoryName(Value.Path),
                        FileName = Path.GetFileName(Value.Path)
                    };

                    dialog.ShowDialog();


                    //Check if source and dest is the same
                    var realPath = Path.GetFullPath(dialog.FileName);
                    if (realPath == Value.Path)
                    {
                        var contentDialog = new ContentDialog
                        {
                            Title = "错误",
                            Content = "不能移动到相同的位置！",
                            CloseButtonText = "好"
                        };

                        await contentDialog.ShowAsync();
                    }
                    else
                    {

                        try
                        {
                            File.Move(Value.Path, realPath, true);
                        }
                        catch (Exception e)
                        {
                            var contentDialog = new ContentDialog
                            {
                                Title = "错误",
                                Content = e.Message,
                                CloseButtonText = "好"
                            };

                            await contentDialog.ShowAsync();
                        }
                    }

                }
                else if (Directory.Exists(Value.Path))
                {
                    //For folder
                    var dialog = new FolderBrowserDialog
                    {
                        SelectedPath = Value.Path
                    };

                    dialog.ShowDialog();

                    var realPath = Path.GetFullPath(dialog.SelectedPath);

                    if (realPath == Value.Path)
                    {
                        var contentDialog = new ContentDialog
                        {
                            Title = "错误",
                            Content = "不能移动到相同的位置！",
                            CloseButtonText = "好"
                        };

                        await contentDialog.ShowAsync();
                    }
                    else
                    {
                        try
                        {
                            Directory.Move(Value.Path, realPath);
                        }
                        catch (Exception e)
                        {
                            var contentDialog = new ContentDialog
                            {
                                Title = "错误",
                                Content = e.Message,
                                CloseButtonText = "好"
                            };

                            await contentDialog.ShowAsync();
                        }
                    }
                }
            });

            CopyToClipBoard = ReactiveCommand.Create(() =>
            {
                Clipboard.SetFileDropList(new StringCollection { Value.Path });
            });
        }

        public SourceViewModel Clone()
        {
            return new SourceViewModel(Value.Clone());
        }
        object ICloneable.Clone()
        {
            return Clone();
        }
    }
}
