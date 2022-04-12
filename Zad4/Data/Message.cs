using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Zad4.Data
{
    public class Message
    {
        public int Id { get; set; }
        public string ForUser { get; set; }
        public string FromUser { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string Status { get; set; }
        public DateTime SendDate { get; set; }
        public Message(Message obj)
        {
            this.Id = obj.Id;
            this.ForUser = obj.ForUser;
            this.FromUser = obj.FromUser;
            this.Title = obj.Title;
            this.Text = obj.Text;
            this.Status = obj.Status;
            this.SendDate = obj.SendDate;
        }
        public Message() { }
    }
}
