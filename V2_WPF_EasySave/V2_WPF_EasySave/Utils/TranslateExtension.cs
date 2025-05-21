using System;
using System.Windows.Markup;
using System.Windows.Data;
using System.Windows;

namespace V2_WPF_EasySave.Utils
{
    public class TranslateExtension : MarkupExtension // Markup extension that generates a binding to languagemanager
    {
        public string Key { get; set; }

        public TranslateExtension(string key)
        {
            Key = key;
        }

        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            var binding = new Binding($"[{Key}]")
            {
                Source = LanguageManager.Instance,
                Mode = BindingMode.OneWay
            };
            return binding.ProvideValue(serviceProvider);
        }
    }
}