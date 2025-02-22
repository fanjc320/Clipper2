import matplotlib.pyplot as plt
import numpy as np
from matplotlib.patches import Polygon
from matplotlib.animation import FuncAnimation
from matplotlib.widgets import Button

# Define coordinates (example: a trapezoid)
coords_up = [(100, 0), (340, 50), (330, 87), (300, 50), (129, 148)]
coords_down = [(310, 160), (300, 200), (150, 250), (137, 185)]

# Create plot
fig, ax = plt.subplots(constrained_layout=True)
# fig, ax = plt.subplots()
# polygonU = Polygon(coords_up, closed=True, edgecolor='grey', facecolor='grey', alpha=0.5)
# polygonD = Polygon(coords_down, closed=True, edgecolor='grey', facecolor='grey', alpha=0.5)
polygonU = Polygon(coords_up, closed=True, facecolor='grey', alpha=0.5)
polygonD = Polygon(coords_down, closed=True, facecolor='grey', alpha=0.5)
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

def update(frame):
    # polygon1 = Polygon([(150, 250), (100, 0)], closed=True, edgecolor='b', facecolor='red', linestyle='--')
    # polygon2 = Polygon([(150, 250), (300, 200)], closed=True, edgecolor='r', facecolor='red', linestyle='--')
    # ax.add_patch(polygon1)
    # ax.add_patch(polygon2)

    pts = [(150, 250), (300, 200)]
    front = [(150, 250), (100, 0)]
    back = [(150, 250), (300, 200)]
    print("frame:", frame)
    ff = frame - 1
    if ff < 1:
        x1 = np.array([0, 0])
        y1 = np.array([0, 0])
        line = ax.plot(x1, y1, "black", animated=True)
        return line
    if ff > 1:
        # print("---------------->1-----------------------");
        pts = [(150,250), (300,200)]
        front = [(150,250), (100,0)]
        back = [(150,250),(300,200)]
    if ff > 2:
        pts = [(137, 185), (300, 200), (150,250)]
        front = [(150, 250), (100, 0)]
        back = [(300, 200), (340, 50)]
    if ff > 3:
        pts = [(310, 160), (300, 200), (150,250), (137, 185)]
        front = [(40, 200), (380, 150)]
        back = [(300, 200), (340, 50)]
    if ff > 4:
        pts = [(129, 148), (300, 50)]
        front = [(150, 250), (100, 0)]
        back = [(40, 200), (300, 50)]
    if ff > 5:
        pts = [(129, 148), (340, 50), (330, 87), (300, 50)]
        front = [(150, 250), (100, 0)]
        back = [(300, 200), (340, 50)]
    if ff > 6:
        pts = [(100, 0), (340, 50), (330, 87), (300, 50), (129, 148)]
        front = [(150, 250), (100, 0)]
        back = [(340, 50), (100, 0)]
        # print("---------------------------------------");

    # print("front:", front)
    # print("back:", back)
    # polygon1 = Polygon(front, closed=True, edgecolor='r', facecolor='red', linestyle='--')
    # polygon2 = Polygon(back, closed=True, edgecolor='b', facecolor='red', linestyle=':')
    # ax.add_patch(polygon1)
    # ax.add_patch(polygon2)

    front_coords_x = [x for x, y in front]
    front_coords_y = [y for x, y in front]
    back_coords_x = [x for x, y in back]
    back_coords_y = [y for x, y in back]
    ax.plot(front_coords_x, front_coords_y, 'r<', linewidth=2, markersize=12)
    ax.plot(back_coords_x, back_coords_y, 'b>', linewidth=2, markersize=12)

    x_coords = [x for x, y in pts]
    y_coords = [y for x, y in pts]
    ax.plot(x_coords, y_coords, 'yo', markersize=22)
    # ax.plot(x_coords, y_coords, 'yo')

    x1 = np.array([150, 300])
    y1 = np.array([250, 200])
    line3 = ax.plot(x1,y1, c="r", linestyle = 'dotted')

    # return line2 # return了 就不显示line2了,但在gif中显示,且有动画
    return line3 # return line3了，就不显示line3了，但在gif中显示

# ani = FuncAnimation(fig, update, interval=2000, frames=np.linspace(0 ,8, 8), blit=True)  # 绘制动画
ani = FuncAnimation(fig, update, interval=2000, frames=np.linspace(0 ,8, 8), blit=False)  # 绘制动画
ani.save("2.gif", fps=0.5, writer="pillow")

# def stop_animation(event):
#   ani.event_source.stop()
#
# def start_animation(event):
#   ani.event_source.start()
#
# stop = Button(ax_stop, 'Stop', color='red')
# start = Button(ax_start, 'Start', color='green')
#
# stop.on_clicked(stop_animation)
# start.on_clicked(start_animation)

plt.figure(figsize=(100,100))
plt.show()