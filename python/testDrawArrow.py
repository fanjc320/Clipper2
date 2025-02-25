import matplotlib.pyplot as plt
import numpy as np
import matplotlib
matplotlib.use('TkAgg')  # 使用 Tkinter 后端
# Generate example points (spiral)
n_points = 20
theta = np.linspace(0, 4*np.pi, n_points)
x = theta * np.cos(theta)
y = theta * np.sin(theta)

# Calculate direction vectors between consecutive points
dx = np.diff(x)
dy = np.diff(y)

# Create plot
plt.figure(figsize=(8, 6))

# Plot the dots
plt.scatter(x, y, c='red', s=50, zorder=3, label='Points')

# Draw arrows between consecutive points
plt.quiver(
    x[:-1], y[:-1],  # Starting points
    dx, dy,          # Direction vectors
    angles='xy',     # Ensure angles match data coordinates
    scale_units='xy',
    scale=1,         # Maintain true scaling
    color='blue',
    width=0.005,     # Adjust arrow thickness
    headwidth=5,     # Adjust arrowhead size
    headlength=7,
    label='Arrows'
)

plt.title('Arrows Between Points')
plt.xlabel('X')
plt.ylabel('Y')
plt.axis('equal')  # Maintain aspect ratio
plt.grid(True)
plt.legend()
plt.show()
