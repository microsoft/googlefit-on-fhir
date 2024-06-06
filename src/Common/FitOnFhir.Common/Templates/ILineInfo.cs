﻿// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

namespace Microsoft.Health.FitOnFhir.Common.Templates
{
    public interface ILineInfo
    {
        public int LineNumber { get; set; }

        public int LinePosition { get; set; }

        public bool HasLineInfo();
    }
}
