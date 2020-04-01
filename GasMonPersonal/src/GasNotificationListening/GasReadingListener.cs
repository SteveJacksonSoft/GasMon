using System;
using System.Threading.Tasks;
using System.Timers;
using GasMonPersonal.AWS;
using GasMonPersonal.Models;

namespace GasMonPersonal.GasNotificationListening
{
    public class GasReadingListener : IAsyncDisposable
    {
        private const int NotificationPollIntervalInMs = 500;
        
        private string _currentQueueUrl;
        private string _currentSubscriptionArn;

        private Timer _messagePollingTimer;

        private readonly Action<GasReading> _messageProcessingAction;

        public GasReadingListener(Action<GasReading> messageProcessAction = null)
        {
            _messageProcessingAction = messageProcessAction ?? Console.WriteLine;
        }

        public async Task StartListeningForGasReadings()
        {
            if (this._currentQueueUrl == null)
            {
                this._currentQueueUrl = await AwsService.CreateQueueAndReturnUrl();
                this._currentSubscriptionArn = await AwsService.SubscribeQueueToGasNotificationTopic(this._currentQueueUrl);

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
            var nextMessages = await AwsService.PopNextQueueMessages(_currentQueueUrl);
            foreach (var message in nextMessages)
            {
                _messageProcessingAction.Invoke(GasNotificationParsing.ExtractReading(message));
            }
        }

        private async Task RemoveQueue()
        {
            Console.WriteLine("Removing queue");
            await AwsService.UnsubscribeQueue(_currentSubscriptionArn);
            await AwsService.DeleteQueue(_currentQueueUrl);
        }
    }
}