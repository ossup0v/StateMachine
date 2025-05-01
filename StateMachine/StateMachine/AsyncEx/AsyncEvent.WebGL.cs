#if UNITY_WEBGL

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Shared.Framework.AsyncEx {

	public class AsyncEvent {
		public bool Completed { get; private set; }
		public Exception Exception { get; private set; }

		public void Reset() {
			Completed = false;
			Exception = null;
		}

		public void FireEvent() {
			Completed = true;
			Exception = null;
		}

		public void FireException(Exception ex) {
			Completed = false;
			Exception = ex;
		}

		public async UniTask WaitAsync(CancellationToken ct = default) {
			while (!Completed) {
				ct.ThrowIfCancellationRequested();

				if (Exception != null) throw Exception;

				await UniTask.NextFrame(ct);
			}
		}
	}

}

#endif