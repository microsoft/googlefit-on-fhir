// -------------------------------------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License (MIT). See LICENSE in the repo root for license information.
// -------------------------------------------------------------------------------------------------

using Azure;
using Azure.Data.Tables;

namespace FitOnFhir.Common.Models
{
    public class User : ITableEntity
    {
        public User(Guid userId)
        {
            PartitionKey = Constants.UsersPartitionKey;
            RowKey = userId.ToString();
            PlatformPartitionKeys = new List<string>();
        }

        public User()
        {
        }

        public string PartitionKey { get; set; }

        public string RowKey { get; set; }

        public DateTimeOffset? Timestamp { get; set; }

        public ETag ETag { get; set; }

        public DateTimeOffset? LastSync { get; set; }

        public List<string> PlatformPartitionKeys { get; set; }
    }
}
