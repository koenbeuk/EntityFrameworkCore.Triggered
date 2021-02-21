using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntityFrameworkCore.Triggered.Lifecycles;

namespace EntityFrameworkCore.Triggered.Extensions.Tests
{
    public class SampleTrigger : Trigger<object>, IBeforeSaveStartingTrigger
    {
        public int BeforeSaveCalls;
        public int BeforeSaveAsyncCalls;

        public int AfterSaveCalls;
        public int AfterSaveAsyncCalls;

        public int AfterSaveFailedCalls;
        public int AfterSaveFailedAsyncCalls;

        public int BeforeSaveStartingTriggerCalls;


#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        public override void BeforeSave(ITriggerContext<object> context) => BeforeSaveCalls += 1;
        public override async Task BeforeSave(ITriggerContext<object> context, CancellationToken cancellationToken) => BeforeSaveAsyncCalls += 1;

        public override void AfterSave(ITriggerContext<object> context) => AfterSaveCalls += 1;
        public override async Task AfterSave(ITriggerContext<object> context, CancellationToken cancellationToken) => AfterSaveAsyncCalls += 1;

        public override void AfterSaveFailed(ITriggerContext<object> context, Exception exception) => AfterSaveFailedCalls += 1;
        public override async Task AfterSaveFailed(ITriggerContext<object> context, Exception exception, CancellationToken cancellationToken) => AfterSaveFailedAsyncCalls += 1;
        public async Task BeforeSaveStarting(CancellationToken cancellationToken) => BeforeSaveStartingTriggerCalls += 1;
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
    }
}
