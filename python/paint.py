import matplotlib.pyplot as plt
import numpy as np
from matplotlib.patches import Polygon
from matplotlib.animation import FuncAnimation

# Define coordinates (example: a trapezoid)
coords_up = [(100, 0), (340, 50), (330, 87), (300, 50), (129, 148)]
coords_down = [(310, 160), (300, 200), (150, 250), (137, 185)]

# Create plot
fig, ax = plt.subplots(constrained_layout=True)
# fig, ax = plt.subplots()
polygonU = Polygon(coords_up, closed=True, edgecolor='black', facecolor='cyan')
polygonD = Polygon(coords_down, closed=True, edgecolor='black', facecolor='green')
ax.add_patch(polygonU)
ax.add_patch(polygonD)

# Adjust axes to fit the polygon
ax.set_xlim(0, 400)
ax.set_ylim(0, 400)
ax.invert_yaxis()
ax.set_aspect(1)
# fig.subplots_adjust(left=0, right=1, top=1, bottom=0)

plt.grid(True)
# Annotate each point with its coordinate.
for i in range(len(coords_up)):
    plt.annotate(f'{coords_up[i]}', coords_up[i], textcoords="offset points", xytext=(0,5), ha='center')
for i in range(len(coords_down)):
    plt.annotate(f'{coords_down[i]}', coords_down[i], textcoords="offset points", xytext=(0,5), ha='center')

# ---------------------------------
# t = np.linspace(0, 10, 100)
# y = np.sin(t)
# ax.set_aspect(3)
# ax.plot(t, y, '--', c='gray')
# line = ax.plot(t, y, c='C2')


# def update(i):  # 帧更新函数
#     x = (150,250)
#     y = (300,200)
#     line = plt.plot(x, y, linestyle='-', lw = 1, label='Solid Line')
#     plt.plot(x, y, marker='+', color='coral')
#     # ax.add_patch(line)
#     return line

# def init():
#     line1, = ax.plot([], [], "b--", linewidth=2.0, label="sin示例")
#     line2, = ax.plot([], [], "g+", linewidth=2.0, label="cos示例")
#     return line1, line2

def update(frame):
    # x = np.linspace(-np.pi + 0.1 * frame, np.pi + 0.1 * frame, 256, endpoint=True)
    x = np.linspace(-np.pi * frame, np.pi * frame, 128, endpoint=True)
    y_cos, y_sin = np.cos(x)*100, np.sin(x)*100
    #
    # line1, = ax.plot(x, y_cos, "b--", linewidth=2.0, label="sin示例")
    line2 = ax.plot(x, y_sin, "g+", linewidth=2.0, label="cos示例")

    polygon = Polygon([(100,200),(300,400)], closed=True, edgecolor='black', facecolor='red')
    ax.add_patch(polygon)

    x1 = np.array([10, 200, 200, 300])
    y1 = np.array([300, 170, 50, 190])
    line3 = ax.plot(x1,y1, c="r", linestyle = 'dotted')

    # return line2 # return了 就不显示line2了,但在gif中显示,且有动画
    return line3 # return line3了，就不显示line3了，但在gif中显示

ani = FuncAnimation(fig, update, interval=100, frames=np.linspace(0 ,100, 10), blit=True)  # 绘制动画
ani.save("1.gif", fps=1, writer="pillow")

plt.figure(figsize=(100,100))
plt.show()