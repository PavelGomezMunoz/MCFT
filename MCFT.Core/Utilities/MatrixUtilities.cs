
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace MCFT.Core.Utilities
{
    public static class MatrixUtilities
    {
        public static Matrix<double> Rotate(Matrix<double> matrix, double angleDegrees)
        {
            double angleRadians = angleDegrees * Math.PI / 180.0;
            double cosPsi = Math.Cos(angleRadians);
            double sinPsi = Math.Sin(angleRadians);
            var rotationMatrix = Matrix<double>.Build.DenseOfArray(new double[,]
            {
                { cosPsi*cosPsi, sinPsi*sinPsi, cosPsi*sinPsi },
                { sinPsi*sinPsi, cosPsi*cosPsi, -cosPsi*sinPsi },
                { -2.0*cosPsi*sinPsi, 2.0*cosPsi*sinPsi, (cosPsi*cosPsi - sinPsi*sinPsi) }    
            });
            return rotationMatrix.Transpose() * matrix * rotationMatrix;
        }
    }
}
