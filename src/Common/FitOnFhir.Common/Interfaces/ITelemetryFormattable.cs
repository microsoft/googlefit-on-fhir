// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Health.FitOnFhir.Common.Telemetry;

namespace Microsoft.Health.FitOnFhir.Common.Interfaces
{
    public interface ITelemetryFormattable
    {
        Metric ToMetric { get; }
    }
}
