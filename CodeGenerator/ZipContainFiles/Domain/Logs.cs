using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities.Auditing;

namespace Blog
{
    public class Logs: AuditedAggregateRoot<Guid>,IDeletionAuditedObject
    {

        /// <summary>
        /// 
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Thread { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Level { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Exception { get; set; }

    }
}