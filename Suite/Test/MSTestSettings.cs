using Test;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]
[assembly: AssemblyFixtureProvider(typeof(SuiteApplication))]
