import matplotlib.pyplot as plt
import numpy as np
import matplotlib
matplotlib.use('TkAgg')  # 使用 Tkinter 后端
# matplotlib.use('Qt5Agg')  # 如果安装了 PyQt5/PySide2
# okkkkkkkkkkkkkkkkk
# 生成示例数据
x = np.linspace(0, 4 * np.pi, 100)
frames = [np.sin(x + 0.1 * i) for i in range(50)]

class FramePlayer:
    def __init__(self, fig, ax, frames):
        self.fig = fig
        self.ax = ax
        self.frames = frames
        self.current_frame = 0

        # 初始化绘图
        self.line, = ax.plot(x, frames[0])
        ax.set_title("Press → to next frame, ← to previous")
        fig.canvas.mpl_connect('key_press_event', self.on_key)

    def on_key(self, event):
        if event.key == 'right':
            self.current_frame = min(len(self.frames) - 1, self.current_frame + 1)
        elif event.key == 'left':
            self.current_frame = max(0, self.current_frame - 1)

        # 更新数据
        self.line.set_ydata(self.frames[self.current_frame])
        self.ax.set_ylabel(f'Frame {self.current_frame}')
        self.fig.canvas.draw()


# 创建图形和坐标轴
fig, ax = plt.subplots()
player = FramePlayer(fig, ax, frames)
plt.show()