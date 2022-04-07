using VkNet.Model.RequestParams;
using VkNet;
using System;
using VkNet.Enums.Filters;
using System.Collections.Generic;
using VkNet.Model;

namespace QuackDuckClient
{
    public class LongPollingManager
    {
        private static VkApi _vkApi;
        private static ulong _ts;

        public LongPollingManager(VkApi vkApi)
        {
            _vkApi = vkApi;
            var lpServer = _vkApi.Messages.GetLongPollServer();
            _ts = Convert.ToUInt64(lpServer.Ts);
        }

        public static IReadOnlyCollection<Message> UpdateMessages()
        {
            var res = _vkApi.Messages.GetLongPollHistory(new MessagesGetLongPollHistoryParams() { Ts = _ts, Fields = UsersFields.Online | UsersFields.OnlineMobile });
            return res.Messages;
        }
    }
}
