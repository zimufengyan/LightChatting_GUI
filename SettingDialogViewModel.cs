using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace LightChatting_GUI
{
    public class SettingDialogViewModel : ViewModelBase
    {
        private string defaultName;

        public string DefaultName { get => defaultName; set => SetProperty( ref defaultName, value ); }

        private string ipAddress;

        public string IpAddress { get => ipAddress; set => SetProperty( ref ipAddress, value ); }

        private string port;

        public string  Port { get => port; set => SetProperty( ref port, value ); }

        public int TruePort { get => int.Parse( Port ); }

        public bool CanChatting
        {
            get
            {
                return DefaultName != null && IpAddress != null && TruePort != 0;
            }

        }

    }

    public class NameValidationRule : ValidationRule
    {
        public override ValidationResult Validate( object value, CultureInfo cultureInfo )
        {
            // 匹配名字，仅支持4-8英文字母
            Regex regex = new( @"^[A-Za-z]{4,8}$" );
            if ( value != null && regex.IsMatch( value.ToString() ) )
            {
                return new ValidationResult( true, null );
            }
            else
            {
                return new ValidationResult(
                    false, "The Name can only contain 4-8 English characters" );
            }
        }

    }

    public class IpValidationRule : ValidationRule
    {
        public override ValidationResult Validate( object value, CultureInfo cultureInfo )
        {
            // 匹配IP地址
            Regex regex = new( @"^((2((5[0-5])|([0-4]\d)))|([0-1]?\d{1,2}))(\.((2((5[0-5])|([0-4]\d)))|([0-1]?\d{1,2}))){3}$" );
            if ( value != null && regex.IsMatch( value.ToString() ) )
            {
                return new ValidationResult( true, null );
            }
            else
            {
                return new ValidationResult(
                    false, "The string cannot be resolved to a canonical IP address" );
            }
        }

    }
    public class PortValidationRule : ValidationRule
    {
        public override ValidationResult Validate( object value, CultureInfo cultureInfo )
        {
            // 匹配Port
            Regex regex = new( @"^\d{5}$" );
            if ( value != null && regex.IsMatch( value.ToString() ) )
            {
                return new ValidationResult( true, null );
            }
            else
            {
                return new ValidationResult(
                    false, "The value cannot be resolved to a canonical Port" );
            }
        }

    }
}
