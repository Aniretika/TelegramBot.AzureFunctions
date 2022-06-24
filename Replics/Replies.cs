using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramBotFunction.Replics
{
    public static class Replies
    {
        public const string Greeting = "Welcome to Queue Manager Bot!";
        public const string Switch = "What we will do?";

        //option: add to the queue
        public const string AddToQueue = "Add to the queue";
        public const string NameAsking = "How can I name you?";
        public const string DescriptionAsking = "Now send me what do you want to print";
        public const string UsersTurn = "Status of your posititons: ";

        //option: delete from the queue
        public const string DeleteFromQueue = "Delete from the line";
        public const string SelectForDeleting = "Select item to delete:";

        //option: check status of the line
        public const string AskPositions = "Show my positions";
        public const string UserPositionsStatus = "Your positions:\n";

        //server status
        public const string AddPositionServerOk = "You have successfully joined the queue!";
        public const string AddPositionServerFailed = "Server connection error. Please, try again later.";
    }
}
