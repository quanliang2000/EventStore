using System;
using System.Collections.Generic;
using EventStore.Core.Data;
using EventStore.Core.Messages;
using EventStore.Core.Messaging;
using EventStore.Core.Tests.Fakes;
using EventStore.Core.Tests.Services.Replication;
using NUnit.Framework;
using DeleteStreamManager = EventStore.Core.Services.RequestManager.Managers.DeleteStream;

namespace EventStore.Core.Tests.Services.RequestManagement.DeleteMgr {
	[TestFixture]
	public class when_delete_stream_gets_commit_timeout_after_commit_replicated : RequestManagerSpecification<DeleteStreamManager> {
		protected override DeleteStreamManager OnManager(FakePublisher publisher) {
			return new DeleteStreamManager(
				publisher, 
				CommitTimeout, 
				Envelope,
				InternalCorrId,
				ClientCorrId,
				"test123",
				true,
				ExpectedVersion.Any,
				null,
				false,
				CommitSource);
		}

		protected override IEnumerable<Message> WithInitialMessages() {	
			
			yield return new StorageMessage.CommitIndexed(InternalCorrId, 1, 1, 0, 0);
		}

		protected override Message When() {
			return new StorageMessage.RequestManagerTimerTick(
				DateTime.UtcNow + TimeSpan.FromTicks(CommitTimeout.Ticks / 2));
		}

		[Test]
		public void no_messages_are_published() {
			Assert.That(Produced.Count == 0);
		}
	}
}