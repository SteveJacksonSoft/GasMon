using System;
using System.Threading.Tasks;
using System.Timers;
using GasMonPersonal.AWS;
using GasMonPersonal.Models;

namespace GasMonPersonal.NotificationListening
{
    public class NotificationManager : IAsyncDisposable
    {
        private const int NotificationPollIntervalInMs = 500;
        
        private string _currentQueueUrl;
        private string _currentSubscriptionArn;

        private AwsApiClient _awsClient;

        private Timer _messagePollingTimer;

        private readonly Action<GasMessage> _messageProcessingAction;

        public NotificationManager(AwsApiClient awsClient, Action<GasMessage> messageProcessAction = null)
        {
            _awsClient = awsClient;

            _messageProcessingAction = messageProcessAction ?? Console.WriteLine;
        }

        public async Task StartProcessingGasNotifications()
        {
            if (this._currentQueueUrl == null)
            {
                this._currentQueueUrl = await this._awsClient.CreateQueueAndReturnUrl();
                this._currentSubscriptionArn = await _awsClient.SubscribeQueueToGasNotificationTopic(this._currentQueueUrl);

                ProcessNewNotificationsRegularly(NotificationPollIntervalInMs);
            }
        }

        public async ValueTask DisposeAsync()
        {
            _messagePollingTimer.Dispose();
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
                _messageProcessingAction.Invoke(NotificationReading.ExtractMessage(message));
            }
        }

        private async Task RemoveQueue()
        {
            Console.WriteLine("Removing queue");
            await _awsClient.UnsubscribeQueue(_currentSubscriptionArn);
            await _awsClient.DeleteQueue(_currentQueueUrl);
        }
    }
}