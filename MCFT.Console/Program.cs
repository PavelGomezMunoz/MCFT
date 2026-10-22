using MCFT.Core.Models;
using MCFT.Core.Solvers;

var input = new MCFTInput
{
    FcPrime = 21.8,
    Ft = 1.541,
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
    MaxIterations = 1000,

    NumberOfStepsInUnitStressVector = 5,
    CurveMaxNumberOfSteps = 1000,
    Refinements = 10
};

var stressStateSolver = new MCFTStressStateSolver();
var stressStateResult = stressStateSolver.Solve(input);
Console.WriteLine($"Iterations: {stressStateResult.Iterations}");
Console.WriteLine($"EpsilonX: {Math.Round(stressStateResult.EpsilonX*1000,3)} mm/m");
Console.WriteLine($"EpsilonY: {Math.Round(stressStateResult.EpsilonY*1000,3)} mm/m");
Console.WriteLine($"GammaXY: {Math.Round(stressStateResult.GammaXY*1000,3)} mm/m");

var stressStrainCurveSolver = new MCFTStressStrainCurveSolver();
var stressStrainCurveResult = stressStrainCurveSolver.Solve(input);
Console.WriteLine("Step  Lambda   SigmaX (MPa)   SigmaY (MPa)   Tau (MPa)   EpsX (mm/m)  EpsY (mm/m)  Gamma (mm/m)  Iter");
for (int i = 0; i < stressStrainCurveResult.CurveResultData.Count; i++)
{
    var p = stressStrainCurveResult.CurveResultData[i];
    Console.WriteLine($"{i + 1,4}  {p.LoadFactor,4:F3}  {p.SigmaX,11:F2}  {p.SigmaY,11:F2}  {p.TauXY,11:F2}  {p.EpsilonX * 1000,14:F3}  {p.EpsilonY * 1000,10:F3}  {p.GammaXY * 1000,10:F3}  {p.Iterations,10}");
}
Console.WriteLine($"Capacity load factor: {stressStrainCurveResult.CapacityLoadFactor:F3} (capacity reached: {stressStrainCurveResult.CapacityReached})");
