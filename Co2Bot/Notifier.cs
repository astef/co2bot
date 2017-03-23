using System;
namespace Co2Bot
{
    public class Notifier
    {
        private readonly int _shortRepeatCount;
        private readonly TimeSpan _longRepeatDelay;

        private DateTime _lastSend;
        private int _warnCount;
        private State _state;

        public event Action<string> Message;

        public Notifier(int shortRepeatCount, TimeSpan longRepeatDelay)
        {
            if (shortRepeatCount <= 1)
                throw new ArgumentOutOfRangeException(nameof(shortRepeatCount));
            if (longRepeatDelay <= TimeSpan.FromMinutes(1))
                throw new ArgumentOutOfRangeException(nameof(longRepeatDelay));
            _shortRepeatCount = shortRepeatCount;
            _longRepeatDelay = longRepeatDelay;
        }

        public void Warn(string msg)
        {
            if (_state == State.Ok)
            {
                OnWarn(msg);
            }
            else if (_state == State.Warn)
            {
                OnWarn(msg);
            }
            else if (_state == State.LongWarn)
            {
                OnLongWarn(msg);
            }
        }

        public void Ok(string msg)
        {
            if (_state != State.Ok)
            {
                OnOk(msg);
            }
        }

        private void OnOk(string msg)
        {
            _state = State.Ok;
            _warnCount = 0;
            Send(msg);
        }

        private void OnLongWarn(string msg)
        {
            if (DateTime.Now - _lastSend > _longRepeatDelay)
            {
                Send(msg);
            }
        }

        private void OnWarn(string msg)
        {
            if (_warnCount < _shortRepeatCount)
            {
                _state = State.Warn;
                _warnCount++;
                Send(msg);
            }
            else
            {
                _state = State.LongWarn;
                Send($"Всё, не буду спамить. Если ничего не изменится, напомню через {_longRepeatDelay.TotalMinutes} минут");
            }
        }

        private void Send(string msg)
        {
            if (Message != null)
            {
                Message(msg);
                _lastSend = DateTime.Now;
            }
        }

        enum State
        {
            Ok,
            Warn,
            LongWarn
        }
    }
}