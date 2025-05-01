#if UNITY_WEBGL

using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;

namespace Shared.Framework.AsyncEx {

	public sealed class AsyncProducerConsumerQueue<T> {
		
		private readonly Queue<T> _queue;

		public AsyncProducerConsumerQueue(IEnumerable<T> collection) {
			_queue = collection == null ? new Queue<T>(16) : new Queue<T>(collection);
		}
		
		public AsyncProducerConsumerQueue() {
			_queue = new Queue<T>(16);
		}

		public UniTask EnqueueAsync(T item, CancellationToken cancellationToken = default) {
			_queue.Enqueue(item);
			return UniTask.CompletedTask;
		}

		public void Enqueue(T item, CancellationToken cancellationToken = default) {
			_queue.Enqueue(item);
		}

		private async UniTask<bool> DoOutputAvailableAsync(CancellationToken cancellationToken, bool sync) {
			while (_queue.Count == 0) await UniTask.NextFrame(cancellationToken);
			return true;
		}

		public UniTask<bool> OutputAvailableAsync(CancellationToken cancellationToken = default) => DoOutputAvailableAsync(cancellationToken, false);

		private async UniTask<T> DoDequeueAsync(CancellationToken cancellationToken, bool sync) {
			await OutputAvailableAsync(cancellationToken);
			return _queue.Dequeue();
		}
		public UniTask<T> DequeueAsync(CancellationToken cancellationToken) => DoDequeueAsync(cancellationToken, false);
		public UniTask<T> DequeueAsync() => DequeueAsync(CancellationToken.None);
	}

}

#endif