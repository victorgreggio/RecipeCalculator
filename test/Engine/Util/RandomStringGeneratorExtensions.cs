using AGTec.Common.Randomizer.ReferenceTypes;

namespace RecipeCalculator.Engine.Test.Util;

public static class RandomStringGeneratorExtensions
{
    private static readonly char[] SkipList =
        { '%', '^', '&', '*', '(', ')', '-', '+', '=', '{', '}', '[', ']', '/', '\'', '<', '>', '|', '\\' };

    public static string GenerateValueWithoutSpecialChar(this IRandomAlphanumericString stringGenerator, int length = 25)
    {
        return stringGenerator.GenerateApartFrom(length, SkipList);
    }
}