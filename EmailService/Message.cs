using MimeKit;
using MailKit.Net.Smtp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EmailService
{
	public class Message
	{
        public List<MailboxAddress> To { get; set; }
        public string Subject { get; set; }
        public string Content { get; set; }
		public Message(string to, string subject, string content)
		{
			To = new List<MailboxAddress>();
			string[] multi = to.Split(',');
			foreach (var item in multi)
			{
				To.Add(new MailboxAddress("email", item));
			}
			
			Subject = subject;
			Content = content;
		}
	}
}
