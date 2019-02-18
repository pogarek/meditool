using System;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Threading.Tasks;
using PushoverClient;

namespace meditool
{
    class PushOverSender
    {

        public static void SendPushMessage(string UserId, string AppTokenId, string MessageTitle, string MessageText)
        {
            var pclient = new Pushover(AppTokenId);
            var options = new PushoverClient.Options
            {
                Recipients = UserId, //User, group or comma separated values
                Priority = Priority.High,
                Notification = NotificationSound.Classical,
                Html = false
                //Url = "http://www.google.com"
            };
            pclient.Push(MessageTitle, MessageText, options);

        }
    }
}