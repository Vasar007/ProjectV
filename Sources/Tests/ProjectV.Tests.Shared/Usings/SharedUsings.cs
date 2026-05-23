#pragma warning disable IDE0005 // Unused global usings are intentional — see remark at the end of this file.
global using System;
global using System.Collections.Generic;
global using System.Linq;
global using System.Threading.Tasks;
global using AwesomeAssertions;
global using NSubstitute;
global using NSubstitute.ExceptionExtensions;
global using NSubstitute.ReceivedExtensions;
global using ProjectV.Tests.Shared.ForTests;
global using Xunit;
#pragma warning restore IDE0005

// Remark: this file exposes global usings to every project that references
// ProjectV.Tests.Shared. Namespaces this assembly itself does not exercise
// (System.Collections.Generic, System.Threading.Tasks,
// NSubstitute.ExceptionExtensions, NSubstitute.ReceivedExtensions,
// ProjectV.Tests.Shared.ForTests) are listed because downstream test
// projects rely on them being globally available without their own
// per-csproj using directives.
