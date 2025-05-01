#if !UNITY_WEBGL

namespace StateMachine.AsyncEx {

	public class AsyncResult<T> {
		private TaskCompletionSource<T> _tcs = new();

		public bool Completed => _tcs.Task.IsCompleted;
		public Exception Exception => _tcs.Task.Exception;
		public T Result => _tcs.Task.Result;

		public void Reset() {
			var oldTcs = _tcs;
			oldTcs?.TrySetCanceled();

			_tcs = new TaskCompletionSource<T>();
		}

		public void SetResult(T result) {
			_tcs.TrySetResult(result);
		}

		public void FireException(Exception ex) {
			_tcs.TrySetException(ex);
		}

		public async Task<T> WaitAsync() {
			await _tcs.Task;
			return _tcs.Task.Result;
		}

		public async Task<T> WaitAsync(CancellationToken ct) {
			await _tcs.Task.WaitAsync(ct);
			return _tcs.Task.Result;
		}
	}

}

#endif