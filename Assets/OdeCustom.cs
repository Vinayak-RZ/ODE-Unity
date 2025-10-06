using System;
using System.Collections.Generic;
using UnityEngine;

public static class OdeCustom
{
    // Delegate type for system of ODEs: f(t, y) -> dy/dt
    public delegate double[] ODEFunc(double t, double[] y);

    public static (List<double> T, List<double[]> Y) Solve(
        ODEFunc f,
        double t0,
        double tf,
        double[] y0,
        double relTol = 1e-6,
        double absTol = 1e-9,
        double h0 = 0.01,
        double hMax = 0.1,
        double hMin = 1e-6)
    {
        List<double> T = new List<double> { t0 };
        List<double[]> Y = new List<double[]> { (double[])y0.Clone() };

        double t = t0;
        double[] y = (double[])y0.Clone();
        double h = h0;

        while (t < tf)
        {
            if (t + h > tf)
                h = tf - t;

            // Perform one adaptive RK45 step
            var (yNew, err) = RK45Step(f, t, y, h);

            // Compute error norm
            double errNorm = 0;
            for (int i = 0; i < y.Length; i++)
            {
                double sc = absTol + Math.Max(Math.Abs(y[i]), Math.Abs(yNew[i])) * relTol;
                errNorm += Math.Pow(err[i] / sc, 2);
            }
            errNorm = Math.Sqrt(errNorm / y.Length);

            if (errNorm <= 1.0) // Accept step
            {
                t += h;
                y = yNew;

                T.Add(t);
                Y.Add((double[])y.Clone());
            }

            // Adapt step size
            double safety = 0.9;
            double factor = (errNorm == 0.0) ? 5.0 : safety * Math.Pow(errNorm, -0.2);
            factor = Math.Min(5.0, Math.Max(0.2, factor));
            h *= factor;
            h = Math.Min(h, hMax);
            h = Math.Max(h, hMin);
        }

        return (T, Y);
    }

    // One Dormand-Prince RK45 step
    private static (double[] yNew, double[] err) RK45Step(ODEFunc f, double t, double[] y, double h)
    {
        int n = y.Length;
        double[] k1 = f(t, y);

        double[] yTemp = new double[n];
        for (int i = 0; i < n; i++) yTemp[i] = y[i] + h * (1.0 / 5.0) * k1[i];
        double[] k2 = f(t + h * 1.0 / 5.0, yTemp);

        for (int i = 0; i < n; i++) yTemp[i] = y[i] + h * (3.0 / 40.0 * k1[i] + 9.0 / 40.0 * k2[i]);
        double[] k3 = f(t + h * 3.0 / 10.0, yTemp);

        for (int i = 0; i < n; i++) yTemp[i] = y[i] + h * (44.0 / 45.0 * k1[i] - 56.0 / 15.0 * k2[i] + 32.0 / 9.0 * k3[i]);
        double[] k4 = f(t + h * 4.0 / 5.0, yTemp);

        for (int i = 0; i < n; i++) yTemp[i] = y[i] + h * (19372.0 / 6561.0 * k1[i] - 25360.0 / 2187.0 * k2[i] +
                                                          64448.0 / 6561.0 * k3[i] - 212.0 / 729.0 * k4[i]);
        double[] k5 = f(t + h * 8.0 / 9.0, yTemp);

        for (int i = 0; i < n; i++) yTemp[i] = y[i] + h * (9017.0 / 3168.0 * k1[i] - 355.0 / 33.0 * k2[i] +
                                                          46732.0 / 5247.0 * k3[i] + 49.0 / 176.0 * k4[i] -
                                                          5103.0 / 18656.0 * k5[i]);
        double[] k6 = f(t + h, yTemp);

        for (int i = 0; i < n; i++) yTemp[i] = y[i] + h * (35.0 / 384.0 * k1[i] + 500.0 / 1113.0 * k3[i] +
                                                          125.0 / 192.0 * k4[i] - 2187.0 / 6784.0 * k5[i] +
                                                          11.0 / 84.0 * k6[i]);
        double[] k7 = f(t + h, yTemp);

        // 5th order solution
        double[] yNew = new double[n];
        for (int i = 0; i < n; i++)
            yNew[i] = y[i] + h * (35.0 / 384.0 * k1[i] + 500.0 / 1113.0 * k3[i] +
                                  125.0 / 192.0 * k4[i] - 2187.0 / 6784.0 * k5[i] +
                                  11.0 / 84.0 * k6[i]);

        // Error estimate = difference between 5th and 4th order
        double[] y4 = new double[n];
        for (int i = 0; i < n; i++)
            y4[i] = y[i] + h * (5179.0 / 57600.0 * k1[i] + 7571.0 / 16695.0 * k3[i] +
                                393.0 / 640.0 * k4[i] - 92097.0 / 339200.0 * k5[i] +
                                187.0 / 2100.0 * k6[i] + 1.0 / 40.0 * k7[i]);

        double[] err = new double[n];
        for (int i = 0; i < n; i++)
            err[i] = yNew[i] - y4[i];

        return (yNew, err);
    }
}
