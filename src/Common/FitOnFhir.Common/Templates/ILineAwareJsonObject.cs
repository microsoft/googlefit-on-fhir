// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.FitOnFhir.Common.Templates
{
    public interface ILineAwareJsonObject : ILineInfo
    {
#pragma warning disable CA2227 // Collection properties should be read only
        IDictionary<string, LineInfo> LineInfoForProperties { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only
    }
}
