using AGTec.Common.Test;
using Microsoft.Extensions.Logging;
using Moq;
using RecipeCalculator.Engine.Formulas;
using RecipeCalculator.Engine.Function;
using RecipeCalculator.Engine.Parser;

namespace RecipeCalculator.Engine.Test;

public abstract class BaseEngineRunnerTest : BaseTestWithSut<IEngineRunner>
{
    protected IParsingContext ParsingContext { get; private set; } = null!;

    protected override void BeforeEachTest()
    {
        base.BeforeEachTest();
        ParsingContext = new DefaultParsingContext(
            new DefaultFunctionCache(),
            new DefaultFunctionResultCache(), 
            new DefaultParseTreeCache(),
            new DefaultFormulaResultCache(),
            new DefaultVariableCache());
    }

    protected override IEngineRunner CreateSut()
    {
        return new EngineRunner(ParsingContext, new Mock<ILogger<EngineRunner>>().Object);
    }
}