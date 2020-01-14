using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Windows;
using HCGStudio.SrcUtils.Models;
using ReactiveUI;

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
        public ReactiveCommand<Unit,Unit> OpenFolder { get; }
        public ReactiveCommand<Unit, Unit> DeleteSource { get; }

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
