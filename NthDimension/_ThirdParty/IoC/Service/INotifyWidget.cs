using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NthDimension.Service
{
    public interface INotifyWidget
    {
        event Action<INotifyWidget, NotificationEventArgs> OnNotify;
    }

    public class NotificationEventArgs : EventArgs
    {

        private string m_notificationText;
        private NotificationType m_notificationType;

        public NotificationEventArgs(NotificationType type, string text)
        {
            this.m_notificationType = type;
            this.m_notificationText = text;
        }

        public NotificationType Type
        {
            get { return m_notificationType; }
            set { m_notificationType = value; }
        }
        public string Text
        {
            get { return m_notificationText; }
            set { m_notificationText = value; }
        }
    }

    public enum NotificationType
    {
        @Notice,
        @Success,
        @Warning,
        @Error
    }
}
