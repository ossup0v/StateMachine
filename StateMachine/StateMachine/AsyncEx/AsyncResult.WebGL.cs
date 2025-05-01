#if UNITY_WEBGL

using System;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Shared.Framework.AsyncEx {

	public class AsyncResult<T> {
		public bool Completed { get; private set; }
		public Exception Exception { get; private set; }
		public T Result { get; private set; }

		public void Reset() {
			Result = default;
			Completed = false;
			Exception = null;
		}

		public void SetResult(T result) {
			Result = result;
			Completed = true;
			Exception = null;
		}

		public void FireException(Exception ex) {
			Completed = false;
			Exception = ex;
		}

		public async UniTask<T> WaitAsync(CancellationToken ct = default) {
			while (!Completed) {
				ct.ThrowIfCancellationRequested();

				if (Exception != null) throw Exception;

				await UniTask.NextFrame(ct);
			}

			return Result;
		}
	}

}

#endif