#if !UNITY_WEBGL

namespace StateMachine.AsyncEx {

	public class AsyncEvent {
		private TaskCompletionSource<bool> _tcs = new();

		public bool Completed => _tcs.Task.IsCompleted;
		public Exception Exception => _tcs.Task.Exception;

		public void Reset() {
			var oldTcs = _tcs;
			oldTcs?.TrySetCanceled();

			_tcs = new TaskCompletionSource<bool>();
		}

		public void FireEvent() {
			_tcs.TrySetResult(true);
		}

		public void FireException(Exception ex) {
			_tcs.TrySetException(ex);
		}

		public Task WaitAsync() => _tcs.Task;

		public Task WaitAsync(CancellationToken ct) => _tcs.Task.WaitAsync(ct);
	}

}

#endif