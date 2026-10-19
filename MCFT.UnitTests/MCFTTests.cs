using System;
using MathNet.Numerics.LinearAlgebra;
using MCFT.Core.Calculators;
using MCFT.Core.Models;
using MCFT.Core.Solvers;
using MCFT.Core.Utilities;
using Xunit;

namespace MCFT.UnitTests
{
    internal static class Helper
    {
        public static void Close(double expected, double actual, double tol = 1e-6, string what = "")
        {
            Assert.True(Math.Abs(expected - actual) <= tol, $"{what} expected {expected}, got {actual} (tolerance {tol})");
        }

        public static Vector<double> Vec(double a, double b, double c) =>
            Vector<double>.Build.DenseOfArray(new[] { a, b, c });
    }

    public class StrainCalculatorTests
    {
        [Fact]
        public void PrincipalStrains_PureShear_AreEqualAndOpposite()
        {
            // ex = ey = 0, gamma = 0.002  ->  e1 = +0.001, e2 = -0.001
            var epsilonPrincipal = StrainCalculator.PrincipalStrainVector(Helper.Vec(0, 0, 0.002));

            Helper.Close(0.001, epsilonPrincipal[0], what: "e1");
            Helper.Close(-0.001,epsilonPrincipal[1], what: "e2");
        }
    }
}