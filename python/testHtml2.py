from IPython.display import display, HTML
from matplotlib.animation import FuncAnimation
import matplotlib.pyplot as plt
import numpy as np

# 创建动态图像
fig, ax = plt.subplots()
xdata, ydata = [], []
ln, = plt.plot([], [], 'r-')

def init():
    ax.set_xlim(0, 2*np.pi)
    ax.set_ylim(-1, 1)
    return ln,

def update(frame):
    xdata.append(frame)
    ydata.append(np.sin(frame))
    ln.set_data(xdata, ydata)
    return ln,

# 创建HTML代码
fig_html = '<figure><canvas id="animation" width="400" height="300"></canvas></figure>'
script_html = '''
<script>
    var animation = document.getElementById("animation");
    var ctx = animation.getContext("2d");
    var frame = 0;

    function draw() {
        ctx.clearRect(0, 0, animation.width, animation.height);
        ctx.beginPath();
        ctx.moveTo(frame, 150);
        ctx.lineTo(frame, 150 - Math.sin(frame) * 100);
        ctx.strokeStyle = '#FF0000';
        ctx.stroke();
        frame += 0.1;
        requestAnimationFrame(draw);
    }

    draw();
</script>
'''

# 创建动态图像和HTML对象
anim = FuncAnimation(fig, update, frames=np.linspace(0, 2*np.pi, 100),
                    init_func=init, blit=True)
html = HTML(fig_html + script_html)

# 显示动态图像和HTML内容
display(anim)
display(html)
