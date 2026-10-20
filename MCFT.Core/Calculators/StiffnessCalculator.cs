using MathNet.Numerics.LinearAlgebra;

namespace MCFT.Core.Calculators
{
    public static class StiffnessCalculator
    {
        public static Matrix<double> ConcreteStiffnessMatrixLocal(double fc1, double fc2, double epsilon1, double epsilon2, double Ec)
        {
            double Ec1;
            double Ec2;
            double Gc;

            if (Math.Abs(fc2) < 0.001) { Ec2 = Ec; }
            else { Ec2 = fc2 / epsilon2; }

            if (fc1 < 0.001) { Ec1 = Ec; }
            else { Ec1 = fc1 / epsilon1; }

            Gc = (Ec1 * Ec2) / (Ec1 + Ec2);

            return Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { Ec2, 0, 0 },
                { 0, Ec1, 0 },
                { 0, 0, Gc }
            });
        }

        public static Matrix<double> SteelStiffnessMatrixGlobal(double Es, double rhoX, double rhoY, double epsilonX, double epsilonY, double fyX, double fyY)
        {
            double EsX;
            double EsY;

            if (epsilonX == 0) { EsX = Es; }
            else { EsX = Math.Min(Es, fyX / Math.Abs(epsilonX)); }

            if (epsilonY == 0) { EsY = Es; }
            else { EsY = Math.Min(Es, fyY / Math.Abs(epsilonY)); }

            return Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { EsX*rhoX, 0, 0 },
                { 0, EsY*rhoY, 0 },
                { 0, 0, 0 }
            });
        }
    }
}