using System;

namespace LottieSharp
{
    public class Usingifier<T> : IDisposable where T : class
    {

        private readonly Action<T> _cleanUp;

        private bool _disposed;

        private T _state;

        public Usingifier(T state, Action<T> cleanUp)
        {
            _state = state;
            _cleanUp = cleanUp;
        }

        public virtual void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;

            if (_state != null)
            {
                _cleanUp(_state);
            }
        }
    }

}
