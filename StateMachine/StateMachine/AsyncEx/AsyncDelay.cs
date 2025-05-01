namespace StateMachine.AsyncEx {

	public static class AsyncDelay {
		public static async Task Delay(int millisecondsDelay, CancellationToken cancellationToken = default) {
			await Task.Delay(millisecondsDelay, cancellationToken);
		}

		public static async Task Delay(TimeSpan delay, CancellationToken cancellationToken = default) {
			await Task.Delay(delay, cancellationToken);
		}
	}

}