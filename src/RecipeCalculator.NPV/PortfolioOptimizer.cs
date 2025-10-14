using Google.OrTools.LinearSolver;

namespace RecipeCalculator.NPV;

/// <summary>
/// Optimizes project portfolio selection to maximize Adjusted NPV given a budget constraint.
/// Uses Google OR-Tools linear programming solver.
/// </summary>
public class PortfolioOptimizer
{
    public class OptimizationResult
    {
        public List<string> SelectedProjects { get; set; } = new();
        public double TotalAdjustedNPV { get; set; }
        public double TotalInvestment { get; set; }
        public double RemainingBudget { get; set; }
        public int ProjectCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public double SolveTime { get; set; }
    }

    /// <summary>
    /// Optimizes portfolio with full project data including actual initial costs
    /// </summary>
    public static OptimizationResult OptimizePortfolioWithData(
        List<(ProjectData data, ProjectResult result)> projects, 
        double budget)
    {
        var result = new OptimizationResult();
        
        // Create the linear solver
        Solver solver = Solver.CreateSolver("SCIP");
        if (solver == null)
        {
            solver = Solver.CreateSolver("CBC");
            if (solver == null)
            {
                result.Status = "No solver available!";
                return result;
            }
        }

        // Create binary decision variables for each project
        var projectVars = new Dictionary<string, Variable>();
        foreach (var (data, _) in projects)
        {
            projectVars[data.ProjectId] = solver.MakeIntVar(0, 1, data.ProjectId);
        }

        // Objective: Maximize total Adjusted NPV
        Objective objective = solver.Objective();
        foreach (var (data, projResult) in projects)
        {
            if (projResult.AdjustedNPV > 0)
            {
                objective.SetCoefficient(projectVars[data.ProjectId], projResult.AdjustedNPV);
            }
        }
        objective.SetMaximization();

        // Constraint: Total initial cost must not exceed budget
        Constraint budgetConstraint = solver.MakeConstraint(0, budget, "budget_constraint");
        foreach (var (data, _) in projects)
        {
            budgetConstraint.SetCoefficient(projectVars[data.ProjectId], data.InitialCost);
        }

        // Solve the problem
        var startTime = DateTime.UtcNow;
        Solver.ResultStatus resultStatus = solver.Solve();
        var solveTime = (DateTime.UtcNow - startTime).TotalSeconds;

        result.SolveTime = solveTime;
        result.Status = resultStatus.ToString();

        if (resultStatus == Solver.ResultStatus.OPTIMAL || resultStatus == Solver.ResultStatus.FEASIBLE)
        {
            result.TotalAdjustedNPV = objective.Value();
            
            foreach (var (data, projResult) in projects)
            {
                if (projectVars[data.ProjectId].SolutionValue() > 0.5)
                {
                    result.SelectedProjects.Add(data.ProjectId);
                    result.TotalInvestment += data.InitialCost;
                }
            }
            
            result.ProjectCount = result.SelectedProjects.Count;
            result.RemainingBudget = budget - result.TotalInvestment;
        }

        return result;
    }
}
