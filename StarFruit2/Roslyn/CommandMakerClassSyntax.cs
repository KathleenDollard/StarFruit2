using Microsoft.CodeAnalysis;
using StarFruit2;
using StarFruit2.Common;
using StarFruit2.Common.Descriptors;
using System.Collections.Generic;
using System.Linq;

namespace Starfruit2
{

    public class ClassSyntaxCommandMaker : DescriptorMaker
    {
        public ClassSyntaxCommandMaker(MakerConfiguration config, SemanticModel semanticModel)
            : base(config, semanticModel)
        { }



    }
}
