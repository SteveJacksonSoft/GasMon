using System;
using System.Threading.Tasks;
using System.Timers;
using GasMonPersonal.AWS;

namespace GasMonPersonal.GasListening
{
    public class NotificationManager : IDisposable
    {
        private const int NotificationPollIntervalInMs = 10000;
        private const int DeleteQueueTimeoutInSeconds = 120;
        
        private string _currentQueueUrl;
        private string _currentSubscriptionArn;

        private AwsApiClient _awsClient;

        private Timer _messagePollingTimer;
        private Timer _queueDeletionTimer;

        public Action<string> MessageProcessingAction { private get; set; } = Console.WriteLine;

        public NotificationManager(AwsApiClient awsClient)
        {
            _awsClient = awsClient;
        }

        public async Task StartProcessingGasNotifications()
        {
            if (this._currentQueueUrl == null)
            {
                this._currentQueueUrl = await this._awsClient.CreateQueueAndReturnUrl();
                this._currentSubscriptionArn = await _awsClient.SubscribeQueueToGasNotificationTopic(this._currentQueueUrl);

                ProcessNewNotificationsRegularly(NotificationPollIntervalInMs);
                RemoveQueueAfterSeconds(DeleteQueueTimeoutInSeconds);
            }
        }

        public async void Dispose()
        {
            _messagePollingTimer.Dispose();
            _queueDeletionTimer.Dispose();
            await RemoveQueue();
        }

        private void ProcessNewNotificationsRegularly(int intervalInMs)
        {
            _messagePollingTimer = new Timer
            {
                Interval = intervalInMs,
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

        private void RemoveQueueAfterSeconds(int numSeconds)
        {
            _queueDeletionTimer = new Timer
            {
                Interval = numSeconds,
                AutoReset = false,
            };
            _queueDeletionTimer.Elapsed += (sender, args) => RemoveQueue();
        }

        private async Task RemoveQueue()
        {
            await _awsClient.UnsubscribeQueue(_currentSubscriptionArn);
            await _awsClient.DeleteQueue(_currentQueueUrl);
        }
    }
}