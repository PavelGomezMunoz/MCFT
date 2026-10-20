using MCFT.Core.Models;
using MCFT.Core.Solvers;

var input = new MCFTInput
{
    FcPrime = 21.8,
    Fcr = 1.541,
    EpsilonNought = 0.002,
    EpsilonCr = 0.000071,
    Ec = 21800,
    RhoX = 0.02195,
    RhoY = 0.0,
    Fyx = 402,
    Fyy = 402,
    Es = 200000,

    Fx = 3.1,
    Fy = 0,
    Vxy = 1.0,

    AggregateSize = 19,
    CrackSpacingX = 115,
    CrackSpacingY = 115,

    Tolerance = 0.001,
    MaxIterations = 1000
};

var solver = new MCFTSolver();

var result = solver.Solve(input);

Console.WriteLine($"Converged: {result.Converged}"); 
Console.WriteLine($"Iterations: {result.Iterations}");
Console.WriteLine($"EpsilonX: {result.EpsilonX*1000} mm/m");
Console.WriteLine($"EpsilonY: {result.EpsilonY*1000} mm/m");
Console.WriteLine($"GammaXY: {result.GammaXY*1000} mm/m");
