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
    [ConceptKeyword("DeactivateOnDelete")]
    public class DeactivateOnDeleteInfo : IConceptInfo
    {
        [ConceptKey]
        public DeactivatableInfo Deactivatable { get; set; }
    }

    [Export(typeof(IConceptCodeGenerator))]
    [ExportMetadata(MefProvider.Implements, typeof(DeactivateOnDeleteInfo))]
    public class DeactivateOnDeleteCodeGenerator : IConceptCodeGenerator
    {
        public void GenerateCode(IConceptInfo conceptInfo, ICodeBuilder codeBuilder)
        {
            var info = (DeactivateOnDeleteInfo)conceptInfo;

            var code = String.Format(
            @"var deactivated = deleted.ToList();

            foreach(var item in deleted)
                item.Active = false;

            updated = updated.Concat(deleted).ToArray();
            updatedNew = updatedNew.Concat(deleted).ToArray();

            deleted = new Common.Queryable.{0}_{1}[]{{}};
            deletedIds = new {0}.{1}[]{{}};
            ",
                info.Deactivatable.Entity.Module.Name,
                info.Deactivatable.Entity.Name);

            codeBuilder.InsertCode(code, WritableOrmDataStructureCodeGenerator.OldDataLoadedTag, info.Deactivatable.Entity);
        }
    }
}
