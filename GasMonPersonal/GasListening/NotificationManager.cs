using System;
using System.Threading.Tasks;
using System.Timers;
using GasMonPersonal.AWS;

namespace GasMonPersonal.GasListening
{
    public class NotificationManager
    {
        private const double NotificationInterval = 10000; 
        
        private string _currentQueueUrl;

        private AwsApiClient _awsClient;

        private Timer _messagePollingTimer;
        private Timer _queueDeletionTimer;
        
        public Action<string> MessageProcessingAction { private get; set; }

        public NotificationManager(AwsApiClient awsClient)
        {
            _awsClient = awsClient;
            MessageProcessingAction = Console.WriteLine;
        }

        public void Dispose()
        {
            _messagePollingTimer.Dispose();
            _queueDeletionTimer.Dispose();
        }

        public async Task StartProcessingGasNotifications()
        {
            if (this._currentQueueUrl == null)
            {
                this._currentQueueUrl = await this._awsClient.CreateQueueAndReturnUrl();
                await _awsClient.SubscribeQueueToGasNotificationTopic(this._currentQueueUrl);

                ProcessNewNotificationsRegularly();
                DeleteQueueIfNoNotificationInSeconds(30);
                DeleteQueueAfterSeconds(120);
            }
        }

        private void ProcessNewNotificationsRegularly()
        {
            _messagePollingTimer = new Timer
            {
                Interval = NotificationInterval,
                AutoReset = true,
            };
            _messagePollingTimer.Elapsed += (sender,  args) => ProcessAnyMessagesOnQueue();
            _messagePollingTimer.Start();
        }

        private async void ProcessAnyMessagesOnQueue()
        {
            var nextMessages = await _awsClient.PopNextQueueMessages(_currentQueueUrl);
            foreach (var message in nextMessages)
            {
                MessageProcessingAction.Invoke(message);
            }
        }

        private void DeleteQueueIfNoNotificationInSeconds(int numSeconds)
        {
            // TODO: Implement
        }

        private void DeleteQueueAfterSeconds(int numSeconds)
        {
            _queueDeletionTimer = new Timer
            {
                Interval = numSeconds,
                AutoReset = false,
            };
            _queueDeletionTimer.Elapsed += (sender, args) => _awsClient.DeleteQueue(_currentQueueUrl);
        }

    }
}