using Microsoft.CodeAnalysis;
using StarFruit2;

namespace Starfruit2
{

    public class ClassBasedDescriptorMaker : DescriptorMaker
    {
        public ClassBasedDescriptorMaker(MakerConfiguration config, SemanticModel semanticModel)
            : base(config, semanticModel)
        { }



    }
}
