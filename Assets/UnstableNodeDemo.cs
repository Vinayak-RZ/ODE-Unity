using System.Collections.Generic;
using UnityEngine;

public class UnstableNodeDemo : MonoBehaviour
{
    [Header("Line Renderers for Visualization")]
    [SerializeField] private LineRenderer phaseSpaceRenderer;      // y1 vs y2
    [SerializeField] private LineRenderer state1Renderer;          // t vs y1
    [SerializeField] private LineRenderer state2Renderer;          // t vs y2
    [SerializeField] private LineRenderer shortIntervalRenderer;   // Short trajectory
    [SerializeField] private LineRenderer longIntervalRenderer;    // Long trajectory

    [Header("Toggle Graphs")]
    [SerializeField] private bool showPhaseSpace = true;
    [SerializeField] private bool showState1 = false;
    [SerializeField] private bool showState2 = false;
    [SerializeField] private bool showShortInterval = true;
    [SerializeField] private bool showLongInterval = true;

    [Header("ODE Parameters")]
    [SerializeField] private Vector2 initialState = new Vector2(0.5f, 0.5f);
    [SerializeField] private float stepSize = 0.01f;

    [Header("Unstable Node Settings")]
    [SerializeField] private float shortEndTime = 2f;   // small interval (readable)
    [SerializeField] private float longEndTime = 10f;   // long interval (blows up)

    private List<double[]> statePoints;
    private List<double> timePoints;

    private void Start()
    {
        Redraw();
    }

    // private void Update()
    // {
    //     Redraw();
    // }

    private void OnValidate()
    {
        Redraw();
    }

    private void Redraw()
    {
        if (!Application.isPlaying && !Application.isEditor) return;

        // === Define unstable node system ===
        OdeCustom.ODEFunc system = (t, y) =>
        {
            // dy1 = y1, dy2 = 2*y2 → eigenvalues (1, 2) → unstable node
            return new double[] { y[0], 2 * y[1] };
        };

        // --- Phase Space (using long interval for clarity) ---
        (timePoints, statePoints) = OdeCustom.Solve(system, 0, longEndTime,
                            new double[] { initialState.x, initialState.y }, stepSize);

        if (phaseSpaceRenderer != null)
        {
            phaseSpaceRenderer.enabled = showPhaseSpace;
            if (showPhaseSpace)
                DrawGraph(phaseSpaceRenderer, timePoints, statePoints, (t, y) => new Vector3((float)y[0], (float)y[1], 0), Color.yellow);
        }

        if (state1Renderer != null)
        {
            state1Renderer.enabled = showState1;
            if (showState1)
                DrawGraph(state1Renderer, timePoints, statePoints, (t, y) => new Vector3((float)t, (float)y[0], 0), Color.blue);
        }

        if (state2Renderer != null)
        {
            state2Renderer.enabled = showState2;
            if (showState2)
                DrawGraph(state2Renderer, timePoints, statePoints, (t, y) => new Vector3((float)t, (float)y[1], 0), Color.green);
        }

        // --- Short interval graph ---
        if (shortIntervalRenderer != null)
        {
            shortIntervalRenderer.enabled = showShortInterval;
            if (showShortInterval)
            {
                var (Ts, Ys) = OdeCustom.Solve(system, 0, shortEndTime,
                                               new double[] { initialState.x, initialState.y }, stepSize);
                DrawGraph(shortIntervalRenderer, Ts, Ys, (t, y) => new Vector3((float)y[0], (float)y[1], 0), Color.darkBlue);
            }
        }

        // --- Long interval graph ---
        if (longIntervalRenderer != null)
        {
            longIntervalRenderer.enabled = showLongInterval;
            if (showLongInterval)
            {
                var (Tl, Yl) = OdeCustom.Solve(system, 0, longEndTime,
                                               new double[] { initialState.x, initialState.y }, stepSize);
                DrawGraph(longIntervalRenderer, Tl, Yl, (t, y) => new Vector3((float)y[0], (float)y[1], 0), Color.red);
            }
        }
    }

    private void DrawGraph(LineRenderer renderer, List<double> T, List<double[]> Y,
                           System.Func<double, double[], Vector3> map, Color color)
    {
        renderer.positionCount = T.Count;
        renderer.startColor = color;
        renderer.endColor = color;
        for (int i = 0; i < T.Count; i++)
        {
            renderer.SetPosition(i, map(T[i], Y[i]));
        }
    }
}
