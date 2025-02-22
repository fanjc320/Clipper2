import matplotlib.pyplot as plt
import numpy as np
from matplotlib.animation import FuncAnimation
import matplotlib.font_manager as fm

font = fm.FontProperties(fname="./msyh.ttc")

fig = plt.figure()
ax = fig.add_subplot(1, 1, 1)
ax.set_title("动态图", fontproperties=font)
ax.grid(True)
ax.set_xlabel("X轴", fontproperties=font)
ax.set_ylabel("Y轴", fontproperties=font)
line1, = ax.plot([], [], "b--", linewidth=2.0, label="sin示例")
line2, = ax.plot([], [], "g+", linewidth=2.0, label="cos示例")
ax.legend(loc="upper left", prop=font, shadow=True)

def init():
    line1, = ax.plot([], [], "b--", linewidth=2.0, label="sin示例")
    line2, = ax.plot([], [], "g+", linewidth=2.0, label="cos示例")
    return line1, line2

def update(frame):
    x = np.linspace(-np.pi + 0.1 * frame, np.pi + 0.1 * frame, 256, endpoint=True)
    y_cos, y_sin = np.cos(x), np.sin(x)

    ax.set_xlim(-4 + 0.1 * frame, 4 + 0.1 * frame)
    ax.set_xticks(np.linspace(-4 + 0.1 * frame, 4 + 0.1 * frame, 9, endpoint=True))
    ax.set_ylim(-1.0, 1.0)
    ax.set_yticks(np.linspace(-1, 1, 9, endpoint=True))

    line1, = ax.plot(x, y_cos, "b--", linewidth=2.0, label="sin示例")
    line2, = ax.plot(x, y_sin, "g+", linewidth=2.0, label="cos示例")

    return line1, line2

ani = FuncAnimation(fig
                   ,update
                   ,init_func=init
                   ,frames=np.linspace(-5 ,5, 5)
                   ,interval=1000
                   ,blit=True
                   )

ani.save("4.gif", fps=1, writer="pillow")

# ok too!
# fig = plt.figure()
# ax = fig.subplots()
# t = np.linspace(0, 10, 100)
# y = np.sin(t)
# ax.set_aspect(3)
# ax.plot(t, y, '--', c='gray')
# line = ax.plot(t, y, c='C2')
#
#
# def update(i):  # 帧更新函数
#     global t  # 直接引用全局变量，也可以通过函数的frames或fargs参数传递。
#     t += 0.1
#     y = np.sin(t)
#     line[0].set_ydata(y)
#     return line
#
#
# ani = FuncAnimation(fig, update, interval=100, frames=np.linspace(-5 ,5, 5))  # 绘制动画
# ani.save("1.gif", fps=1, writer="pillow")