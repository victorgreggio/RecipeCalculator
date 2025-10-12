using System.Text.RegularExpressions;
using Antlr4.Runtime.Tree;
using RecipeCalculator.Common.Function;
using RecipeCalculator.Common.Values;
using RecipeCalculator.Engine.Exceptions;

namespace RecipeCalculator.Engine.Parser;

internal class EvalVisitor : FormulaBaseVisitor<IValue>
{
    private readonly IParsingContext _parsingContext;
    
    public EvalVisitor(IParsingContext parsingContext)
    {
        _parsingContext = parsingContext;
    }
    
    public override IValue VisitBlock(FormulaParser.BlockContext context)
    {
	    FormulaParser.IfStatementContext ifStmt;
	    if ((ifStmt = context.ifStatement()) != null)
	    {
		    return Visit(ifStmt);
	    }

	    FormulaParser.ExpressionContext ex;
	    if ((ex = context.expression()) != null)
	    {
		    return Visit(ex);
	    }

	    FormulaParser.ErrorFunctionCallContext errFunc;
	    if ((errFunc = context.errorFunctionCall()) != null)
	    {
		    return Visit(errFunc);
	    }

	    throw new InvalidOperationException();
    }

    public override IValue VisitErrorFunctionCall(FormulaParser.ErrorFunctionCallContext context)
    {
	    const string message = "Calculation was cancelled by a call of the error function with";
	    var error = Visit(context.expression());
	    if (error.Is<string>()) throw new ErrorCallException($"{message} message '{error.As<string>()}'");
	    throw new ErrorCallException($"{message} code '{error.As<double>()}'");
    }

    public override IValue VisitMaxFunctionCall(FormulaParser.MaxFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<double>())
		    throw new EvalException("Illegal argument type for first parameter of function 'Max', number expected", context);
        
	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<double>())
		    throw new EvalException("Illegal argument type for second parameter of function 'Max', number expected", context);
        
	    return new Value(Math.Max(firstParam.As<double>(), secondParam.As<double>()));
    }

    public override IValue VisitMinFunctionCall(FormulaParser.MinFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<double>())
		    throw new EvalException("Illegal argument type for first parameter of function 'Min', number expected", context);
        
	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<double>())
		    throw new EvalException("Illegal argument type for second parameter of function 'Min', number expected", context);

	    return new Value(Math.Min(firstParam.As<double>(), secondParam.As<double>()));
    }

    public override IValue VisitRoundFunctionCall(FormulaParser.RoundFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<double>())
		    throw new EvalException("Illegal argument type for first parameter of function 'Rnd', number expected", context);
        
	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<double>())
		    throw new EvalException("Illegal argument type for second parameter of function 'Rnd', number expected", context);

	    return new Value(Math.Round(firstParam.As<double>(), Convert.ToInt32(secondParam.As<double>())));
    }

    public override IValue VisitCeilFunctionCall(FormulaParser.CeilFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression());
        
	    if (!firstParam.Is<double>())
		    throw new EvalException("Illegal argument type for first parameter of function 'Ceil', number expected", context);

	    return new Value(Math.Ceiling(firstParam.As<double>()));
    }

    public override IValue VisitFloorFunctionCall(FormulaParser.FloorFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression());
        
	    if (!firstParam.Is<double>())
		    throw new EvalException("Illegal argument type for first parameter of function 'Floor', number expected", context);
	    
	    return new Value(Math.Floor(firstParam.As<double>()));
    }

    public override IValue VisitExpFunctionCall(FormulaParser.ExpFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression());
	    if (!firstParam.Is<double>())
		    throw new EvalException("Illegal argument type for first parameter of function 'Exp', number expected", context);
	    
	    return new Value(Math.Exp(firstParam.As<double>()));
    }

    public override IValue VisitDayFunctionCall(FormulaParser.DayFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression());
	    if (!firstParam.Is<string>()) {
		    throw new EvalException("Illegal argument type for first parameter of function 'Day', string expected", context);
	    }

	    if (!DateTime.TryParse(firstParam.As<string>(), out var date))
		    throw new EvalException($"Cannot convert first parameter of function 'Day' to date: '{firstParam.As<string>()}'", context);
        
	    return new Value(date.Day);
    }

    public override IValue VisitMonthFunctionCall(FormulaParser.MonthFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression());
	    if (!firstParam.Is<string>()) {
		    throw new EvalException("Illegal argument type for first parameter of function 'Month', string expected", context);
	    }

	    if (!DateTime.TryParse(firstParam.As<string>(), out var date))
		    throw new EvalException($"Cannot convert first parameter of function 'Month' to date: '{firstParam.As<string>()}'", context);
        
	    return new Value(date.Month);
    }

    public override IValue VisitYearFunctionCall(FormulaParser.YearFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression());
	    if (!firstParam.Is<string>())
		    throw new EvalException("Illegal argument type for first parameter of function 'Year', string expected",
			    context);

	    if (!DateTime.TryParse(firstParam.As<string>(), out var date))
		    throw new EvalException(
			    $"Cannot convert first parameter of function 'Year' to date: '{firstParam.As<string>()}'", context);

	    return new Value(date.Year);
    }

    public override IValue VisitSubstrFunctionCall(FormulaParser.SubstrFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<string>()) 
		    throw new EvalException("Illegal argument type for first parameter of function 'SubStr', string expected", context);
        
	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<double>()) 
		    throw new EvalException("Illegal argument type for second parameter of function 'SubStr', number expected", context);
        
	    var thirdParam = Visit(context.expression(2));
	    if (!thirdParam.Is<double>()) 
		    throw new EvalException("Illegal argument type for third parameter of function 'SubStr', number expected", context);

	    var subStringValue = firstParam.As<string>().Substring(Convert.ToInt32(secondParam.As<double>()),
		    Convert.ToInt32(thirdParam.As<double>()));
	    return new Value(subStringValue);
    }

    public override IValue VisitAddDaysFunctionCall(FormulaParser.AddDaysFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<string>()) 
		    throw new EvalException("Illegal argument type for first parameter of function 'AddDays', string expected", context);
        
	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<double>()) 
		    throw new EvalException("Illegal argument type for second parameter of function 'AddDays', number expected", context);
        
	    if (!DateTime.TryParse(firstParam.As<string>(), out var date))
		    throw new EvalException($"Cannot convert first parameter of function 'AddDays' to date: '{firstParam.As<string>()}'", context);

	    var newDateValue = date.AddDays(secondParam.As<double>()).ToString("s"); 
	    return new Value(newDateValue);
    }

    public override IValue VisitGetDiffDaysFunctionCall(FormulaParser.GetDiffDaysFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<string>())
		    throw new EvalException("Illegal argument type for first parameter of function 'GetDiffDays', string expected", context);

	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<string>())
		    throw new EvalException(
			    "Illegal argument type for second parameter of function 'GetDiffDays', string expected", context);

	    if (!DateTime.TryParse(firstParam.As<string>(), out var firstDate))
		    throw new EvalException(
			    $"Cannot convert first parameter of function 'GetDiffDays' to date: '{firstParam.As<string>()}'", context);

	    if (!DateTime.TryParse(secondParam.As<string>(), out var secondDate))
		    throw new EvalException(
			    $"Cannot convert second parameter of function 'GetDiffDays' to date: '{secondParam.As<string>()}'", context);

	    var totalDays = (firstDate - secondDate).TotalDays;
        
	    return new Value(totalDays);
    }

    public override IValue VisitPaddedStringFunctionCall(FormulaParser.PaddedStringFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<string>())
		    throw new EvalException(
			    "Illegal argument type for first parameter of function 'PaddedString', string expected", context);

	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<double>())
		    throw new EvalException(
			    "Illegal argument type for first parameter of function 'PaddedString', number expected", context);

	    var paddedString = firstParam.As<string>().PadLeft(Convert.ToInt32(secondParam.As<double>()), '0');
	    return new Value(paddedString);
    }

    public override IValue VisitDifferenceInMonthsFunctionCall(
	    FormulaParser.DifferenceInMonthsFunctionCallContext context)
    {
	    var firstParam = Visit(context.expression(0));
	    if (!firstParam.Is<string>())
		    throw new EvalException("Illegal argument type for first parameter of function 'DifferenceInMonths', string expected", context);

	    var secondParam = Visit(context.expression(1));
	    if (!secondParam.Is<string>())
		    throw new EvalException(
			    "Illegal argument type for second parameter of function 'DifferenceInMonths', number expected", context);

	    if (!DateTime.TryParse(firstParam.As<string>(), out var firstDate))
		    throw new EvalException(
			    $"Cannot convert first parameter of function 'DifferenceInMonths' to date: '{firstParam.As<string>()}'", context);

	    if (!DateTime.TryParse(secondParam.As<string>(), out var secondDate))
		    throw new EvalException(
			    $"Cannot convert second parameter of function 'DifferenceInMonths' to date: '{secondParam.As<string>()}'", context);

	    var totalMonths = (firstDate.Year - secondDate.Year) * 12 + firstDate.Month - secondDate.Month;
        
	    return new Value(Math.Abs(totalMonths));
    }

    public override IValue VisitGetOutputFromFunctionCall(FormulaParser.GetOutputFromFunctionCallContext context)
    {
	    var formulaName = Visit(context.expression());
	    if (!formulaName.Is<string>())
		    throw new EvalException("Illegal argument type for first parameter of function 'GetOutputFrom', string expected", context);

	    var output = _parsingContext.FormulaResultCache.Get(formulaName.As<string>());
	    
	    if (output is null)
		    throw new CalculatorException(ErrorType.InputParameterMissing, $"Missing output from formula '{formulaName}'");
	    
	    return output;
    }

	public override IValue VisitIdentifierFunctionCall(FormulaParser.IdentifierFunctionCallContext context)
	{
		var functionName = context.Identifier().GetText();
		var functionParameters = context.exprList() != null
			? context.exprList().expression()
			: Enumerable.Empty<FormulaParser.ExpressionContext>().ToArray();
		
		var functionId = FunctionIdBuilder.Create(functionName, functionParameters.Length);
		var function = _parsingContext.FunctionCache.Get(functionId);

		if (function == null)
			throw new Exception(
				$"Function '{functionName}' with {functionParameters.Length} parameters not defined");

		var cachedResult = _parsingContext.FunctionResultCache.Get(function);
		if (cachedResult != null) return cachedResult;
		
		var evalVisitor = new EvalVisitor(_parsingContext);
		var paramValues = functionParameters.Select(p => evalVisitor.Visit(p)).ToArray();
		try
		{
			function.Params = paramValues;
			var result = function.Execute();
			_parsingContext.FunctionResultCache.Set(function, result);
			return result;
		}
		catch (CalculatorException)
		{
			throw;
		}
		catch (Exception e)
		{
			throw new EvalException(context, e);
		}
	}

	public override IValue VisitIfStatement(FormulaParser.IfStatementContext context)
	{
		// if ...
		var ifExpr = Visit(context.ifStat().expression());
		if (ifExpr.As<bool>()) {
			return Visit(context.ifStat().block());
		}

		// else if ...
		for (var i = 0; i < context.elseIfStat().Length; i++) {
			var elseIfExpr = Visit(context.elseIfStat(i).expression());
			if (elseIfExpr.As<bool>()) {
				return Visit(context.elseIfStat(i).block());
			}
		}

		// else ...
		if (context.elseStat() != null) {
			return Visit(context.elseStat().block());
		}

		return VisitChildren(context);
	}
	
	public override IValue VisitLtExpression(FormulaParser.LtExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() < secondParam.As<double>());

		if (firstParam.Is<string>() && secondParam.Is<string>())
			return new Value(string.Compare(firstParam.As<string>(), secondParam.As<string>(), StringComparison.Ordinal) < 0);

		throw new EvalException(context);
	}

	public override IValue VisitGtExpression(FormulaParser.GtExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() > secondParam.As<double>());

		if (firstParam.Is<string>() && secondParam.Is<string>())
			return new Value(string.Compare(firstParam.As<string>(), secondParam.As<string>(), StringComparison.Ordinal) > 0);

		throw new EvalException(context);
	}

	public override IValue VisitBoolExpression(FormulaParser.BoolExpressionContext context)
	{
		return new Value(string.Compare(context.GetText(), "true", StringComparison.OrdinalIgnoreCase) == 0);
	}

	public override IValue VisitNotEqExpression(FormulaParser.NotEqExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));
		return new Value(!firstParam.Equals(secondParam));
	}

	public override IValue VisitModuloExpression(FormulaParser.ModuloExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() % secondParam.As<double>());

		throw new EvalException(context);
	}

	public override IValue VisitNumberExpression(FormulaParser.NumberExpressionContext context)
	{
		return new Value(Convert.ToDouble(context.GetText()));
	}

	public override IValue VisitIdentifierExpression(FormulaParser.IdentifierExpressionContext context)
	{
		return _parsingContext.VariableCache.Get(context.Identifier().GetText())!;
	}

	public override IValue VisitNotExpression(FormulaParser.NotExpressionContext context)
	{
		var value = Visit(context.expression());
		if (!value.Is<bool>()) throw new EvalException(context);
		return new Value(!value.As<bool>());
	}

	public override IValue VisitMultiplyExpression(FormulaParser.MultiplyExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));
        
		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() * secondParam.As<double>());
        
		throw new EvalException(context);
	}

	public override IValue VisitGtEqExpression(FormulaParser.GtEqExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() >= secondParam.As<double>());

		if (firstParam.Is<string>() && secondParam.Is<string>())
			return new Value(string.CompareOrdinal(firstParam.As<string>(), secondParam.As<string>()) >= 0);

		throw new EvalException(context);
	}

	public override IValue VisitDivideExpression(FormulaParser.DivideExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() / secondParam.As<double>());

		throw new EvalException(context);
	}

	public override IValue VisitOrExpression(FormulaParser.OrExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (!firstParam.Is<bool>() || !secondParam.Is<bool>()) throw new EvalException(context);
        
		return new Value(firstParam.As<bool>() || secondParam.As<bool>());
	}

	public override IValue VisitUnaryMinusExpression(FormulaParser.UnaryMinusExpressionContext context)
	{
		var value = Visit(context.expression());
        
		if (!value.Is<double>()) {
			throw new EvalException(context);
		}
		return new Value(-1 * value.As<double>());
	}

	public override IValue VisitPowerExpression(FormulaParser.PowerExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));
        
		if (firstParam.Is<double>() && secondParam.Is<double>()) 
			return new Value(Math.Pow(firstParam.As<double>(), secondParam.As<double>()));
        
		throw new EvalException(context);
	}

	public override IValue VisitEqExpression(FormulaParser.EqExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		return new Value(firstParam.Equals(secondParam));
	}

	public override IValue VisitAndExpression(FormulaParser.AndExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (!firstParam.Is<bool>() || !secondParam.Is<bool>())
			throw new EvalException(context);

		return new Value(firstParam.As<bool>() && secondParam.As<bool>());
	}

	public override IValue VisitStringExpression(FormulaParser.StringExpressionContext context)
	{
		var value = context.GetText();
		// first and last char are '
		var text = Regex.Replace(value.Substring(1, value.Length - 2), "\\\\(.)", "$1");
		return new Value(text);
	}

	public override IValue VisitExpressionExpression(FormulaParser.ExpressionExpressionContext context)
	{
		return Visit(context.expression());
	}

	public override IValue VisitAddExpression(FormulaParser.AddExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));
        
		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() + secondParam.As<double>());

		if (firstParam.Is<string>() || secondParam.Is<string>())
			return new Value(firstParam.Get().ToString() + secondParam.Get());

		throw new EvalException(context);
	}

	public override IValue VisitSubtractExpression(FormulaParser.SubtractExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));
		if (firstParam.Is<double>() && secondParam.Is<double>())
		{
			return new Value(firstParam.As<double>() - secondParam.As<double>());
		}

		throw new EvalException(context);
	}

	public override IValue VisitFunctionCallExpression(FormulaParser.FunctionCallExpressionContext context)
	{
		return Visit(context.functionCall());
	}

	public override IValue VisitLtEqExpression(FormulaParser.LtEqExpressionContext context)
	{
		var firstParam = Visit(context.expression(0));
		var secondParam = Visit(context.expression(1));

		if (firstParam.Is<double>() && secondParam.Is<double>())
			return new Value(firstParam.As<double>() <= secondParam.As<double>());

		if (firstParam.Is<string>() && secondParam.Is<string>())
			return new Value(string.CompareOrdinal(firstParam.As<string>(), secondParam.As<string>()) <= 0);

		throw new EvalException(context);
	}
	
	public override IValue VisitChildren(IRuleNode node)
	{
		var result = DefaultResult;
		var childCount = node.ChildCount;
		for (var i = 0; i < childCount && ShouldVisitNextChild(node, result); ++i)
		{
			if (node.GetChild(i).GetText().Equals("<EOF>")) continue; // Skip EOF to prevent NULL return
			var nextResult = node.GetChild(i).Accept(this);
			result = AggregateResult(result, nextResult);
		}
		return result;
	}
}