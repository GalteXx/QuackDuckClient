using System;
using System.Windows;
using System.Windows.Controls;

namespace QuackDuckClient.Components
{
    /// <summary>
    /// Логика взаимодействия для Dialog.xaml
    /// </summary>
    public partial class Dialog : UserControl
    {
        public string ConversationTitle
        {
            get { return (string)GetValue(ConversationTitleProperty); }
            set { SetValue(ConversationTitleProperty, value); }
        }
        public string LastOnlineStamp
        {
            get { return (string)GetValue(LastOnlineStampProperty); }
            set
            {
                if (value[2] != ':')
                    throw new Exception("Wrong time format\nShould be hh:mm\ngot " + value);
                SetValue(LastOnlineStampProperty, value);
            }
        }


        // Using a DependencyProperty as the backing store for bindables
        public static readonly DependencyProperty ConversationTitleProperty =
            DependencyProperty.Register("ConversationTitle", typeof(string), typeof(ChatTape), new PropertyMetadata("Firstname Lastname"));

        public static readonly DependencyProperty LastOnlineStampProperty =
            DependencyProperty.Register("LastOnlineStamp", typeof(string), typeof(ChatTape), new PropertyMetadata("00:00"));

        public Dialog()
        {
            InitializeComponent();

        }
    }
}
