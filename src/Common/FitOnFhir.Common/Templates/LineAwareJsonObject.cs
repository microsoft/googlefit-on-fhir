// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace Microsoft.Health.FitOnFhir.Common.Templates
{
    public class LineAwareJsonObject : LineInfo, ILineAwareJsonObject
    {
        [JsonIgnore]
        public IDictionary<string, LineInfo> LineInfoForProperties { get; set; } = new Dictionary<string, LineInfo>(StringComparer.InvariantCultureIgnoreCase);
    }
}
