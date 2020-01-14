using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reactive;
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
        public ReactiveCommand<Unit, Unit> DeleteSource { get; }

        public SourceViewModel(Source src)
        {
            _value = src;
            OpenFile = ReactiveCommand.Create(() =>
            {
                Process.Start(new ProcessStartInfo(Value.Path)
                {
                    UseShellExecute = true
                });
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
