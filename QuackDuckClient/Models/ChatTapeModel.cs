using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using VkNet;
using VkNet.Model;
using VkNet.Model.RequestParams;

namespace QuackDuckClient.Models
{
    public class ChatTapeModel : INotifyPropertyChanged
    {
        // Variables
        private static VkApi _vkApi;

        private long? _chatId;
        private string _chatTitle;
        private string _lastMessageText;
        private DateTime _messageTime;

        // Properties
        public string ChatTitle 
        { 
            get => _chatTitle;
            set
            {
                _chatTitle = value;
                OnPropertyChanged("ChatTitle");
            }
        }

        public string LastMessageText 
        {
            get => _lastMessageText;
            set
            {
                _lastMessageText = value;
                OnPropertyChanged("LastMessageText");
            }
        }

        public DateTime MessageTime
        {
            get => _messageTime; 
            set
            {
                _messageTime = value;
                OnPropertyChanged("MessageTime");
            }

        }

        public long? ChatId { 
            get => _chatId;
            set
            {
                _chatId = value;
                OnPropertyChanged("LastMessageText");
            }
        }

        // Interface realization
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    
        // Methods
        public static List<ChatTapeModel> GetChatTapes(VkApi api)
        {
            _vkApi = api;

            var response = _vkApi.Messages.GetDialogs(new MessagesDialogsGetParams() { Count = 10 }).Messages.GetEnumerator();
            return TransformDialogsToTape(response);
        }

        public static ChatTapeModel CreateNewChatTape(Message dialog)
        {
            return new ChatTapeModel()
            {
                ChatId = dialog.PeerId,
                ChatTitle = GetChatTitle(dialog),
                LastMessageText = GetChatBody(dialog),
                MessageTime = dialog.Date ?? DateTime.MinValue
            };
        }

        /// <summary>
        /// Translates VkNet Message object to ChatTapeModel object
        /// </summary>
        /// <param name="dialogs">List of messages to translate</param>
        /// <returns>A generic list of translated ChatTapeModels</returns>
        public static List<ChatTapeModel> TransformDialogsToTape(IEnumerator<Message> dialogs)
        {
            List<ChatTapeModel> chatTapes = new List<ChatTapeModel>();
            while (dialogs.MoveNext())
            {
                var dialog = dialogs.Current;

                long? chatID = -1;
                if (dialog.ChatId is null)
                    chatID = dialog.UserId;
                else
                    chatID = dialog.ChatId;


                ChatTapeModel model = new ChatTapeModel()
                {
                    ChatTitle = GetChatTitle(dialog),
                    LastMessageText = GetChatBody(dialog),
                    ChatId = chatID,
                    MessageTime = dialog.Date ?? DateTime.MinValue
                };
                chatTapes.Add(model);
            }

            return chatTapes;
        }

        /// <summary>
        /// Gets the last message of the dialog.
        /// </summary>
        /// <param name="dialog"></param>
        /// <returns></returns>
        private static string GetChatBody(Message dialog)
        {
            // Usually it is dialog.Body but sometimes last message is an attachment
            if (!string.IsNullOrEmpty(dialog.Body))
                return dialog.Body;
           
            var attachments = dialog.Attachments.GetEnumerator();
            string attachmentTypeName = "";
            
            while (attachments.MoveNext())
                attachmentTypeName = attachments.Current.Type.Name;

            return attachmentTypeName;
        }

        /// <summary>
        /// Gets chat title for the chat tape
        /// </summary>
        /// <param name="dialog">Dialog object to extract data from</param>
        /// <returns>String result of name+surname or conversation title.</returns>
        private static string GetChatTitle(Message dialog)
        {
            // If dialog is not a conversation
            if (string.IsNullOrEmpty(dialog.Title))
            {
                // VK support has negative ID 
                if (dialog.UserId < 0)
                    return $"Команда Поддержки ВКонтакте";

                // Get user and return his first and last name
                var request = new long[] { dialog.UserId ?? throw new Exception($"Wrong id\nGot: {dialog.UserId}") };
                var user = _vkApi.Users.Get(request).FirstOrDefault();
                return $"{user.FirstName} {user.LastName}";
            }
            return dialog.Title;
        }
    }
}
