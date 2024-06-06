// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.FitOnFhir.Common.Interfaces
{
    public interface IResourceMetadata
    {
        string Id { get; }

        string VersionId { get; }

        DateTime? LastUpdated { get; }
    }
}
