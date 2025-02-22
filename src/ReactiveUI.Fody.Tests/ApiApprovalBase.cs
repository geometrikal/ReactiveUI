﻿// Copyright (c) 2023 .NET Foundation and Contributors. All rights reserved.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for full license information.

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

using PublicApiGenerator;

using VerifyTests;

using VerifyXunit;

/// <summary>
/// Tests for API approvals.
/// </summary>
[ExcludeFromCodeCoverage]
#pragma warning disable CA1050 // Declare types in namespaces
#pragma warning disable RCS1110 // Declare type inside namespace.
public abstract class ApiApprovalBase
#pragma warning restore RCS1110 // Declare type inside namespace.
#pragma warning restore CA1050 // Declare types in namespaces
{
    /// <summary>
    /// Checks the assembly to detect the public API. Generates a received/approved version of the API.
    /// </summary>
    /// <param name="assembly">The assembly to check.</param>
    /// <param name="filePath">Auto populated file path.</param>
    /// <returns>A task to monitor the progress.</returns>
    protected static Task CheckApproval(Assembly assembly, [CallerFilePath]string? filePath = null)
    {
        if (filePath is null)
        {
            return Task.CompletedTask;
        }

        var generatorOptions = new ApiGeneratorOptions { AllowNamespacePrefixes = new[] { "ReactiveUI" } };
        var apiText = assembly.GeneratePublicApi(generatorOptions);
        var verifySettings = new VerifySettings();
        return Verifier.Verify(apiText, verifySettings, filePath)
            .UniqueForRuntimeAndVersion()
            .ScrubEmptyLines()
            .ScrubLines(l =>
                l.StartsWith("[assembly: AssemblyVersion(", StringComparison.InvariantCulture) ||
                l.StartsWith("[assembly: AssemblyFileVersion(", StringComparison.InvariantCulture) ||
                l.StartsWith("[assembly: AssemblyInformationalVersion(", StringComparison.InvariantCulture) ||
                l.StartsWith("[assembly: System.Reflection.AssemblyMetadata(", StringComparison.InvariantCulture));
    }
}
