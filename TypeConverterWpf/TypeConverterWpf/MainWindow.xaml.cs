using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Globalization;

namespace TypeConverterWpf
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent(); 

            var value = GetValue<int>("10");
            var value2 = GetValueByChangeType<int>("10");
            var value3 = GetValueByTypeConverter<int>("10");
        }


        private T GetValue<T>(string source)
        {
            var type = typeof(T);
            T result;
            if (type == typeof(bool))
            {
                result = (T)(object)Convert.ToBoolean(source);
            }
            else if (type == typeof(string))
            {
                result = (T)(object)source;
            }
            else if (type == typeof(short))
            {
                result = (T)(object)Convert.ToInt16(source);
            }
            else if (type == typeof(int))
            {
                result = (T)(object)Convert.ToInt32(source);
            }
            else
            {
                result = default(T);
            }

            return result;
        }

        private T GetValueByChangeType<T>(string source)
        {
            return (T)Convert.ChangeType(source, typeof(T));
        }

        private T GetValueByTypeConverter<T>(string source)
        {
            var typeConverter = TypeDescriptor.GetConverter(typeof(T));
            if (typeConverter.CanConvertTo(typeof(T)))
                return (T)typeConverter.ConvertFromString(source);

            return default(T);
        }
    }

    public class Email
    {
        [TypeConverter(typeof(ReceiverCollectionConverterExtend))]
        public ReceiverCollection Receivers { get; set; }
    }

    public class Receiver
    {
        public string Name { get; set; }
    }


    [TypeConverter(typeof(ReceiverCollectionConverter))]
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

        public string ConvertToString()
        {
            var result = string.Empty;
            foreach (var item in this)
            {
                result += item.Name;
                result += ";";
            }
            return result;
        }
    }

    public class ReceiverCollectionConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }


        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value)
            {
                case null:
                    throw GetConvertFromException(null);
                case string source:
                    return ReceiverCollection.Parse(source);
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            switch (value)
            {
                case ReceiverCollection instance:
                    if (destinationType == typeof(string))
                    {
                        return instance.ConvertToString();
                    }
                    break;
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class ReceiverCollectionConverterExtend : ReceiverCollectionConverter
    {
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            switch (value)
            {
                case null:
                    throw GetConvertFromException(null);
                case string source:
                    return Parse(source);
            }

            return base.ConvertFrom(context, culture, value);

        }

        private ReceiverCollection Parse(string source)
        {
            var result = new ReceiverCollection();
            var tokens = source.Split(';');
            foreach (var token in tokens)
            {
                result.Add(new Receiver { Name = token.ToUpper() });
            }
            return result;
        }
    }
}
