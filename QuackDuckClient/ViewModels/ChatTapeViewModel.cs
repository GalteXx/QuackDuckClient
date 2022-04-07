using QuackDuckClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Threading;
using VkNet;

namespace QuackDuckClient.ViewModels
{
    public class ChatTapeViewModel : INotifyPropertyChanged
    {
        private DispatcherTimer _timer;

        private static List<ChatTapeModel> _chatTapes;
        public static List<ChatTapeModel> ChatTapes { get => _chatTapes; set => _chatTapes = value; }
        
        public event PropertyChangedEventHandler PropertyChanged;


        public ChatTapeViewModel(VkApi api)
        {
            ChatTapes = ChatTapeModel.GetChatTapes(api);
            _timer = new DispatcherTimer()
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _timer.Tick += UpdateChatsTick; 
            _timer.Start();
        }

        private void UpdateChatsTick(object sender, EventArgs e)
        {
            //ChatTapes[0].ChatTitle = "qwerty";
            // Получаем изменения
            var changes = LongPollingManager.UpdateMessages().GetEnumerator();
            // Проверить, есть ли у меня этот chattape
            while (changes.MoveNext())
            {
                var dialog = changes.Current;
                var chatTape = ChatTapes.Where(x => x.ChatId == dialog.FromId).FirstOrDefault();
                if (!(chatTape is null))
                    chatTape.LastMessageText = dialog.Text;
            }
            // Изменить lastmessage у chattape соответствующего
        }
    }
}
