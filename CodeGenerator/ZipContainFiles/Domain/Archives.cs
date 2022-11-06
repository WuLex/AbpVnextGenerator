using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Blog
{
    public class Archives: AuditedAggregateRoot<Guid>,IDeletionAuditedObject
    {

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string ArchiveDate { get; set; }

    }
}