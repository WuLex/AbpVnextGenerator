using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Blog
{
    public class __EFMigrationsHistory: AuditedAggregateRoot<Guid>,IDeletionAuditedObject
    {

        /// <summary>
        /// 
        /// </summary>
        public string MigrationId { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ProductVersion { get; set; }

    }
}