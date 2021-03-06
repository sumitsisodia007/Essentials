﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Xamarin.Essentials
{
    public static partial class Email
    {
        public static Task ComposeAsync()
            => ComposeAsync(null);

        public static Task ComposeAsync(string subject, string body, params string[] to)
            => ComposeAsync(new EmailMessage(subject, body, to));

        public static Task ComposeAsync(EmailMessage message)
        {
            if (!IsComposeSupported)
                throw new FeatureNotSupportedException();

            return PlatformComposeAsync(message);
        }

        static string GetMailToUri(EmailMessage message)
        {
            if (message != null && message.BodyFormat != EmailBodyFormat.PlainText)
                throw new FeatureNotSupportedException("Only EmailBodyFormat.PlainText is supported if no email account is set up.");

            var parts = new List<string>();
            if (!string.IsNullOrEmpty(message?.Body))
                parts.Add("body=" + Uri.EscapeUriString(message.Body));
            if (!string.IsNullOrEmpty(message?.Subject))
                parts.Add("subject=" + Uri.EscapeUriString(message.Subject));
            if (message?.To.Count > 0)
                parts.Add("to=" + string.Join(",", message.To));
            if (message?.Cc.Count > 0)
                parts.Add("cc=" + string.Join(",", message.Cc));
            if (message?.Bcc.Count > 0)
                parts.Add("bcc=" + string.Join(",", message.Bcc));

            var uri = "mailto:";
            if (parts.Count > 0)
                uri += "?" + string.Join("&", parts);
            return uri;
        }
    }

    public class EmailMessage
    {
        public EmailMessage()
        {
        }

        public EmailMessage(string subject, string body, params string[] to)
        {
            To = to?.ToList() ?? new List<string>();
        }

        public string Subject { get; set; }

        public string Body { get; set; }

        public EmailBodyFormat BodyFormat { get; set; }

        public List<string> To { get; set; } = new List<string>();

        public List<string> Cc { get; set; } = new List<string>();

        public List<string> Bcc { get; set; } = new List<string>();
    }

    public enum EmailBodyFormat
    {
        PlainText,
        Html
    }
}
