# Unity Adaptive ODE Solver Visualization

A Unity project demonstrating a **custom adaptive Runge–Kutta ODE solver (RK45)** in C#, with real-time visualization of 2D dynamical systems in phase space. Inspired by MATLAB's `ode45`, it allows experimentation with linear systems and interactive trajectory plotting.
<img width="1483" height="814" alt="Screenshot 2025-09-24 193903" src="https://github.com/user-attachments/assets/26877a4a-b795-49a4-b41f-3f7257f029dc" />

---

## Features

- **Adaptive RK45 Solver**: Custom implementation of Dormand–Prince 4(5) method.
- **Phase Space Visualization**: Real-time plotting of 2D system trajectories.
- **Configurable Systems**: Easily define linear ODEs through coefficients.
- **Interactive Parameters**: Modify initial conditions, time range, and step size in the Unity Inspector.
- **Extensible**: Can be extended to nonlinear systems or multiple trajectories.

---

## Example Systems

### Simple Harmonic Oscillator
$y_1' = y_2, \quad y_2' = -y_1$  
Coefficients: `a = 0, b = 1, c = -1, d = 0`

### Damped Oscillator
$y_1' = y_2, \quad y_2' = -y_1 - 0.5\,y_2$  
Coefficients: `a = 0, b = 1, c = -1, d = -0.5`

### Exponential Growth/Decay
$y_1' = y_1, \quad y_2' = -y_2$  
Coefficients: `a = 1, b = 0, c = 0, d = -1`

---

## Quick Start

1. Open the project in Unity.
2. Attach `OdeSimulation.cs` to an empty GameObject.
3. Assign a `LineRenderer` component in the Inspector.
4. Configure ODE parameters (`tStart`, `tEnd`, `stepSize`, `initialState`, `a, b, c, d`).
5. Run the scene to visualize the trajectory in phase space.

---

## Project Structure

Assets/
├─ Scripts/
│ ├─ OdeCustom.cs # Adaptive RK45 solver
│ └─ OdeSimulation.cs # Simulation & visualization
├─ Scenes/
│ └─ MainScene.unity
└─ Prefabs/
└─ TrajectoryLine.prefab

---

## Future Enhancements

- Support for **nonlinear ODEs** and user-defined functions.
- Interactive runtime parameter adjustments.
- Multiple trajectory visualization with color coding.
- Dense output for querying solutions at arbitrary times.

---

## License

MIT License – free to use, modify, and distribute.

---

## References

- MATLAB `ode45`: [https://www.mathworks.com/help/matlab/ref/ode45.html](https://www.mathworks.com/help/matlab/ref/ode45.html)  
- Dormand–Prince RK45 method: [https://en.wikipedia.org/wiki/Dormand%E2%80%93Prince_method](https://en.wikipedia.org/wiki/Dormand%E2%80%93Prince_method)
