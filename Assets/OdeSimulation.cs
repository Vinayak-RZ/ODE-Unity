using System.Collections.Generic;
using UnityEngine;

public class OdeSimulation : MonoBehaviour
{
    [Header("Line Renderers for Visualization")]
    [SerializeField] private LineRenderer phaseSpaceRenderer; // y1 vs y2
    [SerializeField] private LineRenderer state1Renderer;     // t vs y1
    [SerializeField] private LineRenderer state2Renderer;     // t vs y2

    [Header("Toggle Graphs")]
    [SerializeField] private bool showPhaseSpace = true;
    [SerializeField] private bool showState1 = false;
    [SerializeField] private bool showState2 = false;

    [Header("ODE Parameters")]
    [SerializeField] private float tStart = 0f;       
    [SerializeField] private float tEnd = 20f;        
    [SerializeField] private float stepSize = 0.01f;  
    [SerializeField] private Vector2 initialState = new Vector2(1f, 0f);

    [Header("System Type")]
    [SerializeField] private OdeSystemType systemType = OdeSystemType.Linear;

    [Header("Linear Coefficients (dy/dt = A * y)")]
    [SerializeField] private float a = 0f;
    [SerializeField] private float b = 1f;
    [SerializeField] private float c = -1f;
    [SerializeField] private float d = 0f;

    [Header("Van der Pol Parameters")]
    [SerializeField] private float mu = 1f;

    [Header("Lotka-Volterra Parameters")]
    [SerializeField] private float alpha = 1.0f;
    [SerializeField] private float beta = 0.1f;
    [SerializeField] private float delta = 0.1f;
    [SerializeField] private float gamma = 1.0f;

    private List<double[]> statePoints;
    private List<double> timePoints;

    private void Start()
    {
        Redraw();
    }

    private void Update()
    {
        Redraw();
    }

    private void OnValidate()
    {
        Redraw();
    }

    private void Redraw()
    {
        if (!Application.isPlaying && !Application.isEditor) return;

        // Define ODE system
        OdeCustom.ODEFunc system = (t, y) =>
        {
            double dy1, dy2;
            switch (systemType)
            {
                case OdeSystemType.VanDerPol:
                    dy1 = y[1];
                    dy2 = mu * (1 - y[0] * y[0]) * y[1] - y[0];
                    break;

                case OdeSystemType.LotkaVolterra:
                    dy1 = alpha * y[0] - beta * y[0] * y[1];
                    dy2 = delta * y[0] * y[1] - gamma * y[1];
                    break;

                default: // Linear
                    dy1 = a * y[0] + b * y[1];
                    dy2 = c * y[0] + d * y[1];
                    break;
            }
            return new double[] { dy1, dy2 };
        };

        // Solve
        (timePoints, statePoints) = OdeCustom.Solve(system, tStart, tEnd,
                                                    new double[] { initialState.x, initialState.y },
                                                    stepSize);

        // === Draw Phase Space (y1 vs y2) ===
        if (phaseSpaceRenderer != null)
        {
            phaseSpaceRenderer.enabled = showPhaseSpace;
            if (showPhaseSpace)
            {
                phaseSpaceRenderer.positionCount = timePoints.Count;
                for (int i = 0; i < timePoints.Count; i++)
                {
                    float y1 = (float)statePoints[i][0];
                    float y2 = (float)statePoints[i][1];
                    phaseSpaceRenderer.SetPosition(i, new Vector3(y1, y2, 0f));
                }
            }
        }

        // === Draw State 1 (t vs y1) ===
        if (state1Renderer != null)
        {
            state1Renderer.enabled = showState1;
            if (showState1)
            {
                state1Renderer.positionCount = timePoints.Count;
                for (int i = 0; i < timePoints.Count; i++)
                {
                    float t = (float)timePoints[i];
                    float y1 = (float)statePoints[i][0];
                    state1Renderer.SetPosition(i, new Vector3(t, y1, 0f));
                }
            }
        }

        // === Draw State 2 (t vs y2) ===
        if (state2Renderer != null)
        {
            state2Renderer.enabled = showState2;
            if (showState2)
            {
                state2Renderer.positionCount = timePoints.Count;
                for (int i = 0; i < timePoints.Count; i++)
                {
                    float t = (float)timePoints[i];
                    float y2 = (float)statePoints[i][1];
                    state2Renderer.SetPosition(i, new Vector3(t, y2, 0f));
                }
            }
        }
    }
}

public enum OdeSystemType
{
    Linear,
    VanDerPol,
    LotkaVolterra
}
