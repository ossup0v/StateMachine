#if UNITY_WEBGL

using System;
using System.Threading;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;

namespace Shared.Framework.AsyncEx {

	public sealed class AsyncLock {
		private int _lockCount;
		public bool Taken => _lockCount > 0;

		public async Task<IDisposable> LockAsync(CancellationToken cancellationToken = default) {
			while (Taken) await UniTask.NextFrame(cancellationToken);

			++_lockCount;
			return new Key(this);
		}

		public IDisposable Lock(CancellationToken cancellationToken = default) {
			++_lockCount;
			return new Key(this);
		}

		private void Release() {
			_lockCount = Math.Max(0, _lockCount - 1);
		}

		private sealed class Key : IDisposable {
			private AsyncLock _asyncLock;

			public Key(AsyncLock asyncLock) {
				_asyncLock = asyncLock;
			}

			public void Dispose() {
				_asyncLock?.Release();
				_asyncLock = null;
			}
		}
	}

}

#endif