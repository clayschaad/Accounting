using System.Collections.Generic;

namespace Schaad.Accounting.Datasets
{
    public class MessageDataset
    {
        public string Title { get; set; }

        public List<string> Lines { get; set; }

        public MessageStatus Status { get; set; }

        public MessageDataset(string title, MessageStatus status = MessageStatus.Success)
        {
            Title = title;
            Status = status;
            Lines = new List<string>();
        }

        public void Add(string message, MessageStatus status = MessageStatus.Success)
        {
            Lines.Add(message);
            if (status > Status)
            {
                Status = status;
            }
        }

        public string GetCssAlertClass()
        {
            switch (Status)
            {
                case MessageStatus.Success: return "alert-success";
                case MessageStatus.Info: return "alert-info";
                case MessageStatus.Warning: return "alert-warning";
                case MessageStatus.Error: return "alert-danger";
                default: return "alert-success";
            }
        }
    }

    public enum MessageStatus
    {
        Success = 0,
        Info = 1,
        Warning = 2,
        Error = 3
    }
}