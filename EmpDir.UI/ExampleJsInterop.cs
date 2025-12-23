//using Microsoft.JSInterop;

//namespace EmpDir.UI
//{
    
//    // This class can be registered as scoped DI service and then injected into Blazor
//    // components for use.

//    public class ExampleJsInterop : IAsyncDisposable
//    {
//        private readonly Lazy<Task<IJSObjectReference>> moduleTask;

//        public ExampleJsInterop(IJSRuntime jsRuntime)
//        {
//            moduleTask = new(() => jsRuntime.InvokeAsync<IJSObjectReference>(
//                "import", "./_content/EmpDir.UI/exampleJsInterop.js").AsTask());
//        }

//        public async ValueTask<string> Prompt(string message)
//        {
//            var module = await moduleTask.Value;
//            return await module.InvokeAsync<string>("showPrompt", message);
//        }

//        public async ValueTask DisposeAsync()
//        {
//            if (moduleTask.IsValueCreated)
//            {
//                var module = await moduleTask.Value;
//                await module.DisposeAsync();
//            }
//        }
//    }
//}
