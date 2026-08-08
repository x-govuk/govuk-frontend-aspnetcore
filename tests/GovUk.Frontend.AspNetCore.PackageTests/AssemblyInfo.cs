using GovUk.Frontend.AspNetCore.PackageTests.Infrastructure;

// One sandbox and one packed package for the whole run.
[assembly: AssemblyFixture(typeof(PackageTestContext))]
