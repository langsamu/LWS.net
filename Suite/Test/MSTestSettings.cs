using Test;

[assembly: Parallelize(Scope = ExecutionScope.MethodLevel)]

// Starts DI
[assembly: AssemblyFixtureProvider(typeof(SuiteApplication))]
