import matplotlib
matplotlib.use('TkAgg')  # 使用 Tkinter 后端
# matplotlib.use('Qt5Agg')  # 如果安装了 PyQt5/PySide2

import matplotlib.pyplot as plt
import numpy as np
from matplotlib.patches import Polygon

# Define coordinates (example: a trapezoid)
coords_up = [(100, 0), (340, 40), (328, 85), (300, 50), (127, 136)]
coords_down = [(310, 156), (300, 200), (150, 250), (134, 171)]

# Create plot
fig, ax = plt.subplots(constrained_layout=True)
# plt.figure(figsize=(10, 10))
# plt.rcParams['figure.figsize'] = [16, 8]
fig.set_size_inches(10,10)

# polygonU = Polygon(coords_up, closed=True, edgecolor='grey', facecolor='grey', alpha=0.5)
# polygonD = Polygon(coords_down, closed=True, edgecolor='grey', facecolor='grey', alpha=0.5)
polygonU = Polygon(coords_up, closed=True, facecolor='grey', alpha=0.5)
polygonD = Polygon(coords_down, closed=True, facecolor='grey', alpha=0.5)
ax.add_patch(polygonU)
ax.add_patch(polygonD)

# Adjust axes to fit the polygon
ax.set_xlim(0, 400)
ax.set_ylim(-50, 400)
ax.invert_yaxis()
ax.set_aspect(1)
# fig.subplots_adjust(left=0, right=1, top=1, bottom=0)

plt.grid(True)
# Annotate each point with its coordinate.
for i in range(len(coords_up)):
    plt.annotate(f'{coords_up[i]}', coords_up[i], textcoords="offset points", xytext=(0,5), ha='center')
for i in range(len(coords_down)):
    plt.annotate(f'{coords_down[i]}', coords_down[i], textcoords="offset points", xytext=(0,5), ha='center')

plt.annotate((40, 180), (40, 180), textcoords="offset points", xytext=(0,5), ha='center')
plt.annotate((380, 150), (380, 150), textcoords="offset points", xytext=(0,5), ha='center')
# ---------------------------------

data = [
    {
        'pts': [(150, 250), (300, 200)],
        'front': [(150, 250), (100, 0)],
        'back': [(150, 250), (300, 200)]
    },
    {
        'pts': [(134, 171), (300, 200), (150, 250)],
        'front': [(150, 250), (100, 0)],
        'back': [(300, 200), (340, 40)]
    },
    {
        'pts': [(310, 156), (300, 200), (150, 250), (134, 171)],
        'front': [(40, 180), (380, 150)],
        'back': [(300, 200), (340, 40)]
    },
    {
        'pts': [(127, 136), (300, 50)],
        'front': [(150, 250), (100, 0)],
        'back': [(40, 180), (300, 50)]
    },
    {
        'pts': [(127, 136), (340, 40), (328, 85), (300, 50)],
        'front': [(150, 250), (100, 0)],
        'back': [(300, 200), (340, 40)]
    },
    {
        'pts': [(100, 0), (340, 40), (328, 85), (300, 50), (127, 136)],
        'front': [(150, 250), (100, 0)],
        'back': [(340, 40), (100, 0)]
    }
]

# 生成示例数据
frames = [i for i in range(6)]

class FramePlayer:
    def __init__(self, fig, ax, frames):
        self.fig = fig
        self.ax = ax
        self.frames = frames
        self.current_frame = -1

        # 初始化绘图
        # ax.set_title("Press → to next frame, ← to previous")
        fig.canvas.mpl_connect('key_press_event', self.on_key)

        self.left, = ax.plot([], [], 'r>-', linewidth=2, markersize=10)
        self.right, = ax.plot([], [], 'b>-', linewidth=2, markersize=10)
        self.pts, = ax.plot([], [], 'yo', markersize=18)

    def on_key(self, event):
        if event.key == 'right':
            self.current_frame = min(len(self.frames) - 1, self.current_frame + 1)
        elif event.key == 'left':
            self.current_frame = max(0, self.current_frame - 1)

        # 更新数据
        # self.line.set_ydata(self.frames[self.current_frame])
        self.ax.set_ylabel(f'Frame {self.current_frame}')

        print("left>>>>>>>>>>>>>>>>>>>>", self.current_frame)
        front_coords_x = [x for x, y in data[self.current_frame]['front']]
        front_coords_y = [y for x, y in data[self.current_frame]['front']]
        back_coords_x = [x for x, y in data[self.current_frame]['back']]
        back_coords_y = [y for x, y in data[self.current_frame]['back']]
        x_coords = [x for x, y in data[self.current_frame]['pts']]
        y_coords = [y for x, y in data[self.current_frame]['pts']]

        self.left.set_data(front_coords_x, front_coords_y)
        self.right.set_data(back_coords_x, back_coords_y)
        self.pts.set_data(x_coords, y_coords)

        self.fig.canvas.draw()


player = FramePlayer(fig, ax, frames)

plt.show()