//using Model;
//using Model.EARL;
//using System.Reflection;

//namespace Test;

//[AttributeUsage(AttributeTargets.Method)]
//public sealed class SomethingAttribute : Attribute, ITestDataSource
//{
//    IEnumerable<object?[]> ITestDataSource.GetData(MethodInfo methodInfo) =>
//        from assertion in Executor.Execute(Resources.ManifestGraph).Assertions
//        select (object?[])[
//            new TestDataRow<Assertion>(assertion)
//            {
//                DisplayName = assertion.Test.Title,
//                //TestCategories = [.. entry.Traits.Select(t => t.ToString())],
//                //IgnoreMessage =assertion.Result entry.Status == Status.Pending ? "Test ignored due to pending status" : null, // TODO: What's the real logic?
//            }
//        ];

//    string? ITestDataSource.GetDisplayName(MethodInfo methodInfo, object?[]? data) => null;
//}
