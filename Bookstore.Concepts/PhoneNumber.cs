using System.Collections.Generic;
using System.ComponentModel.Composition;
using Rhetos.Dsl;
using Rhetos.Dsl.DefaultConcepts;

namespace Bookstore.Concepts
{
    [Export(typeof(IConceptInfo))]
    [ConceptKeyword("PhoneNumber")]
    public class PhoneNumberInfo : ShortStringPropertyInfo
    {
    }

    [Export(typeof(IConceptMacro))]
    public class PhoneNumberMacro : IConceptMacro<PhoneNumberInfo>
    {
        public IEnumerable<IConceptInfo> CreateNewConcepts(PhoneNumberInfo conceptInfo, IDslModel existingConcepts)
        {
            var newConcepts = new List<IConceptInfo>();

            if (conceptInfo.DataStructure is IWritableOrmDataStructure) // Activate validation only on writable data, for example on Entity.
                newConcepts.Add(new RegExMatchInfo // Effect is same as adding "RegExMatch" validation on this property in DSL script.
                {
                    Property = conceptInfo,
                    RegularExpression = @"[+]*[(]{0,1}[0-9]{1,4}[)]{0,1}[-\s\./0-9]*",
                    ErrorMessage = "Invalid phone number format."
                });

            return newConcepts;
        }
    }
}
