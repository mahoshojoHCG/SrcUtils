using System.Reactive;
using System.Windows.Forms;
using ReactiveUI;

namespace HCGStudio.SrcUtils.ViewModels
{
    public class InputViewModel : ReactiveObject
    {
        private string _value;
        public string Value
        {
            get => _value;
            set => this.RaiseAndSetIfChanged(ref _value, value);
        }

        private string _title;
        public string Title
        {
            get => _title;
            set => this.RaiseAndSetIfChanged(ref _title, value);
        }

        private string _hint;

        public string Hint
        {
            get => _hint;
            set => this.RaiseAndSetIfChanged(ref _hint, value);
        }

        public bool IsCanceled { get; set; }

    }
}
