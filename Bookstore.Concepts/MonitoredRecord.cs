using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

using System;
using Rhetos.Compiler;
using Rhetos.Dom.DefaultConcepts;
using Rhetos.Extensibility;

namespace Bookstore.Concepts
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("MonitoredRecords")]
    public class MonitoredRecordsInfo : IConceptInfo
    {
        [ConceptKey]
        public EntityInfo Entity { get; set; }
    }

    [Export(typeof(IConceptMacro))]
    public class MonitoredRecordsMacro : IConceptMacro<MonitoredRecordsInfo>
    {
        public IEnumerable<IConceptInfo> CreateNewConcepts(MonitoredRecordsInfo conceptInfo, IDslModel existingConcepts)
        {
            var newConcepts = new List<IConceptInfo>();

            var createdAtPropertyInfo = new DateTimePropertyInfo
            {
                Name =  "CreatedAt",
                DataStructure = conceptInfo.Entity
            };

            newConcepts.Add(createdAtPropertyInfo);
            newConcepts.Add(new CreationTimeInfo
            {
                Property = createdAtPropertyInfo,
            });
            newConcepts.Add(new DenyUserEditPropertyInfo
            {
                Property = createdAtPropertyInfo,
            });

            var loggingProperty = new EntityLoggingInfo
            {
                Entity = conceptInfo.Entity
            };

            newConcepts.Add(loggingProperty);

            newConcepts.Add(new AllPropertiesLoggingInfo
            {
                EntityLogging = loggingProperty
            });

            return newConcepts;
        }
    }
}
