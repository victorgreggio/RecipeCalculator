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
    /// Optimizes portfolio selection using integer linear programming
    /// </summary>
    /// <param name="projects">List of project results with NPV and costs</param>
    /// <param name="budget">Maximum budget available for investment</param>
    /// <returns>Optimization result with selected projects</returns>
    public static OptimizationResult OptimizePortfolio(List<ProjectResult> projects, double budget)
    {
        var result = new OptimizationResult();
        
        // Create the linear solver with the SCIP backend (open source)
        Solver solver = Solver.CreateSolver("SCIP");
        if (solver == null)
        {
            result.Status = "SCIP solver unavailable. Trying CBC...";
            solver = Solver.CreateSolver("CBC");
            if (solver == null)
            {
                result.Status = "No solver available!";
                return result;
            }
        }

        // Create binary decision variables for each project (0 = not selected, 1 = selected)
        var projectVars = new Dictionary<string, Variable>();
        foreach (var project in projects)
        {
            projectVars[project.ProjectId] = solver.MakeIntVar(0, 1, project.ProjectId);
        }

        // Objective: Maximize total Adjusted NPV
        Objective objective = solver.Objective();
        foreach (var project in projects)
        {
            // Only include projects with positive adjusted NPV
            if (project.AdjustedNPV > 0)
            {
                objective.SetCoefficient(projectVars[project.ProjectId], project.AdjustedNPV);
            }
        }
        objective.SetMaximization();

        // Constraint: Total initial cost must not exceed budget
        Constraint budgetConstraint = solver.MakeConstraint(0, budget, "budget_constraint");
        foreach (var project in projects)
        {
            // We need to get initial cost from the project - for now use a placeholder
            // In real scenario, you'd pass ProjectData with InitialCost
            double initialCost = Math.Abs(project.NPV) > 0 ? project.NPV * 0.7 : 100000; // Estimate
            budgetConstraint.SetCoefficient(projectVars[project.ProjectId], initialCost);
        }

        // Solve the problem
        var startTime = DateTime.UtcNow;
        Solver.ResultStatus resultStatus = solver.Solve();
        var solveTime = (DateTime.UtcNow - startTime).TotalSeconds;

        result.SolveTime = solveTime;
        result.Status = resultStatus.ToString();

        if (resultStatus == Solver.ResultStatus.OPTIMAL || resultStatus == Solver.ResultStatus.FEASIBLE)
        {
            // Extract selected projects
            result.TotalAdjustedNPV = objective.Value();
            
            foreach (var project in projects)
            {
                if (projectVars[project.ProjectId].SolutionValue() > 0.5) // Binary variable = 1
                {
                    result.SelectedProjects.Add(project.ProjectId);
                    double initialCost = Math.Abs(project.NPV) > 0 ? project.NPV * 0.7 : 100000;
                    result.TotalInvestment += initialCost;
                }
            }
            
            result.ProjectCount = result.SelectedProjects.Count;
            result.RemainingBudget = budget - result.TotalInvestment;
        }

        return result;
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
