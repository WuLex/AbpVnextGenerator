using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Blog
{
    public class Tags: AuditedAggregateRoot<Guid>,IDeletionAuditedObject
    {

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string TagName { get; set; }

    }
}