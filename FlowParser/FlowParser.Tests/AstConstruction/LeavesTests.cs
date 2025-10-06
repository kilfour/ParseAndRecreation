using FlowParser.AstConstruction.AstNodes;

namespace FlowParser.Tests.AstConstruction;

public class LeavesTests
{
    [Fact]
    public void Single_number()
    {
        var ast = LostIn.Translation("42");
        var node = Assert.IsType<Number>(ast);
        Assert.Equal(42, node.Value);
    }
}
