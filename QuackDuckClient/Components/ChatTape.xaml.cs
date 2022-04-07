using QuackDuckClient.Models;
using QuackDuckClient.ViewModels;
using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace QuackDuckClient.Components
{
    /// <summary>
    /// Логика взаимодействия для ChatTape.xaml
    /// </summary>
    public partial class ChatTape : UserControl
    {
        public string UserName
        {
            get { return (string)GetValue(UserNameProperty); }
            set { SetValue(UserNameProperty, value); }
        }
        public string MessageTime
        {
            get { return (string)GetValue(MessageTimeProperty); }
            set {
                if (value[2] != ':')
                    throw new Exception("Wrong time format\nShould be hh:mm\ngot " + value);
                SetValue(MessageTimeProperty, value); 
            }
        }
        public string LastMessageText
        {
            get { return (string)GetValue(LastMessageTextProperty); }
            set { SetValue(LastMessageTextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for bindables
        public static readonly DependencyProperty UserNameProperty =
            DependencyProperty.Register("UserName", typeof(string), typeof(ChatTape), new PropertyMetadata("FirstName LastName"));

        public static readonly DependencyProperty LastMessageTextProperty =
            DependencyProperty.Register("LastMessageText", typeof(string), typeof(ChatTape), new PropertyMetadata("Last message text..."));

        public static readonly DependencyProperty MessageTimeProperty =
            DependencyProperty.Register("MessageTime", typeof(string), typeof(ChatTape), new PropertyMetadata("00:00"));

        public ChatTape()
        {
            InitializeComponent();
        }
    }
}
