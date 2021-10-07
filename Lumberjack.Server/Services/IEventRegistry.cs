using System;
using System.Reactive.Linq;
using System.Reactive.Subjects;

namespace Lumberjack.Server.Services
{
    public interface IEventRegistry
    {
        void On(string channel, Action<object> onNext);

        void Emit(string channel, object data);
    }

    public class EventRegistry : IEventRegistry
    {
        private readonly Subject<EventData> subject;

        public EventRegistry()
        {
            subject = new Subject<EventData>();
        }

        public void On(string channel, Action<object> onNext)
        {
            subject.Do(ev =>
            {
                if (ev.Channel == channel)
                    onNext(ev);
            }).Subscribe();
        }

        public void Emit(string channel, object data)
        {
            subject.OnNext(new EventData(channel, data));
        }
    }

    public class EventData
    {
        public EventData(string channel, object data)
        {
            Channel = channel;
            Data = data;
        }

        public string Channel { get; }
        public object Data { get; }
    }
}
