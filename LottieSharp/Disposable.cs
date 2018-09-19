using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LottieSharp
{
    public class Disposable : IDisposable
    {
        private Action _action;
        private bool _isDisposed = false;

        public Disposable(Action action)
        {
            _action = action;
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            _action?.Invoke();
            _isDisposed = true;
        }
    }
}
