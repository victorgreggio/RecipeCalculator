using AGTec.Common.Base.Extensions;

namespace RecipeCalculator.Common.Function;

public static class FunctionIdBuilder
{
    public static string Create(string functionName, int numOfArgs)
    {
        return functionName.ToSnakeCase() + "_" + numOfArgs;
    }
}