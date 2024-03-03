using System;

public class Spline {
    private float[] a;
    private float[] b;

    private float[] xOrig;
    private float[] yOrig;

    private int _lastIndex = 0;

    public float[] FitAndEval(float[] x, float[] y, float[] xs, float startSlope = float.NaN, float endSlope = float.NaN) {
        Fit(x, y, startSlope, endSlope);
        return Eval(xs);
    }

    public void Fit(float[] x, float[] y, float startSlope = float.NaN, float endSlope = float.NaN) {
        if(Single.IsInfinity(startSlope) || Single.IsInfinity(endSlope)) {
            throw new Exception("startSlope and endSlope cannot be infinity.");
        }

        this.xOrig = x;
        this.yOrig = y;

        int n = x.Length;
        float[] r = new float[n];

        TriDiagonalMatrixF m = new TriDiagonalMatrixF(n);
        float dx1, dx2, dy1, dy2;
        
        if(float.IsNaN(startSlope)) {
            dx1 = x[1] - x[0];
            m.C[0] = 1.0f / dx1;
            m.B[0] = 2.0f * m.C[0];
            r[0] = 3 * (y[1] - y[0]) / (dx1 * dx1);
        } else {
            m.B[0] = 1;
            r[0] = startSlope;
        }

        for(int i = 1; i < n - 1; i++) {
            dx1 = x[i] - x[i - 1];
            dx2 = x[i + 1] - x[i];

            m.A[i] = 1.0f / dx1;
            m.C[i] = 1.0f / dx2;
            m.B[i] = 2.0f * (m.A[i] + m.C[i]);

            dy1 = y[i] - y[i - 1];
            dy2 = y[i + 1] - y[i];
            r[i] = 3 * (dy1 / (dx1 * dx1) + dy2 / (dx2 * dx2));
        }

        if(float.IsNaN(endSlope)) {
            dx1 = x[n - 1] - x[n - 2];
            dy1 = y[n - 1] - y[n - 2];
            m.A[n - 1] = 1.0f / dx1;
            m.B[n - 1] = 2.0f * m.A[n - 1];
            r[n - 1] = 3 * (dy1 / (dx1 * dx1));
        } else {
            m.B[n - 1] = 1;
            r[n - 1] = endSlope;
        }

        float[] k = m.Solve(r);

        this.a = new float[n - 1];
        this.b = new float[n - 1];

        for(int i = 1; i < n; i++) {
            dx1 = x[i] - x[i - 1];
            dy1 = y[i] - y[i - 1];
            a[i - 1] = k[i - 1] * dx1 - dy1; 
            b[i - 1] = -k[i] * dx1 + dy1;
        }
    }

    public float[] Eval(float[] x) {
        CheckAlreadyFitted();

        int n = x.Length;
        float[] y = new float[n];
        _lastIndex = 0;

        for(int i = 0; i < n; i++) {
            int j = GetNextXIndex(x[i]);

            y[i] = EvalSpline(x[i], j);
        }

        return y;
    }

    private void CheckAlreadyFitted() {
        if(a == null) throw new Exception("Fit must be called before you can evaluate.");
    }

    private int GetNextXIndex(float x) {
        if(x < xOrig[_lastIndex]) {
            throw new ArgumentException("The X values to evaluate must be sorted.");
        }

        while((_lastIndex < xOrig.Length - 2) && (x > xOrig[_lastIndex + 1])) {
            _lastIndex++;
        }

        return _lastIndex;
    }

    private float EvalSpline(float x, int j) {
        float dx = xOrig[j + 1] - xOrig[j];
        float t = (x - xOrig[j]) / dx;
        float y = (1 - t) * yOrig[j] + t * yOrig[j + 1] + t * (1 - t) * (a[j] * (1 - t) + b[j] * t);
        return y;
    }

    public static void FitParametric(float[] x, float[] y, int nOutputPoints, out float[] xs, out float[] ys,
    float firstDx = Single.NaN, float firstDy = Single.NaN, float lastDx = Single.NaN, float lastDy = Single.NaN) {
        int n = x.Length;
        float[] dists = new float[n];
        dists[0] = 0;
        float totalDist = 0;

        for(int i = 1; i < n; i++) {
            float dx = x[i] - x[i - 1];
            float dy = y[i] - y[i - 1];
            float dist = (float)Math.Sqrt(dx * dx + dy * dy);
            totalDist += dist;
            dists[i] = totalDist;
        }

        float dt = totalDist / (nOutputPoints - 1);
        float[] times = new float[nOutputPoints];
        times[0] = 0;

        for(int i = 1; i < nOutputPoints; i++) {
            times[i] = times[i - 1] + dt;
        }

        NormalizeVector(ref firstDx, ref firstDy);
        NormalizeVector(ref lastDx, ref lastDy);

        Spline xSpline = new Spline();
        xs = xSpline.FitAndEval(dists, x, times, firstDx / dt, lastDx / dt);

        Spline ySpline = new Spline();
        ys = ySpline.FitAndEval(dists, y, times, firstDy / dt, lastDy / dt);
    }

    private static void NormalizeVector(ref float dx, ref float dy) {
        if(!Single.IsNaN(dx) && !Single.IsNaN(dy)) {
            float d = (float)Math.Sqrt(dx * dx + dy * dy);

            if(d > Single.Epsilon)
            {
                dx = dx / d;
                dy = dy / d;
            } else {
                throw new ArgumentException("The input vector is too small to be normalized.");
            }
        } else {
            dx = dy = Single.NaN;
        }
    }
}
