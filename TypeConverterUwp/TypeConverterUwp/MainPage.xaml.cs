using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace TypeConverterUwp
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
    }

    public class MyContentControl : ContentControl
    {
        /// <summary>
        /// 获取或设置Amount的值
        /// </summary>  
        [TypeConverter(typeof(DecimalConverter))]
        public Decimal Amount
        {
            get { return (Decimal)GetValue(AmountProperty); }
            set { SetValue(AmountProperty, value); }
        }

        /// <summary>
        /// 标识 Amount 依赖属性。
        /// </summary>
        public static readonly DependencyProperty AmountProperty =
            DependencyProperty.Register("Amount", typeof(Decimal), typeof(MyContentControl), new PropertyMetadata(0m, OnAmountChanged));

        private static void OnAmountChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            MyContentControl target = obj as MyContentControl;
            Decimal oldValue = (Decimal)args.OldValue;
            Decimal newValue = (Decimal)args.NewValue;
            if (oldValue != newValue)
                target.OnAmountChanged(oldValue, newValue);
        }

        protected virtual void OnAmountChanged(Decimal oldValue, Decimal newValue)
        {
            Content = "Amount is " + newValue;
        }
    }

    public class Email
    {
        public ReceiverCollection Receivers { get; set; }
    }

    public class Receiver
    {
        public string Name { get; set; }
    }

    [Windows.Foundation.Metadata.CreateFromString(MethodName = "TypeConverterUwp.ReceiverCollection.Parse")]
    public class ReceiverCollection : ObservableCollection<Receiver>
    {
        public static ReceiverCollection Parse(string source)
        {
            var result = new ReceiverCollection();
            var tokens = source.Split(';');
            foreach (var token in tokens)
            {
                result.Add(new Receiver { Name = token });
            }
            return result;
        }
    }

 

    public class StringToDecimalBridge
    {
        public Decimal this[string key]
        {
            get
            {
                return Convert.ToDecimal(key);
            }
        }
    }

}
