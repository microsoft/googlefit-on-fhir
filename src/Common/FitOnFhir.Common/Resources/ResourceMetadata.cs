// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Microsoft.Health.FitOnFhir.Common.Interfaces;

namespace Microsoft.Health.FitOnFhir.Common.Resources
{
#pragma warning disable CA1815
    public struct ResourceMetadata : IResourceMetadata
#pragma warning restore CA1815
    {
        public string Id { get; set; }

        public string VersionId { get; set; }

        public DateTime? LastUpdated { get; set; }
    }
}
