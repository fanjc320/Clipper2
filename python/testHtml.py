import numpy as np
import matplotlib.pyplot as plt
import matplotlib.animation as animation
from IPython.display import HTML

# 创建数据
x = np.linspace(0, 2 * np.pi, 100)
y = np.sin(x)

# 创建绘图对象
fig, ax = plt.subplots()
line, = ax.plot(x, y)

# 动画更新函数
def update(frame):
    line.set_ydata(np.sin(x + frame / 10))  # 更新y数据，即每一帧
    return line,

# 创建动画
ani = animation.FuncAnimation(fig, update, frames=np.arange(0, 100), interval=100)

# 显示动画
plt.close(fig)  # 关闭静态图形
HTML(ani.to_jshtml())  # 在 Jupyter 中显示动画，这里我们需要更改